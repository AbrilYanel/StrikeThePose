using UnityEngine;


public class SkipInstantaneo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameManager playerGameManager;
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private MatchManager matchManager;

    [Header("Beatmaps")]
    [SerializeField] private Beatmap defaultBeatMap;
    [SerializeField] private Beatmap easyBeatMap;
    [SerializeField] private Beatmap normalBeatMap;
    [SerializeField] private Beatmap hardBeatMap;

    [Header("Cálculo de notas hold")]
    [Tooltip("Debe coincidir con Tutorial Obstacle Count del spawner. Los holds del tutorial se convierten en notas normales.")]
    [Min(0)]
    [SerializeField] private int tutorialObstacleCount = 10;
    [Tooltip("Debe coincidir con el intervalo de ticks usado por script obstacle")]
    [Min(0.01f)]
    [SerializeField] private float holdTickInterval = 0.1f;
    [Min(0)]
    [SerializeField] private int pointsPerHoldTick = 5;

    [Header("Bonus opcional")]
    
    [Min(0)]
    [SerializeField] private int perfectBonusAreaPoints;

    [Header("Seguridad")]
    [Tooltip("Impide ejecutar el botón más de una vez durante la misma partida.")]
    [SerializeField] private bool useOnlyOnce = true;

    private bool _alreadyUsed;

    public void CompleteSongWithPerfectScore()
    {
        if (useOnlyOnce && _alreadyUsed)
            return;

        if (!ValidateReferences())
            return;

        if (!matchManager.IsMatchStarted || matchManager.IsMatchFinished)
        {
            Debug.LogWarning(
                "[InstantPerfectSongButton] La partida debe estar activa antes de usar el botón.",
                this
            );
            return;
        }

        Beatmap selectedBeatMap = GetBeatmapForCurrentDifficulty();

        if (selectedBeatMap == null)
        {
            Debug.LogError(
                "[InstantPerfectSongButton] No hay un Beatmap asignado para la dificultad actual.",
                this
            );
            return;
        }

        _alreadyUsed = true;

       
        playerGameManager.ResetGame();

        int holdBonusPoints = 0;
        int obstacleIndex = 0;

        foreach (BeatEvent beatEvent in selectedBeatMap.events)
        {
            if (beatEvent.isBonusAreaStart || beatEvent.isBonusAreaEnd)
                continue;

            playerGameManager.OnObstacleResult(
                true,
                beatEvent.requiredPose
            );

            bool isTutorialObstacle =
                obstacleIndex < tutorialObstacleCount;

            if (!isTutorialObstacle &&
                beatEvent.isHoldNote &&
                beatEvent.holdDuration > 0f)
            {
                int tickCount = Mathf.FloorToInt(
                    beatEvent.holdDuration / holdTickInterval
                );

                holdBonusPoints += tickCount * pointsPerHoldTick;
            }

            obstacleIndex++;
        }

        int additionalPoints =
            holdBonusPoints + perfectBonusAreaPoints;

        if (additionalPoints > 0)
            playerGameManager.AddBonusPoints(additionalPoints);

        // Detiene música/coroutines, limpia obstáculos y utiliza el mismo evento
        // que se ejecuta cuando la canción termina normalmente.
        obstacleSpawner.StopGame();
        obstacleSpawner.OnSongFinishedEvent.Invoke();

        Debug.Log(
            $"[InstantPerfectSongButton] Canción completada al 100%. " +
            $"Score final: {playerGameManager.Score} | " +
            $"Combo máximo: {playerGameManager.MaxCombo}"
        );
    }

    private Beatmap GetBeatmapForCurrentDifficulty()
    {
        switch (playerGameManager.CurrentDifficulty)
        {
            case Difficulty.Easy:
                return easyBeatMap != null
                    ? easyBeatMap
                    : defaultBeatMap;

            case Difficulty.Hard:
                return hardBeatMap != null
                    ? hardBeatMap
                    : defaultBeatMap;

            default:
                return normalBeatMap != null
                    ? normalBeatMap
                    : defaultBeatMap;
        }
    }

    private bool ValidateReferences()
    {
        if (playerGameManager == null ||
            obstacleSpawner == null ||
            matchManager == null)
        {
            Debug.LogError(
                "[InstantPerfectSongButton] Faltan referencias en el Inspector.",
                this
            );
            return false;
        }

        return true;
    }
}
