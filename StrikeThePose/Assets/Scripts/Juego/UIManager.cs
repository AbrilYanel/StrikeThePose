using System.Collections;
using System.Collections.Generic;
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
    [Tooltip("Color único utilizado por todas las hints, independientemente de la pose")]
    [SerializeField] private Color tutorialHintColor = Color.white;

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

    [Header("Sonidos de UI")]
    [Tooltip("AudioSource exclusivo para la interfaz. Si queda vacío, se crea uno automáticamente.")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip winPanelSound;
    [SerializeField] private AudioClip losePanelSound;
    [SerializeField] private AudioClip drawPanelSound;
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Sonidos de poses")]
    [SerializeField] private AudioClip correctPoseSound;
    [SerializeField] private AudioClip incorrectPoseSound;
    [SerializeField] private bool playPoseFeedbackSounds = true;
    [Range(0f, 1f)]
    [SerializeField] private float poseFeedbackVolume = 1f;

    [Header("Volumen y activación")]
    [Range(0f, 1f)]
    [SerializeField] private float uiSoundVolume = 1f;
    [Tooltip("Desactivar en la UI de P2 si no querés que Win y Lose suenen simultáneamente en 1v1 local.")]
    [SerializeField] private bool playResultSounds = true;
    [SerializeField] private bool playButtonSounds = true;
    [Tooltip("Tiempo máximo que se espera antes de cambiar de escena para que se alcance a oír el clic.")]
    [Range(0f, 0.5f)]
    [SerializeField] private float sceneChangeSoundDelay = 0.15f;
    [Tooltip("Botones que no sean hijos de este UIManager y también deban reproducir el clic.")]
    [SerializeField] private Button[] additionalSoundButtons;

    [Header("Referencias")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CameraEffects cameraEffects;
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private AudioSource musicSource;

    [Header("Flujo global")]
    [Tooltip("Activar sólo en la UI de P1. P1 manejará menú, inicio y pausa.")]
    [SerializeField] private bool controlsGameFlow = true;

    [Header("Estado eliminado")]
    [Tooltip("Indicador opcional. No debe bloquear la vista ni detener al jugador.")]
    [SerializeField] private GameObject eliminatedIndicator;
    [SerializeField] private TextMeshProUGUI eliminatedText;

    [Header("Panel de resultado genérico (opcional)")]
    [Tooltip("Sirve para Single Player y 1v1. Si no se asigna, se reutilizan los paneles de victoria/derrota existentes.")]
    [SerializeField] private GameObject matchResultPanel;
    [SerializeField] private TextMeshProUGUI matchResultTitleText;
    [SerializeField] private TextMeshProUGUI matchResultScoreText;
    [SerializeField] private Button matchResultRetryButton;

    private Coroutine _feedbackCoroutine;
    private Coroutine _bonusFeedbackCoroutine;
    private readonly List<Button> _registeredSoundButtons = new List<Button>();
    private bool _resultSoundPlayed;

    public bool IsTutorialHintActive { get; private set; } = false;

    private struct TutorialHintRequest
    {
        public int OwnerId;
        public PoseType Pose;

        public TutorialHintRequest(int ownerId, PoseType pose)
        {
            OwnerId = ownerId;
            Pose = pose;
        }
    }

    // Cola conserva exactamente el orden en que spawnearon los obstáculos.
    private readonly List<TutorialHintRequest> _tutorialHintQueue =
        new List<TutorialHintRequest>();

    private bool _hasActiveTutorialHint;
    private int _activeTutorialHintOwnerId;

    private bool _isPaused = false;
    private bool _gameStarted = false;

    private void Awake()
    {
        if (uiAudioSource == null)
        {
            uiAudioSource = gameObject.AddComponent<AudioSource>();
            uiAudioSource.playOnAwake = false;
            uiAudioSource.loop = false;
            uiAudioSource.spatialBlend = 0f;
            uiAudioSource.ignoreListenerPause = true;
        }
    }

    private void Start()
    {
        // Activación inicial de paneles de forma segura
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (matchResultPanel != null) matchResultPanel.SetActive(false);
        if (eliminatedIndicator != null) eliminatedIndicator.SetActive(false);

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

                if (matchManager != null)
                    matchManager.StartMatch(Difficulty.Normal);
                else
                    Debug.LogError("[UIManager] Falta asignar MatchManager.", this);
            }
        }
        else
        {
            // La UI secundaria funciona sólo como HUD y feedback.
            if (difficultyPanel != null) difficultyPanel.SetActive(false);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
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
        if (winRetryButton != null)
            winRetryButton.onClick.AddListener(RetryLevel);

        if (loseRetryButton != null)
            loseRetryButton.onClick.AddListener(RetryLevel);

        if (matchResultRetryButton != null)
            matchResultRetryButton.onClick.AddListener(RetryLevel);

        // Menú, inicio y pausa se registran únicamente en la UI principal (P1).
        if (controlsGameFlow)
        {
            if (tutorialStartButton != null)
                tutorialStartButton.onClick.AddListener(HideTutorialPanel);

            if (resumeButton != null)
                resumeButton.onClick.AddListener(ResumeGame);

            if (restartButton != null)
                restartButton.onClick.AddListener(RetryLevel);

            if (menuButton != null)
                menuButton.onClick.AddListener(GoToMainMenu);

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

        // Un jugador eliminado continúa jugando. Sólo mostramos un indicador.
        if (gameManager != null && gameManager.OnEliminatedEvent != null)
            gameManager.OnEliminatedEvent.AddListener(ShowEliminatedIndicator);

        RegisterButtonSounds();
    }

    private void Update()
    {
        if (gameManager == null) return;

        if (controlsGameFlow && _gameStarted && !gameManager.IsMatchFinished)
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



    private void SelectDifficulty(Difficulty difficulty)
    {
        Debug.Log($"[UIManager] Selección de dificultad: {difficulty}");

        if (!controlsGameFlow)
            return;

        if (difficultyPanel != null) difficultyPanel.SetActive(false);
        Time.timeScale = 1f;
        _gameStarted = true;

        if (matchManager != null)
            matchManager.StartMatch(difficulty);
        else
            Debug.LogError("[UIManager] Falta asignar MatchManager.", this);
    }



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



    private void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        if (musicSource != null && musicSource.isPlaying) musicSource.Pause();
    }

    private void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (musicSource != null && !musicSource.isPlaying) musicSource.UnPause();
    }

    private void GoToMainMenu()
    {
        StartCoroutine(LoadSceneAfterClickSound(mainMenuSceneName));
    }



    public void RegisterTutorialHint(int ownerId, PoseType pose)
    {
        if (tutorialHintText == null || HasTutorialHintOwner(ownerId))
            return;

        _tutorialHintQueue.Add(new TutorialHintRequest(ownerId, pose));
        TryShowNextTutorialHint();
    }


    public void ReleaseTutorialHint(int ownerId)
    {
        if (_hasActiveTutorialHint &&
            _activeTutorialHintOwnerId == ownerId)
        {
            _hasActiveTutorialHint = false;
            _activeTutorialHintOwnerId = 0;
            HideTutorialHintVisual();
            TryShowNextTutorialHint();
            return;
        }

        // El obstáculo todavía estaba esperando en la cola. Lo quitamos para
        // impedir que aparezca después de haber sido destruido.
        for (int i = _tutorialHintQueue.Count - 1; i >= 0; i--)
        {
            if (_tutorialHintQueue[i].OwnerId == ownerId)
                _tutorialHintQueue.RemoveAt(i);
        }
    }

    private bool HasTutorialHintOwner(int ownerId)
    {
        if (_hasActiveTutorialHint &&
            _activeTutorialHintOwnerId == ownerId)
        {
            return true;
        }

        for (int i = 0; i < _tutorialHintQueue.Count; i++)
        {
            if (_tutorialHintQueue[i].OwnerId == ownerId)
                return true;
        }

        return false;
    }

    private void TryShowNextTutorialHint()
    {
        if (_hasActiveTutorialHint || _tutorialHintQueue.Count == 0)
            return;

        TutorialHintRequest nextHint = _tutorialHintQueue[0];
        _tutorialHintQueue.RemoveAt(0);

        _hasActiveTutorialHint = true;
        _activeTutorialHintOwnerId = nextHint.OwnerId;
        ShowTutorialHintVisual(nextHint.Pose);
    }

    private void ShowTutorialHintVisual(PoseType pose)
    {
        if (tutorialHintText == null)
            return;

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

        tutorialHintText.text =
            $"Presiona <b><size=150%>{keyLabel}</size></b>";

        tutorialHintText.color = tutorialHintColor;
        tutorialHintText.gameObject.SetActive(true);

        if (tutorialHintBackground != null)
            tutorialHintBackground.SetActive(true);

        IsTutorialHintActive = true;
    }

    private void HideTutorialHintVisual()
    {
        if (tutorialHintText != null)
            tutorialHintText.gameObject.SetActive(false);

        if (tutorialHintBackground != null)
            tutorialHintBackground.SetActive(false);

        IsTutorialHintActive = false;
    }


    public void ClearTutorialHints()
    {
        _tutorialHintQueue.Clear();
        _hasActiveTutorialHint = false;
        _activeTutorialHintOwnerId = 0;
        HideTutorialHintVisual();
    }

    // Compatibilidad con llamadas anteriores del proyecto.
    public void ShowTutorialHint(PoseType pose)
    {
        ClearTutorialHints();
        _hasActiveTutorialHint = true;
        _activeTutorialHintOwnerId = int.MinValue;
        ShowTutorialHintVisual(pose);
    }

    public void HideTutorialHint()
    {
        ClearTutorialHints();
    }



    public void ShowHitFeedback(bool success)
    {
        if (playPoseFeedbackSounds)
        {
            AudioClip feedbackClip = success
                ? correctPoseSound
                : incorrectPoseSound;

            PlayUISound(feedbackClip, poseFeedbackVolume);
        }

        // El sonido funciona aunque no haya una imagen de feedback asignada.
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



    public void PrepareForMatch()
    {
        _gameStarted = true;
        _isPaused = false;
        _resultSoundPlayed = false;

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (matchResultPanel != null) matchResultPanel.SetActive(false);
        if (eliminatedIndicator != null) eliminatedIndicator.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        ShowBonusAreaUI(false);
        HideTutorialHint();
    }

    public void ShowSinglePlayerResult(
        bool completedSong,
        int score,
        int maxCombo)
    {
        _gameStarted = false;

        if (eliminatedIndicator != null)
            eliminatedIndicator.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        string title = completedSong ? "¡NIVEL COMPLETADO!" : "GAME OVER";
        PlayResultSound(completedSong ? MatchOutcome.Win : MatchOutcome.Lose);

        if (matchResultPanel != null)
        {
            matchResultPanel.SetActive(true);

            if (matchResultTitleText != null)
                matchResultTitleText.text = title;

            if (matchResultScoreText != null)
            {
                matchResultScoreText.text =
                    $"Puntaje: {score:D7}\nCombo máximo: {maxCombo}x";
            }

            return;
        }

        if (completedSong)
        {
            if (winPanel != null)
                winPanel.SetActive(true);

            if (winScoreText != null)
                winScoreText.text = score.ToString("D7");

            if (winComboText != null)
                winComboText.text = $"Combo máximo: {maxCombo}x";
        }
        else
        {
            if (losePanel != null)
                losePanel.SetActive(true);

            if (loseMissesText != null)
                loseMissesText.text = $"Perdiste\n\nPuntaje: \n{score:D7}";
        }
    }

    public void ShowMatchResult(
        MatchOutcome outcome,
        int ownScore,
        int rivalScore)
    {
        _gameStarted = false;
        if (eliminatedIndicator != null) eliminatedIndicator.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        string title = outcome switch
        {
            MatchOutcome.Win => "GANADOR",
            MatchOutcome.Lose => "PERDISTE",
            _ => "EMPATE"
        };

        PlayResultSound(outcome);

        if (matchResultPanel != null)
        {
            matchResultPanel.SetActive(true);

            if (matchResultTitleText != null)
                matchResultTitleText.text = title;

            if (matchResultScoreText != null)
            {
                matchResultScoreText.text =
                    $"Tu puntaje:\n {ownScore:D7}\n" +
                    $"Rival:\n {rivalScore:D7}";
            }

            return;
        }

        // Fallback: reutiliza los paneles que ya existían en el modo individual.
        if (outcome == MatchOutcome.Lose)
        {
            if (losePanel != null) losePanel.SetActive(true);

            if (loseMissesText != null)
            {
                loseMissesText.text =
                    $"{title}\nTu puntaje: \n{ownScore:D7}\nRival: \n{rivalScore:D7}";
            }
        }
        else
        {
            if (winPanel != null) winPanel.SetActive(true);

            if (winScoreText != null)
                winScoreText.text = ownScore.ToString("D7");

            if (winComboText != null)
            {
                winComboText.text =
                    $"{title} - Rival: {rivalScore:D7}";
            }
        }
    }

    private void RegisterButtonSounds()
    {
        _registeredSoundButtons.Clear();

        if (!playButtonSounds)
            return;

        Button[] childButtons = GetComponentsInChildren<Button>(true);

        foreach (Button button in childButtons)
            RegisterButtonSound(button);

        if (additionalSoundButtons == null)
            return;

        foreach (Button button in additionalSoundButtons)
            RegisterButtonSound(button);
    }

    private void RegisterButtonSound(Button button)
    {
        if (button == null || _registeredSoundButtons.Contains(button))
            return;

        // Evita registrar el mismo callback más de una vez si se refresca la UI.
        button.onClick.RemoveListener(PlayButtonClickSound);
        button.onClick.AddListener(PlayButtonClickSound);
        _registeredSoundButtons.Add(button);
    }

    private void PlayButtonClickSound()
    {
        if (!playButtonSounds)
            return;

        PlayUISound(buttonClickSound);
    }

    private void PlayResultSound(MatchOutcome outcome)
    {
        if (!playResultSounds || _resultSoundPlayed)
            return;

        AudioClip clip = outcome switch
        {
            MatchOutcome.Win => winPanelSound,
            MatchOutcome.Lose => losePanelSound,
            _ => drawPanelSound
        };

        if (clip == null)
            return;

        _resultSoundPlayed = true;
        PlayUISound(clip);
    }

    private void PlayUISound(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null || uiAudioSource == null)
            return;

        float finalVolume = Mathf.Clamp01(uiSoundVolume * volumeMultiplier);
        uiAudioSource.PlayOneShot(clip, finalVolume);
    }

    private void ShowEliminatedIndicator()
    {
        // En Single Player la eliminación termina el nivel inmediatamente y se
        // muestra el panel de derrota. Este indicador sólo corresponde al 1v1.
        if (matchManager != null &&
            matchManager.CurrentMode == MatchMode.SinglePlayer)
        {
            return;
        }

        if (eliminatedIndicator != null)
            eliminatedIndicator.SetActive(true);

        if (eliminatedText != null)
            eliminatedText.text = "ELIMINADO - PODES SEGUIR JUGANDO";
    }

    private void OnDestroy()
    {
        if (gameManager != null && gameManager.OnEliminatedEvent != null)
            gameManager.OnEliminatedEvent.RemoveListener(ShowEliminatedIndicator);

        foreach (Button button in _registeredSoundButtons)
        {
            if (button != null)
                button.onClick.RemoveListener(PlayButtonClickSound);
        }

        _registeredSoundButtons.Clear();
    }

    private void HideTutorialPanel()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
        _gameStarted = true;

        if (matchManager != null)
            matchManager.StartMatch(Difficulty.Normal);
        else
            Debug.LogError("[UIManager] Falta asignar MatchManager.", this);
    }

    private void RetryLevel()
    {
        StartCoroutine(ReloadSceneAfterClickSound());
    }

    private IEnumerator LoadSceneAfterClickSound(string sceneName)
    {
        yield return WaitForSceneChangeSound();
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ReloadSceneAfterClickSound()
    {
        int buildIndex =
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        yield return WaitForSceneChangeSound();
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
    }

    private IEnumerator WaitForSceneChangeSound()
    {
        if (!playButtonSounds || buttonClickSound == null ||
            sceneChangeSoundDelay <= 0f)
        {
            yield break;
        }

        float delay = Mathf.Min(sceneChangeSoundDelay, buttonClickSound.length);

        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);
    }
}
