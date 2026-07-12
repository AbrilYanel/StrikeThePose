using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Datos rítmicos (Default / Normal)")]
    [SerializeField] private Beatmap beatMap;

    [Header("Dificultades (Opcionales)")]
    [SerializeField] private Beatmap easyBeatMap;
    [SerializeField] private Beatmap normalBeatMap;
    [SerializeField] private Beatmap hardBeatMap;

    [Header("Prefab")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Jugador 1")]
    [SerializeField] private PoseController player1;
    [SerializeField] private GameManager gameManager1;
    [SerializeField] private UIManager uiManager1;
    [SerializeField] private Transform obstacleParent1;
    [SerializeField] private string player1LayerName = "Track_P1";

    [Header("Jugador 2")]
    [SerializeField] private PoseController player2;
    [SerializeField] private GameManager gameManager2;
    [SerializeField] private UIManager uiManager2;
    [SerializeField] private Transform obstacleParent2;
    [SerializeField] private string player2LayerName = "Track_P2";

    [Header("Música compartida")]
    [SerializeField] private AudioSource musicSource;

    [Header("Posición de spawn local")]
    [SerializeField] private float spawnZ = -20f;

    [Header("Límites del hueco aleatorio")]
    [SerializeField] private float holeXMin = -4f;
    [SerializeField] private float holeXMax = 4f;

    [Header("Velocidad del obstáculo por dificultad")]
    [SerializeField] private float easySpeed = 6f;
    [SerializeField] private float normalSpeed = 8f;
    [SerializeField] private float hardSpeed = 11f;

    [Header("Ventanas de juicio por dificultad")]
    [SerializeField] private float easyEarlyWindow = 0.6f;
    [SerializeField] private float easyLateWindow = 0.45f;
    [SerializeField] private float normalEarlyWindow = 0.4f;
    [SerializeField] private float normalLateWindow = 0.3f;
    [SerializeField] private float hardEarlyWindow = 0.28f;
    [SerializeField] private float hardLateWindow = 0.21f;

    [Header("Obstáculos fake (sólo difícil)")]
    [Range(0f, 1f)]
    [SerializeField] private float fakeObstacleChance = 0.25f;

    [Header("Tutorial")]
    [SerializeField] private int tutorialObstacleCount = 10;

    [Header("Eventos")]
    public UnityEvent OnSongFinishedEvent = new UnityEvent();

    private float obstacleSpeed = 8f;
    private float _currentEarlyWindow = 0.4f;
    private float _currentLateWindow = 0.3f;
    private bool _isHardDifficulty;

    private readonly List<BeatEvent> _obstacleEvents = new List<BeatEvent>();
    private readonly List<BeatEvent> _bonusEvents = new List<BeatEvent>();

    private int _nextObstacleIndex;
    private int _nextBonusIndex;
    private int _spawnedCount;
    private int _player1Layer = -1;
    private int _player2Layer = -1;

    private bool _running;
    private bool _configurationValid;
    private MatchMode _activeMode = MatchMode.Versus;
    private float _gameStartRealTime = -1f;

    private float TravelTime => Mathf.Abs(spawnZ) / obstacleSpeed;

    private void Awake()
    {
        _player1Layer = LayerMask.NameToLayer(player1LayerName);
        _player2Layer = LayerMask.NameToLayer(player2LayerName);

        _configurationValid = ValidateCommonConfiguration();

        if (!_configurationValid)
            return;

        if (normalBeatMap == null)
            normalBeatMap = beatMap;

        if (easyBeatMap == null)
            easyBeatMap = beatMap;

        if (hardBeatMap == null)
            hardBeatMap = beatMap;

        InitializeBeatmap(normalBeatMap);

        Debug.Log(
            $"[MasterSpawner] Configurado. TravelTime: {TravelTime:F2}s | " +
            $"P1 Layer: {player1LayerName} | P2 Layer: {player2LayerName}"
        );
    }

    private bool ValidateCommonConfiguration()
    {
        bool valid = true;

        if (beatMap == null && easyBeatMap == null &&
            normalBeatMap == null && hardBeatMap == null)
        {
            Debug.LogError("[MasterSpawner] No se asignó ningún Beatmap.", this);
            valid = false;
        }

        if (obstaclePrefab == null)
        {
            Debug.LogError("[MasterSpawner] Falta el prefab de obstáculo.", this);
            valid = false;
        }

        if (player1 == null || gameManager1 == null || uiManager1 == null ||
            obstacleParent1 == null)
        {
            Debug.LogError(
                "[MasterSpawner] Faltan referencias de Single Player/P1.",
                this
            );
            valid = false;
        }

        if (_player1Layer < 0)
        {
            Debug.LogError(
                $"[MasterSpawner] No existe la layer '{player1LayerName}'.",
                this
            );
            valid = false;
        }

        return valid;
    }

    private bool ValidateModeConfiguration(MatchMode mode)
    {
        if (mode == MatchMode.SinglePlayer)
            return true;

        if (player2 == null || gameManager2 == null || uiManager2 == null ||
            obstacleParent2 == null)
        {
            Debug.LogError(
                "[MasterSpawner] El modo Versus requiere todas las referencias de P2.",
                this
            );
            return false;
        }

        if (_player2Layer < 0)
        {
            Debug.LogError(
                $"[MasterSpawner] No existe la layer '{player2LayerName}'.",
                this
            );
            return false;
        }

        return true;
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        if (_running)
        {
            Debug.LogWarning(
                "[MasterSpawner] No se puede cambiar la dificultad durante la partida.",
                this
            );
            return;
        }

        Beatmap chosenMap = null;
        _isHardDifficulty = difficulty == Difficulty.Hard;

        switch (difficulty)
        {
            case Difficulty.Easy:
                chosenMap = easyBeatMap != null ? easyBeatMap : beatMap;
                obstacleSpeed = easySpeed;
                _currentEarlyWindow = easyEarlyWindow;
                _currentLateWindow = easyLateWindow;
                break;

            case Difficulty.Normal:
                chosenMap = normalBeatMap != null ? normalBeatMap : beatMap;
                obstacleSpeed = normalSpeed;
                _currentEarlyWindow = normalEarlyWindow;
                _currentLateWindow = normalLateWindow;
                break;

            case Difficulty.Hard:
                chosenMap = hardBeatMap != null ? hardBeatMap : beatMap;
                obstacleSpeed = hardSpeed;
                _currentEarlyWindow = hardEarlyWindow;
                _currentLateWindow = hardLateWindow;
                break;
        }

        if (chosenMap != null)
            InitializeBeatmap(chosenMap);

        Debug.Log(
            $"[MasterSpawner] Dificultad {difficulty}. " +
            $"Velocidad: {obstacleSpeed}. " +
            $"Early: {_currentEarlyWindow}s / Late: {_currentLateWindow}s"
        );
    }

    private void InitializeBeatmap(Beatmap targetBeatmap)
    {
        if (targetBeatmap == null)
            return;

        beatMap = targetBeatmap;
        _obstacleEvents.Clear();
        _bonusEvents.Clear();

        foreach (BeatEvent beatEvent in beatMap.events)
        {
            if (beatEvent.isBonusAreaStart || beatEvent.isBonusAreaEnd)
                _bonusEvents.Add(beatEvent);
            else
                _obstacleEvents.Add(beatEvent);
        }

        _obstacleEvents.Sort((a, b) => a.beat.CompareTo(b.beat));
        _bonusEvents.Sort((a, b) => a.beat.CompareTo(b.beat));

        _nextObstacleIndex = 0;
        _nextBonusIndex = 0;
        _spawnedCount = 0;
    }

    public void StartGame()
    {
        StartGame(MatchMode.Versus);
    }

    public void StartGame(MatchMode mode)
    {
        if (_running)
            return;

        if (!_configurationValid || beatMap == null ||
            !ValidateModeConfiguration(mode))
        {
            Debug.LogError(
                "[MasterSpawner] No se puede iniciar por una configuración inválida.",
                this
            );
            return;
        }

        _activeMode = mode;
        _running = true;
        StartCoroutine(RunSpawner());
    }

    private IEnumerator RunSpawner()
    {
        yield return new WaitForSeconds(beatMap.startOffsetSeconds);

        if (musicSource != null && beatMap.song != null)
        {
            musicSource.clip = beatMap.song;
            musicSource.Play();
        }

        _gameStartRealTime = Time.time;

        while (_nextObstacleIndex < _obstacleEvents.Count ||
               _nextBonusIndex < _bonusEvents.Count)
        {
            float elapsed = Time.time - _gameStartRealTime;
            float audioTime = musicSource != null && musicSource.isPlaying
                ? musicSource.time
                : Mathf.Max(0f, elapsed);

            SpawnDueObstacles(audioTime);
            ProcessDueBonusEvents(audioTime);

            yield return null;
        }

        yield return new WaitUntil(() =>
            (musicSource == null || !musicSource.isPlaying) &&
            AreAllTrackObstaclesGone());

        _running = false;

        if (gameManager1 != null)
            gameManager1.SetBonusArea(false);

        if (_activeMode == MatchMode.Versus && gameManager2 != null)
            gameManager2.SetBonusArea(false);

        Debug.Log("[MasterSpawner] Canción terminada.");
        OnSongFinishedEvent?.Invoke();
    }

    private void SpawnDueObstacles(float audioTime)
    {
        while (_nextObstacleIndex < _obstacleEvents.Count)
        {
            BeatEvent beatEvent = _obstacleEvents[_nextObstacleIndex];
            float beatTime = beatMap.BeatToSeconds(beatEvent.beat);
            float spawnTime = beatTime - TravelTime;

            if (audioTime < spawnTime)
                break;

            SpawnObstaclePair(beatEvent);
            _nextObstacleIndex++;
        }
    }

    private void ProcessDueBonusEvents(float audioTime)
    {
        while (_nextBonusIndex < _bonusEvents.Count)
        {
            BeatEvent beatEvent = _bonusEvents[_nextBonusIndex];
            float beatTime = beatMap.BeatToSeconds(beatEvent.beat);

            if (audioTime < beatTime)
                break;

            bool active = beatEvent.isBonusAreaStart && !beatEvent.isBonusAreaEnd;

            if (gameManager1 != null)
                gameManager1.SetBonusArea(active);

            if (_activeMode == MatchMode.Versus && gameManager2 != null)
                gameManager2.SetBonusArea(active);

            Debug.Log(
                active
                    ? $"[MasterSpawner] Inicia Área Bonus en beat {beatEvent.beat}."
                    : $"[MasterSpawner] Termina Área Bonus en beat {beatEvent.beat}."
            );

            _nextBonusIndex++;
        }
    }

    private void SpawnObstaclePair(BeatEvent beatEvent)
    {
        // Todos los valores aleatorios se calculan una sola vez para que ambos
        // jugadores reciban exactamente el mismo desafío.
        float holePosX = beatEvent.IsHoleRandom
            ? Random.Range(holeXMin, holeXMax)
            : beatEvent.holePositionX;

        bool isTutorial = _spawnedCount < tutorialObstacleCount;
        bool isFake = _isHardDifficulty && Random.value < fakeObstacleChance;

        float holdDuration = 0f;
        if (!isTutorial && !isFake && beatEvent.isHoldNote)
            holdDuration = beatEvent.holdDuration;

        SpawnForPlayer(
            beatEvent,
            holePosX,
            isTutorial,
            isFake,
            holdDuration,
            player1,
            gameManager1,
            uiManager1,
            obstacleParent1,
            _player1Layer
        );

        if (_activeMode == MatchMode.Versus)
        {
            SpawnForPlayer(
                beatEvent,
                holePosX,
                isTutorial,
                isFake,
                holdDuration,
                player2,
                gameManager2,
                uiManager2,
                obstacleParent2,
                _player2Layer
            );
        }

        _spawnedCount++;
    }

    private void SpawnForPlayer(
        BeatEvent beatEvent,
        float holePosX,
        bool isTutorial,
        bool isFake,
        float holdDuration,
        PoseController player,
        GameManager gameManager,
        UIManager uiManager,
        Transform obstacleParent,
        int renderLayer)
    {
        GameObject obstacleObject = Instantiate(obstaclePrefab, obstacleParent);
        obstacleObject.transform.localPosition = new Vector3(0f, 0f, spawnZ);
        obstacleObject.transform.localRotation = Quaternion.identity;

        TrackLayerUtility.SetLayerRecursively(obstacleObject, renderLayer);

        Obstacle obstacle = obstacleObject.GetComponent<Obstacle>();
        if (obstacle == null)
        {
            Debug.LogError(
                "[MasterSpawner] El prefab no contiene el componente Obstacle.",
                obstacleObject
            );
            Destroy(obstacleObject);
            return;
        }

        obstacle.Initialize(
            beatEvent.requiredPose,
            holePosX,
            player,
            obstacleSpeed,
            _currentEarlyWindow,
            _currentLateWindow,
            isTutorial,
            isFake,
            holdDuration,
            gameManager,
            uiManager,
            renderLayer
        );
    }

    private bool AreAllTrackObstaclesGone()
    {
        bool player1Clear =
            obstacleParent1.GetComponentsInChildren<Obstacle>().Length == 0;

        if (_activeMode == MatchMode.SinglePlayer)
            return player1Clear;

        bool player2Clear =
            obstacleParent2.GetComponentsInChildren<Obstacle>().Length == 0;

        return player1Clear && player2Clear;
    }

    public void StopGame()
    {
        StopAllCoroutines();
        _running = false;

        if (musicSource != null)
            musicSource.Stop();

        if (gameManager1 != null)
            gameManager1.SetBonusArea(false);

        if (_activeMode == MatchMode.Versus && gameManager2 != null)
            gameManager2.SetBonusArea(false);

        DestroyTrackObstacles(obstacleParent1);

        if (_activeMode == MatchMode.Versus)
            DestroyTrackObstacles(obstacleParent2);
    }

    private static void DestroyTrackObstacles(Transform obstacleParent)
    {
        if (obstacleParent == null)
            return;

        Obstacle[] obstacles =
            obstacleParent.GetComponentsInChildren<Obstacle>();

        foreach (Obstacle obstacle in obstacles)
            Destroy(obstacle.gameObject);
    }

    public void SetPaused(bool paused)
    {
        if (musicSource == null)
            return;

        if (paused)
            musicSource.Pause();
        else
            musicSource.UnPause();
    }

#if UNITY_EDITOR
    [ContextMenu("Generar BeatMap Completo")]
    private void GenerateFullBeatMap()
    {
        if (beatMap == null)
        {
            Debug.LogError("[MasterSpawner] No hay un Beatmap asignado.");
            return;
        }

        AudioClip clip = beatMap.song != null
            ? beatMap.song
            : musicSource != null ? musicSource.clip : null;

        if (clip == null)
        {
            Debug.LogError("[MasterSpawner] No hay un AudioClip asignado.");
            return;
        }

        UnityEditor.Undo.RecordObject(beatMap, "Generar BeatMap Completo");
        beatMap.events.Clear();

        float beatsPerSecond = beatMap.bpm / 60f;
        int totalBeats = Mathf.FloorToInt(clip.length * beatsPerSecond);
        PoseType[] poses =
        {
            PoseType.PoseA,
            PoseType.PoseB,
            PoseType.PoseC,
            PoseType.PoseD
        };

        for (int beat = 1; beat <= totalBeats; beat++)
        {
            if (beat % 4 != 0)
                continue;

            beatMap.events.Add(new BeatEvent
            {
                beat = beat,
                requiredPose = poses[Random.Range(0, poses.Length)],
                holePositionX = -999f
            });
        }

        UnityEditor.EditorUtility.SetDirty(beatMap);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log($"[MasterSpawner] Generados {beatMap.events.Count} obstáculos.");
    }
#endif
}
