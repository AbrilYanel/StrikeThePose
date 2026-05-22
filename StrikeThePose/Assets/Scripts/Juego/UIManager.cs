using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI livesText;

    [Header("Feedback de acierto/fallo")]
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private float feedbackDuration = 0.7f;

    [Header("Panel de victoria")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winScoreText;
    [SerializeField] private TextMeshProUGUI winComboText;
    [SerializeField] private Button winRetryButton;

    [Header("Panel de derrota")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TextMeshProUGUI loseMissesText;
    [SerializeField] private Button loseRetryButton;

    [Header("Panel de tutorial (intro)")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Button tutorialStartButton;

 
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    private static readonly string[] HitMessages = { "PERFECT!", "NICE!", "GREAT!" };
    private static readonly string[] MissMessages = { "MISS", "TOO LATE", "WRONG POSE", "NOPE" };

    private Coroutine _feedbackCoroutine;

    private void Start()
    {
        winPanel?.SetActive(false);
        losePanel?.SetActive(false);

        winRetryButton?.onClick.AddListener(RetryLevel);
        loseRetryButton?.onClick.AddListener(RetryLevel);
        tutorialStartButton?.onClick.AddListener(HideTutorialPanel);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOverEvent.AddListener(ShowLosePanel);
            GameManager.Instance.OnGameWonEvent.AddListener(ShowWinPanel);
        }

       
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        if (scoreText != null)
            scoreText.text = $"Puntos: {GameManager.Instance.Score}";

        if (comboText != null)
            comboText.text = GameManager.Instance.Combo > 1
                ? $"x{GameManager.Instance.Combo} COMBO"
                : "";

        if (livesText != null)
        {
            string hearts = "";
            for (int i = 0; i < GameManager.Instance.Lives; i++) hearts += "♥ ";
            livesText.text = hearts.TrimEnd();
        }
    }

    public void ShowHitFeedback(bool success)
    {
        if (feedbackText == null) return;

        if (_feedbackCoroutine != null)
            StopCoroutine(_feedbackCoroutine);

        _feedbackCoroutine = StartCoroutine(FeedbackRoutine(success));

        if (success && CameraEffects.Instance != null)
            CameraEffects.Instance.PlaySuccessFeedback();
    }

    private IEnumerator FeedbackRoutine(bool success)
    {
        feedbackText.gameObject.SetActive(true);

        if (success)
        {
            feedbackText.text = HitMessages[Random.Range(0, HitMessages.Length)];
            feedbackText.color = new Color(0.2f, 1f, 0.4f);
        }
        else
        {
            feedbackText.text = MissMessages[Random.Range(0, MissMessages.Length)];
            feedbackText.color = new Color(1f, 0.25f, 0.25f);
        }

        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 1.3f;
        Vector3 endScale = Vector3.one;
        feedbackText.transform.localScale = startScale;

        while (elapsed < feedbackDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / feedbackDuration;
            feedbackText.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            Color c = feedbackText.color;
            feedbackText.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        feedbackText.gameObject.SetActive(false);
    }

    private void ShowWinPanel(int score, int maxCombo)
    {
        winPanel?.SetActive(true);
        if (winScoreText != null) winScoreText.text = $"Puntuación: {score}";
        if (winComboText != null) winComboText.text = $"Combo máximo: {maxCombo}x";
    }

    private void ShowLosePanel()
    {
        losePanel?.SetActive(true);
        if (loseMissesText != null)
            loseMissesText.text = "Te quedaste sin vidas";
    }

    private void HideTutorialPanel()
    {
        tutorialPanel?.SetActive(false);

        
        Time.timeScale = 1f;
        obstacleSpawner?.StartGame();
    }

    private void RetryLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}