using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Estado y puntuación de un único jugador.
/// En 1v1 debe existir una instancia separada para P1 y otra para P2.
/// </summary>
public class GameManager : MonoBehaviour
{
    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }
    public int Lives { get; private set; }

    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Normal;
    public bool IsInBonusArea { get; private set; }
    public bool IsEliminated { get; private set; }
    public bool IsMatchFinished { get; private set; }

    [Header("Referencias de este jugador")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CameraEffects cameraEffects;

    [Header("Puntuación")]
    [SerializeField] private int pointsPerHit = 100;
    [SerializeField] private int comboBonusEvery = 5;

    [Header("Vidas")]
    [SerializeField] private int maxLives = 5;

    [Header("Recuperación de vidas")]
    [Tooltip("Cantidad de aciertos consecutivos para recuperar una vida")]
    [SerializeField] private int hitsToRecoverLife = 3;

    private int _consecutiveHits;

    [System.Serializable]
    public class ResultEvent : UnityEvent<bool, PoseType> { }

    [System.Serializable]
    public class EliminatedEvent : UnityEvent { }

    public ResultEvent OnObstacleResultEvent = new ResultEvent();
    public EliminatedEvent OnEliminatedEvent = new EliminatedEvent();

    private void Awake()
    {
        ResetGame();
    }

    /// <summary>
    /// Configura vidas y puntuación según la dificultad y reinicia al jugador.
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

        ResetGame();
    }

    public void SetBonusArea(bool active)
    {
        IsInBonusArea = active;

        if (uiManager != null)
            uiManager.ShowBonusAreaUI(active);
    }

    /// <summary>
    /// Suma puntos de bonus o ticks de notas sostenidas.
    /// Un jugador eliminado no puede sumar más puntos.
    /// </summary>
    public void AddBonusPoints(int points)
    {
        if (IsEliminated || IsMatchFinished)
            return;

        Score += points;

        if (uiManager != null)
            uiManager.ShowBonusPointsFeedback(points);
    }

    public void OnObstacleResult(bool success, PoseType poseRequired)
    {
        if (IsMatchFinished)
            return;

        // El jugador eliminado continúa recibiendo feedback, pero su estado
        // y su puntuación quedan congelados.
        if (IsEliminated)
        {
            if (!success && cameraEffects != null)
                cameraEffects.PlayErrorFeedback();

            OnObstacleResultEvent?.Invoke(success, poseRequired);
            return;
        }

        if (success)
        {
            Combo++;

            if (Combo > MaxCombo)
                MaxCombo = Combo;

            int multiplier =
                Combo > 0 && Combo % comboBonusEvery == 0 ? 2 : 1;

            Score += pointsPerHit * multiplier;
            _consecutiveHits++;

            if (_consecutiveHits >= hitsToRecoverLife)
            {
                _consecutiveHits = 0;

                if (Lives < maxLives)
                    Lives++;
            }
        }
        else
        {
            Combo = 0;
            _consecutiveHits = 0;
            Lives = Mathf.Max(0, Lives - 1);

            if (cameraEffects != null)
                cameraEffects.PlayErrorFeedback();

            if (Lives == 0)
            {
                IsEliminated = true;
                OnEliminatedEvent?.Invoke();
            }
        }

        OnObstacleResultEvent?.Invoke(success, poseRequired);
    }

    public void MarkMatchFinished()
    {
        IsMatchFinished = true;
        IsInBonusArea = false;

        if (uiManager != null)
            uiManager.ShowBonusAreaUI(false);
    }

    public void ResetGame()
    {
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        Lives = maxLives;
        _consecutiveHits = 0;
        IsInBonusArea = false;
        IsEliminated = false;
        IsMatchFinished = false;
    }
}
