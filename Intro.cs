using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [SerializeField] private Text[] text;
    [SerializeField] private Text info;
    [SerializeField] private GameObject title;
    [SerializeField] private AudioSource ui_sound_move;
    [SerializeField] private AudioSource ui_sound_choose;

    private List<HighScore> _high_scores;
    private RectTransform _title_rect;
    private Text _high_score_list;

    private bool _choice = true;
    private bool _hs = false;
    private bool _sb_has_run = false;
    private const int _game_scene = 1;

    private void Start()
    {
        info.enabled = false;
        foreach (Text item in text)
            item.enabled = false;
        _title_rect = title.GetComponent<RectTransform>();
        _high_score_list = GameObject.Find("HighScoreList").GetComponent<Text>();
        _high_score_list.text = "";
        StartCoroutine(GetScores(results => { _high_scores = results; }));
    }
    void Update()
    {
        if (!_hs)
        {
            if (_title_rect.anchoredPosition.y > 25f)
                _title_rect.anchoredPosition -= (50f * Time.deltaTime * Vector2.up);
            if (transform.position.y < -3.20)
                transform.Translate(0f, 2f * Time.deltaTime, 0f);
            foreach (Text item in text)
                item.enabled = true;
            if (Input.GetKeyDown(KeyCode.S))
            {
                ui_sound_move.Play();
                text[0].text = "New Game";
                text[1].text = "> High Scores <";
                _choice = false;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                ui_sound_move.Play();
                text[0].text = "> New Game <";
                text[1].text = "High Scores";
                _choice = true;
            }
            else if (Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return))
            {
                if (_choice)
                {
                    ui_sound_choose.Play();
                    SceneManager.LoadScene(_game_scene);
                }
                else
                {
                    ui_sound_choose.Play();
                    _hs = true;
                    StartCoroutine(WaitToEnable());
                    foreach (Text item in text)
                        item.enabled = false;
                }
            }
        }
        else
        {
            StartCoroutine(WaitForShip());
            if (transform.position.y < 6f)
                transform.Translate(0f, 4f * Time.deltaTime, 0f);
            if (_title_rect.anchoredPosition.y >= 180f)
                info.enabled = true;
            if (Input.anyKeyDown && transform.position.y > 5.9f)
            {
                _hs = false;
                info.enabled = false;
                _high_score_list.enabled = false;
                transform.position = new Vector3(0f, -6f, 0f);
            }
        }
    }
    IEnumerator GetScores(System.Action<List<HighScore>> callback = null)
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
    private IEnumerator WaitToEnable()
    {
        yield return new WaitForSeconds(1f);
        _high_score_list.enabled = true;
    }
    private IEnumerator WaitForShip()
    {
        yield return new WaitForSeconds(1f);
        ShowHighScores();
    }
    private void ShowHighScores()
    {
        if (!_sb_has_run)
        {
            _high_score_list.text = BuildString();
            _sb_has_run = true;
        }
        if (_title_rect.anchoredPosition.y < 180f)
            _title_rect.anchoredPosition += (50f * Time.deltaTime * Vector2.up);
    }
    private string BuildString()
    {
        StringBuilder sb = new();
        try
        {
            foreach (HighScore hs in _high_scores)
                sb.AppendLine($"{hs.name} : {hs.score}");
        }
        catch (Exception e) { Debug.Log("string builder: " + e.Message); }
        return sb.ToString();
    }
}
