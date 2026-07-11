using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI livesText;

    [Header("Feedback de acierto/fallo")]
    [Tooltip("Imagen donde se muestra el feedback (reemplaza al texto)")]
    [SerializeField] private Image feedbackImage;
    [SerializeField] private float feedbackDuration = 0.7f;
    [Header("Sprites de acierto")]
    [Tooltip("Imágenes que aparecen al ACERTAR (se elige una aleatoria)")]
    [SerializeField] private Sprite[] hitSprites;
    [Header("Sprites de fallo")]
    [Tooltip("Imágenes que aparecen al FALLAR (se elige una aleatoria)")]
    [SerializeField] private Sprite[] missSprites;
    [Header("Colores de feedback (tintan la imagen)")]
    [SerializeField] private Color hitTint = new Color(0.2f, 1f, 0.4f);
    [SerializeField] private Color missTint = new Color(1f, 0.25f, 0.25f);
    [Tooltip("Multiplicador de tamaño para la imagen de feedback (1 = tamaño del Rect Transform)")]
    [SerializeField] private float feedbackBaseScale = 2f;

    [Header("Panel de victoria")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winScoreText;
    [SerializeField] private TextMeshProUGUI winComboText;
    [SerializeField] private Button winRetryButton;

    [Header("Panel de derrota")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TextMeshProUGUI loseMissesText;
    [SerializeField] private Button loseRetryButton;

    [Header("Panel de Selección de Dificultad")]
    [Tooltip("Asigna aquí el panel que contiene los botones de selección de dificultad")]
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;

    [Header("Panel de tutorial (intro)")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Button tutorialStartButton;

    [Header("Tutorial en gameplay")]
    [Tooltip("Texto grande que muestra la tecla/pose del próximo obstáculo")]
    [SerializeField] private TextMeshProUGUI tutorialHintText;
    [Tooltip("Fondo opcional detrás del texto de tutorial")]
    [SerializeField] private GameObject tutorialHintBackground;

    [Header("Área Bonus / Frenesí")]
    [Tooltip("Cartel o banner en pantalla que se activa en el Área Bonus (ej. ¡FRENESÍ DE TECLAS!)")]
    [SerializeField] private GameObject bonusAreaBanner;
    [Tooltip("Texto opcional para hacer aparecer el flotante '+20 BONUS!' en pantalla")]
    [SerializeField] private TextMeshProUGUI bonusPointsFeedbackText;

    [Header("Panel de pausa")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Referencias")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CameraEffects cameraEffects;
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private AudioSource musicSource;

    [Header("Flujo temporal para prueba del Paso 3")]
    [Tooltip("Activar sólo en la UI de P1. P1 manejará menú, inicio y pausa para ambas pistas.")]
    [SerializeField] private bool controlsGameFlow = true;
    [Tooltip("GameManager de P2. Se usa temporalmente para aplicar la misma dificultad.")]
    [SerializeField] private GameManager secondaryGameManager;
    [Tooltip("Spawner de P2. Se inicia junto al spawner principal.")]
    [SerializeField] private ObstacleSpawner secondaryObstacleSpawner;

    private Coroutine _feedbackCoroutine;
    private Coroutine _bonusFeedbackCoroutine;

    public bool IsTutorialHintActive { get; private set; } = false;

    private bool _isPaused = false;
    private bool _gameStarted = false;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Start()
    {
        // Activación inicial de paneles de forma segura
        winPanel?.SetActive(false);
        losePanel?.SetActive(false);
        pausePanel?.SetActive(false);
        tutorialPanel?.SetActive(false);

        if (bonusAreaBanner != null) bonusAreaBanner.SetActive(false);
        if (bonusPointsFeedbackText != null) bonusPointsFeedbackText.gameObject.SetActive(false);

        if (controlsGameFlow)
        {
            if (difficultyPanel != null)
            {
                Debug.Log("[UIManager] Activando panel de selección de dificultad.");
                difficultyPanel.SetActive(true);
                Time.timeScale = 0f;
            }
            else if (tutorialPanel != null)
            {
                Debug.Log("[UIManager] No hay panel de dificultad. Activando panel de tutorial.");
                tutorialPanel.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                Debug.Log("[UIManager] No se asignó ningún panel inicial. El juego inicia de inmediato.");
                Time.timeScale = 1f;
                _gameStarted = true;
                obstacleSpawner?.StartGame();
                secondaryObstacleSpawner?.StartGame();
            }
        }
        else
        {
            // La UI secundaria funciona sólo como HUD y feedback.
            difficultyPanel?.SetActive(false);
            tutorialPanel?.SetActive(false);
            pausePanel?.SetActive(false);
            _gameStarted = true;
        }

        // Ocultar elementos de gameplay
        if (feedbackImage != null)
            feedbackImage.gameObject.SetActive(false);
        if (tutorialHintText != null)
            tutorialHintText.gameObject.SetActive(false);
        if (tutorialHintBackground != null)
            tutorialHintBackground.SetActive(false);

        // Los paneles de resultado siguen siendo individuales.
        winRetryButton?.onClick.AddListener(RetryLevel);
        loseRetryButton?.onClick.AddListener(RetryLevel);

        // Menú, inicio y pausa se registran únicamente en la UI principal (P1).
        if (controlsGameFlow)
        {
            tutorialStartButton?.onClick.AddListener(HideTutorialPanel);
            resumeButton?.onClick.AddListener(ResumeGame);
            restartButton?.onClick.AddListener(RetryLevel);
            menuButton?.onClick.AddListener(GoToMainMenu);

            if (easyButton != null)
            {
                easyButton.onClick.RemoveAllListeners();
                easyButton.onClick.AddListener(() => {
                    Debug.Log("[UIManager] Botón FÁCIL clickeado.");
                    SelectDifficulty(Difficulty.Easy);
                });
            }
            if (normalButton != null)
            {
                normalButton.onClick.RemoveAllListeners();
                normalButton.onClick.AddListener(() => {
                    Debug.Log("[UIManager] Botón NORMAL clickeado.");
                    SelectDifficulty(Difficulty.Normal);
                });
            }
            if (hardButton != null)
            {
                hardButton.onClick.RemoveAllListeners();
                hardButton.onClick.AddListener(() => {
                    Debug.Log("[UIManager] Botón DIFÍCIL clickeado.");
                    SelectDifficulty(Difficulty.Hard);
                });
            }
        }

        // Registro de eventos de GameManager
        if (gameManager != null)
        {
            if (gameManager.OnGameOverEvent != null)
                gameManager.OnGameOverEvent.AddListener(ShowLosePanel);
            if (gameManager.OnGameWonEvent != null)
                gameManager.OnGameWonEvent.AddListener(ShowWinPanel);
        }
    }

    private void Update()
    {
        if (gameManager == null) return;

        if (controlsGameFlow && _gameStarted && !gameManager.IsGameOver && !gameManager.IsGameWon)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (_isPaused) ResumeGame(); else PauseGame();
            }
        }

        if (_isPaused) return;

        if (scoreText != null)
            scoreText.text = gameManager.Score.ToString("D7");

        if (comboText != null)
            comboText.text = gameManager.Combo > 1
                ? $"x{gameManager.Combo} COMBO" : "";

        if (livesText != null)
        {
            string hearts = "";
            for (int i = 0; i < gameManager.Lives; i++) hearts += "♥ ";
            livesText.text = hearts.TrimEnd();
        }
    }

    // ── Selección de Dificultad ───────────────────────────────────────────────

    private void SelectDifficulty(Difficulty difficulty)
    {
        Debug.Log($"[UIManager] Procesando selección de dificultad: {difficulty}");

        if (gameManager != null)
        {
            gameManager.SetDifficulty(difficulty);
        }

        if (secondaryGameManager != null)
        {
            secondaryGameManager.SetDifficulty(difficulty);
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.SetDifficulty(difficulty);
        }

        if (secondaryObstacleSpawner != null)
        {
            secondaryObstacleSpawner.SetDifficulty(difficulty);
        }

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        _gameStarted = true;

        if (obstacleSpawner != null)
        {
            Debug.Log("[UIManager] Iniciando Spawner de P1...");
            obstacleSpawner.StartGame();
        }

        if (secondaryObstacleSpawner != null)
        {
            Debug.Log("[UIManager] Iniciando Spawner de P2...");
            secondaryObstacleSpawner.StartGame();
        }
    }

    // ── Área Bonus ────────────────────────────────────────────────────────────

    public void ShowBonusAreaUI(bool active)
    {
        if (bonusAreaBanner != null)
        {
            bonusAreaBanner.SetActive(active);
        }
    }

    public void ShowBonusPointsFeedback(int points)
    {
        if (bonusPointsFeedbackText == null) return;

        if (_bonusFeedbackCoroutine != null)
            StopCoroutine(_bonusFeedbackCoroutine);

        _bonusFeedbackCoroutine = StartCoroutine(BonusPointsFeedbackRoutine(points));
    }

    private IEnumerator BonusPointsFeedbackRoutine(int points)
    {
        bonusPointsFeedbackText.gameObject.SetActive(true);
        bonusPointsFeedbackText.text = $"+{points} BONUS!";
        bonusPointsFeedbackText.color = new Color(1f, 0.85f, 0f); // Amarillo dorado brillante

        float elapsed = 0f;
        float duration = 0.3f;
        Vector3 startScale = Vector3.one * 1.5f;
        bonusPointsFeedbackText.transform.localScale = startScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            bonusPointsFeedbackText.transform.localScale = Vector3.Lerp(startScale, Vector3.one, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        bonusPointsFeedbackText.gameObject.SetActive(false);
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

    // ── Feedback de hit/miss (IMAGEN) ─────────────────────────────────────────

    public void ShowHitFeedback(bool success)
    {
        if (feedbackImage == null) return;

        // Elegir sprite aleatorio según resultado
        Sprite[] pool = success ? hitSprites : missSprites;
        if (pool == null || pool.Length == 0)
        {
            Debug.LogWarning($"[UIManager] No hay sprites de feedback asignados para {(success ? "ACIERTO" : "FALLO")}.");
            return;
        }

        Sprite chosen = pool[Random.Range(0, pool.Length)];
        feedbackImage.sprite = chosen;

        // Tintar la imagen
        feedbackImage.color = success ? hitTint : missTint;

        if (_feedbackCoroutine != null)
            StopCoroutine(_feedbackCoroutine);

        _feedbackCoroutine = StartCoroutine(FeedbackRoutine());

        if (success && cameraEffects != null)
            cameraEffects.PlaySuccessFeedback();
    }

    private IEnumerator FeedbackRoutine()
    {
        feedbackImage.gameObject.SetActive(true);

        // Asegurar alpha completo al inicio
        Color baseColor = feedbackImage.color;
        baseColor.a = 1f;
        feedbackImage.color = baseColor;

        float elapsed = 0f;
        Vector3 startSc = Vector3.one * feedbackBaseScale * 1.3f;
        Vector3 endSc = Vector3.one * feedbackBaseScale;
        feedbackImage.transform.localScale = startSc;

        while (elapsed < feedbackDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / feedbackDuration;

            // Escala: de 1.3×base → 1.0×base
            feedbackImage.transform.localScale = Vector3.Lerp(startSc, endSc, t);

            // Fade out: alpha de 1 → 0
            Color c = feedbackImage.color;
            feedbackImage.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t));

            yield return null;
        }

        feedbackImage.gameObject.SetActive(false);
    }

    // ── Paneles de resultado ──────────────────────────────────────────────────

    private void ShowWinPanel(int score, int maxCombo)
    {
        winPanel?.SetActive(true);
        if (winScoreText != null) winScoreText.text = score.ToString("D7");
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
        secondaryObstacleSpawner?.StartGame();
    }

    private void RetryLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
