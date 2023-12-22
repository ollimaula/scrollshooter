using UnityEngine;
using UnityEngine.UI;

public class CanvasOverlay : MonoBehaviour
{
    public Sprite heatbar_sprite;
    public bool paused = false;

    [SerializeField] private Text score_text;
    [SerializeField] private Image heatbar;
    [SerializeField] private Text game_over;
    [SerializeField] private Text final_score;
    [SerializeField] private Text hs_info;
    [SerializeField] private Text pause;
    [SerializeField] private GameObject player_name;

    private GameObject _game_ender;

    void Start()
    {
        heatbar = transform.Find("Overheatbar").GetComponent<Image>();
        // Workaround to get an inactive gameobject
        _game_ender = GameObject.Find("GameEnderHolder").transform.Find("GameEnder").gameObject;
        game_over.enabled = false;
        final_score.enabled = false;
        hs_info.enabled = false;
        pause.enabled = false;
        player_name.SetActive(false);
    }
    void Update()
    {
        if (score_text != null)
            score_text.text = $"Score: {ScoreKeeper.score}";
        // heatbar_sprite is controlled by "heatbar" animation controller
        heatbar.sprite = heatbar_sprite;
        if (Input.GetKeyDown(KeyCode.Escape) && !_game_ender.activeSelf)
            PauseButton();
    }
    private void PauseButton()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            pause.enabled = false;
            paused = false;
        }
        else
        {
            Time.timeScale = 0f;
            pause.enabled = true;
            paused = true;
        }  
    }
}
