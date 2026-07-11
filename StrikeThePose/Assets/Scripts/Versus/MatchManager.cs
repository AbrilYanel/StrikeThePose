using UnityEngine;
using UnityEngine.Events;

public enum MatchOutcome
{
    Win,
    Lose,
    Draw
}

/// <summary>
/// Controla el inicio y el resultado global de la partida 1v1.
/// </summary>
public class MatchManager : MonoBehaviour
{
    [Header("Jugadores")]
    [SerializeField] private GameManager player1GameManager;
    [SerializeField] private GameManager player2GameManager;

    [Header("Interfaces")]
    [SerializeField] private UIManager player1UI;
    [SerializeField] private UIManager player2UI;

    [Header("Spawner maestro")]
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    [Header("Eventos opcionales")]
    public UnityEvent OnMatchStarted = new UnityEvent();
    public UnityEvent OnMatchFinished = new UnityEvent();

    public bool IsMatchStarted { get; private set; }
    public bool IsMatchFinished { get; private set; }

    private void OnEnable()
    {
        if (obstacleSpawner != null)
            obstacleSpawner.OnSongFinishedEvent.AddListener(FinishMatch);
    }

    private void OnDisable()
    {
        if (obstacleSpawner != null)
            obstacleSpawner.OnSongFinishedEvent.RemoveListener(FinishMatch);
    }

    public void StartEasy() => StartMatch(Difficulty.Easy);
    public void StartNormal() => StartMatch(Difficulty.Normal);
    public void StartHard() => StartMatch(Difficulty.Hard);

    public void StartMatch(Difficulty difficulty)
    {
        if (IsMatchStarted && !IsMatchFinished)
            return;

        if (!ValidateReferences())
            return;

        Time.timeScale = 1f;
        IsMatchStarted = true;
        IsMatchFinished = false;

        player1GameManager.SetDifficulty(difficulty);
        player2GameManager.SetDifficulty(difficulty);

        player1UI.PrepareForMatch();
        player2UI.PrepareForMatch();

        obstacleSpawner.SetDifficulty(difficulty);
        obstacleSpawner.StartGame();

        Debug.Log($"[MatchManager] Partida iniciada en dificultad {difficulty}.");
        OnMatchStarted?.Invoke();
    }

    private void FinishMatch()
    {
        if (!IsMatchStarted || IsMatchFinished)
            return;

        IsMatchFinished = true;

        player1GameManager.MarkMatchFinished();
        player2GameManager.MarkMatchFinished();

        int player1Score = player1GameManager.Score;
        int player2Score = player2GameManager.Score;

        if (player1Score > player2Score)
        {
            player1UI.ShowMatchResult(
                MatchOutcome.Win,
                player1Score,
                player2Score
            );

            player2UI.ShowMatchResult(
                MatchOutcome.Lose,
                player2Score,
                player1Score
            );
        }
        else if (player2Score > player1Score)
        {
            player1UI.ShowMatchResult(
                MatchOutcome.Lose,
                player1Score,
                player2Score
            );

            player2UI.ShowMatchResult(
                MatchOutcome.Win,
                player2Score,
                player1Score
            );
        }
        else
        {
            player1UI.ShowMatchResult(
                MatchOutcome.Draw,
                player1Score,
                player2Score
            );

            player2UI.ShowMatchResult(
                MatchOutcome.Draw,
                player2Score,
                player1Score
            );
        }

        Debug.Log(
            $"[MatchManager] Resultado final: P1 {player1Score} - P2 {player2Score}"
        );

        OnMatchFinished?.Invoke();
    }

    private bool ValidateReferences()
    {
        if (player1GameManager == null || player2GameManager == null ||
            player1UI == null || player2UI == null || obstacleSpawner == null)
        {
            Debug.LogError(
                "[MatchManager] Faltan referencias en el Inspector.",
                this
            );
            return false;
        }

        return true;
    }
}
