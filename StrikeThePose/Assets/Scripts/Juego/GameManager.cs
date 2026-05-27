using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }
    public int Lives { get; private set; }

  
    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Normal;

    [Header("Puntuación")]
    [SerializeField] private int pointsPerHit = 100;
    [SerializeField] private int comboBonusEvery = 5;

    [Header("Vidas")]
    [SerializeField] private int maxLives = 5;

    [Header("Recuperación de vidas")]
    [Tooltip("Cantidad de aciertos consecutivos para recuperar una vida")]
    [SerializeField] private int hitsToRecoverLife = 3;

    public bool IsGameOver { get; private set; }
    public bool IsGameWon { get; private set; }

    // Contador de aciertos consecutivos para recuperar vida
    private int _consecutiveHits = 0;

    [System.Serializable] public class ResultEvent : UnityEvent<bool, PoseType> { }
    [System.Serializable] public class GameOverEvent : UnityEvent { }
    [System.Serializable] public class GameWonEvent : UnityEvent<int, int> { }

    // Inicializamos los eventos para evitar NullReferenceException
    public ResultEvent OnObstacleResultEvent = new ResultEvent();
    public GameOverEvent OnGameOverEvent = new GameOverEvent();
    public GameWonEvent OnGameWonEvent = new GameWonEvent();

    private void Start()
    {
        Lives = maxLives;
    }

    /// <summary>
    /// Configura las vidas y puntuaciones máximas según la dificultad elegida.
    /// </summary>
    public void SetDifficulty(Difficulty difficulty)
    {
        CurrentDifficulty = difficulty;
        switch (difficulty)
        {
            case Difficulty.Easy:
                maxLives = 7;
                pointsPerHit = 80;
                hitsToRecoverLife = 2;
                break;
            case Difficulty.Normal:
                maxLives = 5;
                pointsPerHit = 100;
                hitsToRecoverLife = 3;
                break;
            case Difficulty.Hard:
                maxLives = 3;
                pointsPerHit = 150;
                hitsToRecoverLife = 4;
                break;
        }
        Lives = maxLives;
        _consecutiveHits = 0;
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        IsGameOver = false;
        IsGameWon = false;
    }

    public void OnObstacleResult(bool success, PoseType poseRequired)
    {
        if (IsGameOver || IsGameWon) return;

        if (success)
        {
            Combo++;
            if (Combo > MaxCombo) MaxCombo = Combo;

            int bonus = (Combo > 0 && Combo % comboBonusEvery == 0) ? 2 : 1;
            Score += pointsPerHit * bonus;

            // Contar aciertos consecutivos
            _consecutiveHits++;

            // Recuperar vida cada X aciertos
            if (_consecutiveHits >= hitsToRecoverLife)
            {
                _consecutiveHits = 0;
                if (Lives < maxLives)
                {
                    Lives++; // Se corrige el bug de Lives = 10 para aumentar gradualmente
                }
            }
        }
        else
        {
            Combo = 0;
            _consecutiveHits = 0; // Resetear contador al fallar
            Lives--;

            if (CameraEffects.Instance != null)
                CameraEffects.Instance.PlayErrorFeedback();

            if (Lives <= 0)
            {
                IsGameOver = true;
                OnGameOverEvent?.Invoke();
                return;
            }
        }

        OnObstacleResultEvent?.Invoke(success, poseRequired);
    }

    public void OnSongFinished()
    {
        if (IsGameOver) return;
        IsGameWon = true;
        OnGameWonEvent?.Invoke(Score, MaxCombo);
    }

    public void ResetGame()
    {
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        Lives = maxLives;
        IsGameOver = false;
        IsGameWon = false;
        _consecutiveHits = 0;
    }
}