using UnityEngine;
using UnityEngine.Events;

public enum MatchMode
{
    SinglePlayer,
    Versus
}

public enum MatchOutcome
{
    Win,
    Lose,
    Draw
}

/// <summary>
/// Controla el flujo global tanto del modo single-player como del modo 1v1.
/// </summary>
public class MatchManager : MonoBehaviour
{
    [Header("Modo de juego")]
    [SerializeField] private MatchMode matchMode = MatchMode.Versus;

    [Header("Jugador 1 / Single Player")]
    [SerializeField] private GameManager player1GameManager;
    [SerializeField] private UIManager player1UI;

    [Header("Jugador 2 (sólo Versus)")]
    [SerializeField] private GameManager player2GameManager;
    [SerializeField] private UIManager player2UI;

    [Header("Spawner")]
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    [Header("Eventos opcionales")]
    public UnityEvent OnMatchStarted = new UnityEvent();
    public UnityEvent OnMatchFinished = new UnityEvent();

    public MatchMode CurrentMode => matchMode;
    public bool IsMatchStarted { get; private set; }
    public bool IsMatchFinished { get; private set; }

    private void OnEnable()
    {
        if (obstacleSpawner != null)
            obstacleSpawner.OnSongFinishedEvent.AddListener(HandleSongFinished);

        if (player1GameManager != null)
            player1GameManager.OnEliminatedEvent.AddListener(HandlePlayer1Eliminated);
    }

    private void OnDisable()
    {
        if (obstacleSpawner != null)
            obstacleSpawner.OnSongFinishedEvent.RemoveListener(HandleSongFinished);

        if (player1GameManager != null)
            player1GameManager.OnEliminatedEvent.RemoveListener(HandlePlayer1Eliminated);
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
        player1UI.PrepareForMatch();

        if (matchMode == MatchMode.Versus)
        {
            player2GameManager.SetDifficulty(difficulty);
            player2UI.PrepareForMatch();
        }

        obstacleSpawner.SetDifficulty(difficulty);
        obstacleSpawner.StartGame(matchMode);

        Debug.Log(
            $"[MatchManager] Modo {matchMode} iniciado en dificultad {difficulty}."
        );

        OnMatchStarted.Invoke();
    }

    private void HandleSongFinished()
    {
        if (!IsMatchStarted || IsMatchFinished)
            return;

        if (matchMode == MatchMode.SinglePlayer)
            FinishSinglePlayer(!player1GameManager.IsEliminated);
        else
            FinishVersus();
    }

    private void HandlePlayer1Eliminated()
    {
        if (matchMode != MatchMode.SinglePlayer ||
            !IsMatchStarted || IsMatchFinished)
        {
            return;
        }

        // En single-player se conserva el comportamiento original: perder todas
        // las vidas termina el nivel inmediatamente.
        obstacleSpawner.StopGame();
        FinishSinglePlayer(false);
    }

    private void FinishSinglePlayer(bool completedSong)
    {
        if (IsMatchFinished)
            return;

        IsMatchFinished = true;
        player1GameManager.MarkMatchFinished();

        player1UI.ShowSinglePlayerResult(
            completedSong,
            player1GameManager.Score,
            player1GameManager.MaxCombo
        );

        Debug.Log(
            completedSong
                ? $"[MatchManager] Nivel completado. Score: {player1GameManager.Score}"
                : $"[MatchManager] Game Over. Score: {player1GameManager.Score}"
        );

        OnMatchFinished.Invoke();
    }

    private void FinishVersus()
    {
        if (IsMatchFinished)
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

        OnMatchFinished.Invoke();
    }

    private bool ValidateReferences()
    {
        if (player1GameManager == null || player1UI == null ||
            obstacleSpawner == null)
        {
            Debug.LogError(
                "[MatchManager] Faltan referencias de Single Player/P1.",
                this
            );
            return false;
        }

        if (matchMode == MatchMode.Versus &&
            (player2GameManager == null || player2UI == null))
        {
            Debug.LogError(
                "[MatchManager] El modo Versus requiere GameManager y UI de P2.",
                this
            );
            return false;
        }

        return true;
    }
}
