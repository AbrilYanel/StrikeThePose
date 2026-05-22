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

    [Header("Puntuación")]
    [SerializeField] private int pointsPerHit = 100;
    [SerializeField] private int comboBonusEvery = 5;

    [Header("Vidas")]
    [SerializeField] private int maxLives = 5;

    public bool IsGameOver { get; private set; }
    public bool IsGameWon { get; private set; }

    [System.Serializable] public class ResultEvent : UnityEvent<bool, PoseType> { }
    [System.Serializable] public class GameOverEvent : UnityEvent { }
    [System.Serializable] public class GameWonEvent : UnityEvent<int, int> { }

    public ResultEvent OnObstacleResultEvent;
    public GameOverEvent OnGameOverEvent;
    public GameWonEvent OnGameWonEvent;

    private void Start()
    {
        Lives = maxLives;
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
        }
        else
        {
            Combo = 0;
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
    }
}