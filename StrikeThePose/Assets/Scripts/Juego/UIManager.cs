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

    [Header("Tutorial en gameplay")]
    [Tooltip("Texto grande que muestra la tecla/pose del próximo obstáculo")]
    [SerializeField] private TextMeshProUGUI tutorialHintText;
    [Tooltip("Fondo opcional detrás del texto de tutorial")]
    [SerializeField] private GameObject tutorialHintBackground;

    [Header("Panel de pausa")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Referencias")]
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private AudioSource musicSource;

    // ── Mensajes de feedback ──────────────────────────────────────────────────
    private static readonly string[] HitMessages = { "PERFECTO!", "BIEN!", "GENIAL!" };
    private static readonly string[] MissMessages = { "MISS", "TARDE", "INCORRECTO", "NOPE" };

    private Coroutine _feedbackCoroutine;

    public bool IsTutorialHintActive { get; private set; } = false;

    private bool _isPaused = false;
    private bool _gameStarted = false;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Start()
    {
        winPanel?.SetActive(false);
        losePanel?.SetActive(false);
        pausePanel?.SetActive(false);

        winRetryButton?.onClick.AddListener(RetryLevel);
        loseRetryButton?.onClick.AddListener(RetryLevel);
        tutorialStartButton?.onClick.AddListener(HideTutorialPanel);
        resumeButton?.onClick.AddListener(ResumeGame);
        restartButton?.onClick.AddListener(RetryLevel);
        menuButton?.onClick.AddListener(GoToMainMenu);

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

        if (tutorialHintText != null)
            tutorialHintText.gameObject.SetActive(false);
        if (tutorialHintBackground != null)
            tutorialHintBackground.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        if (_gameStarted && !GameManager.Instance.IsGameOver && !GameManager.Instance.IsGameWon)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (_isPaused) ResumeGame(); else PauseGame();
            }
        }

        if (_isPaused) return;

        if (scoreText != null)
            scoreText.text = $"Puntos: {GameManager.Instance.Score}";

        if (comboText != null)
            comboText.text = GameManager.Instance.Combo > 1
                ? $"x{GameManager.Instance.Combo} COMBO" : "";

        if (livesText != null)
        {
            string hearts = "";
            for (int i = 0; i < GameManager.Instance.Lives; i++) hearts += "♥ ";
            livesText.text = hearts.TrimEnd();
        }
    }

    // ── Pausa ─────────────────────────────────────────────────────────────────

    private void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        pausePanel?.SetActive(true);
        if (musicSource != null && musicSource.isPlaying) musicSource.Pause();
    }

    private void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        pausePanel?.SetActive(false);
        if (musicSource != null && !musicSource.isPlaying) musicSource.UnPause();
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    // ── Tutorial hint ─────────────────────────────────────────────────────────

    public void ShowTutorialHint(PoseType pose)
    {
        if (tutorialHintText == null) return;

        // Teclas a mostrar
        string keyLabel = pose switch
        {
            PoseType.PoseA => "W",
            PoseType.PoseB => "A",
            PoseType.PoseC => "S",
            PoseType.PoseD => "D",
            PoseType.PoseAB => "W + A",
            PoseType.PoseAD => "W + D",
            PoseType.PoseBC => "A + S",
            PoseType.PoseCD => "S + D",
            _ => "?",
        };

        Color poseColor = pose switch
        {
            PoseType.PoseA => new Color(0.2f, 0.6f, 1f),
            PoseType.PoseB => new Color(1f, 0.4f, 0.2f),
            PoseType.PoseC => new Color(0.3f, 0.9f, 0.3f),
            PoseType.PoseD => new Color(0.9f, 0.2f, 0.8f),
            PoseType.PoseAB => new Color(0.6f, 0.3f, 1f),
            PoseType.PoseAD => new Color(1f, 0.9f, 0.1f),
            PoseType.PoseBC => new Color(0.1f, 0.9f, 0.9f),
            PoseType.PoseCD => new Color(1f, 0.5f, 0.7f),
            _ => Color.white,
        };

        tutorialHintText.text = $"Presioná <b><size=150%>{keyLabel}</size></b>";
        tutorialHintText.color = poseColor;
        tutorialHintText.gameObject.SetActive(true);

        if (tutorialHintBackground != null)
            tutorialHintBackground.SetActive(true);

        IsTutorialHintActive = true;
    }

    public void HideTutorialHint()
    {
        if (tutorialHintText != null)
            tutorialHintText.gameObject.SetActive(false);
        if (tutorialHintBackground != null)
            tutorialHintBackground.SetActive(false);

        IsTutorialHintActive = false;
    }

    // ── Feedback de hit/miss ──────────────────────────────────────────────────

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
        feedbackText.text = success
            ? HitMessages[Random.Range(0, HitMessages.Length)]
            : MissMessages[Random.Range(0, MissMessages.Length)];
        feedbackText.color = success
            ? new Color(0.2f, 1f, 0.4f)
            : new Color(1f, 0.25f, 0.25f);

        float elapsed = 0f;
        Vector3 startSc = Vector3.one * 1.3f;
        feedbackText.transform.localScale = startSc;

        while (elapsed < feedbackDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / feedbackDuration;
            feedbackText.transform.localScale = Vector3.Lerp(startSc, Vector3.one, t);
            Color c = feedbackText.color;
            feedbackText.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        feedbackText.gameObject.SetActive(false);
    }

    // ── Paneles de resultado ──────────────────────────────────────────────────

    private void ShowWinPanel(int score, int maxCombo)
    {
        winPanel?.SetActive(true);
        if (winScoreText != null) winScoreText.text = $"Puntuación: {score}";
        if (winComboText != null) winComboText.text = $"Combo máximo: {maxCombo}x";
    }

    private void ShowLosePanel()
    {
        losePanel?.SetActive(true);
        if (loseMissesText != null) loseMissesText.text = "Te quedaste sin vidas";
    }

    private void HideTutorialPanel()
    {
        tutorialPanel?.SetActive(false);
        Time.timeScale = 1f;
        _gameStarted = true;
        obstacleSpawner?.StartGame();
    }

    private void RetryLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}