using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEnder : MonoBehaviour
{
    [SerializeField] private RectTransform final_score_rect;
    [SerializeField] private RectTransform hs_info_rect;
    [SerializeField] private Text game_over;
    [SerializeField] private Text final_score;
    [SerializeField] private Text hs_info;
    [SerializeField] private GameObject name_field;
    [SerializeField] private EndOverlay end_overlay;
    [SerializeField] private AudioSource submit_sound;

    private HighScore _old_hs;
    private List<HighScore> _high_scores;

    private bool _hs;
    private bool _no_hs;
    private float _alpha = 0;
    private const int _game_scene = 1;
    private const int _title_scene = 0;

    void Start()
    {
        StartCoroutine(CheckScore(ScoreKeeper.score, result => { _old_hs = result; }));
        StartCoroutine(GameOver());
    }
    void Update()
    {
        if (_alpha < 0.8f)
        {
            _alpha += (1f * Time.deltaTime);
            end_overlay.SetAlpha(_alpha);
        }
        if (_hs)
        {
            if (final_score_rect.anchoredPosition.y < -52f)
                final_score_rect.anchoredPosition += (50f * Time.deltaTime * new Vector2(0f, 1f));
            else
            {
                hs_info.enabled = true;
                ScoreKeeper.score = 0;
                if (Input.GetKeyDown(KeyCode.Return))
                    SceneManager.LoadScene(1);
                else if (Input.GetKeyDown(KeyCode.Escape))
                    SceneManager.LoadScene(0);
            }
        }
        else if (_no_hs)
        {
            ScoreKeeper.score = 0;
            if (Input.GetKeyDown(KeyCode.Return))
                SceneManager.LoadScene(_game_scene);
            else if (Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadScene(_title_scene);
        }
    }
    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("Score").SetActive(false);
        GameObject.Find("Overheatbar").SetActive(false);
        yield return new WaitForSeconds(0.5f);
        game_over.enabled = true;
        final_score.text = "Final Score: " + ScoreKeeper.score.ToString();
        final_score.enabled = true;
        if (_old_hs != null)
        {
            hs_info.enabled = true;
            name_field.SetActive(true);
        }
        else
        {
            hs_info.text = "Press enter to play again!\nPress esc to go back to the title screen.";
            hs_info.enabled = true;
            _no_hs = true;
        }
    }
    public void CreateHighScore(string input)
    {
        submit_sound.Play();
        StartCoroutine(UploadScore(input, ScoreKeeper.score));
        game_over.enabled = false;
        hs_info.enabled = false;
        name_field.SetActive(false);
        StartCoroutine(ShowHighScores());
    }
    private IEnumerator ShowHighScores()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(GetScores(results => { _high_scores = results; }));
        yield return new WaitForSeconds(1);
        string scores = BuildString();
        hs_info.text = "Press enter to play again!\nPress esc to go back to the title screen.";
        hs_info_rect.anchoredPosition = new Vector2(0f, 180f);
        final_score_rect.anchoredPosition = new Vector2(0f, -400f);
        final_score.text = scores;
        _hs = true;
    }
    private string BuildString()
    {
        StringBuilder sb = new();
        try
        {
            foreach (HighScore hs in _high_scores)
                if (hs._id == _old_hs._id)
                    sb.AppendLine($"-> {hs.name} : {hs.score} <-");
                else
                    sb.AppendLine($"{hs.name} : {hs.score}");
        }
        catch (Exception e) { Debug.Log("string builder: " + e.Message); }
        return sb.ToString();
    }
    private IEnumerator CheckScore(int score, System.Action<HighScore> callback = null)
    {
        using UnityWebRequest request = UnityWebRequest.Get("https://eu-central-1.aws.data.mongodb-api.com/app/application-0-umgbk/endpoint/check?score=" + score);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
            callback?.Invoke(null);
        }
        else
        {
            try { callback?.Invoke(HighScore.Parse(request.downloadHandler.text)); }
            catch { callback?.Invoke(null); }
        }
    }
    private IEnumerator GetScores(System.Action<List<HighScore>> callback = null)
    {
        using UnityWebRequest request = UnityWebRequest.Get("https://eu-central-1.aws.data.mongodb-api.com/app/application-0-umgbk/endpoint/get");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
            callback?.Invoke(null);
        }
        else
        {
            List<HighScore> results = HighScore.ParseList(request.downloadHandler.text);
            callback?.Invoke(results);
        }
    }
    private IEnumerator UploadScore(string name, int score)
    {
        string url = "https://eu-central-1.aws.data.mongodb-api.com/app/application-0-umgbk/endpoint/update?id=" + _old_hs._id;
        
        string clean_name = SanitizeInput(name);
        string json = $"{{\"name\": \"{SanitizeInput(clean_name)}\", \"score\": {score}}}"; // Keep It Simple, Stupid

        UnityWebRequest request = UnityWebRequest.Put(url, json);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Score upload failed: " + request.error);
        else
            Debug.Log("Score uploaded successfully!");
    }
    private string SanitizeInput(string input)
    {
        input ??= "Anonymous";

        input = Regex.Replace(input, "[^a-zA-Z0-9äöåÄÖÅ ]", "");
        input = Regex.Replace(input, @"\s+", " ");

        if (input.Length > 40)
            input = input[..40];
        else if (input.Length == 0)
            input = "Anonymous";

        return input;
    }
}
