using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Datos rítmicos (Default / Normal)")]
    [SerializeField] private Beatmap beatMap;

    [Header("Dificultades (Opcionales)")]
    [Tooltip("Asigna aquí un Beatmap simplificado si lo tienes")]
    [SerializeField] private Beatmap easyBeatMap;
    [Tooltip("Asigna aquí el Beatmap por defecto")]
    [SerializeField] private Beatmap normalBeatMap;
    [Tooltip("Asigna aquí un Beatmap de alta dificultad si lo tienes")]
    [SerializeField] private Beatmap hardBeatMap;

    [Header("Prefab")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Referencias")]
    [SerializeField] private PoseController player;
    [SerializeField] private AudioSource musicSource;

    [Header("Posición de spawn")]
    [SerializeField] private float spawnZ = -20f;

    [Header("Límites del hueco aleatorio")]
    [SerializeField] private float holeXMin = -4f;
    [SerializeField] private float holeXMax = 4f;

    [Header("Velocidad del obstáculo por Dificultad")]
    [SerializeField] private float easySpeed = 6f;
    [SerializeField] private float normalSpeed = 8f;
    [SerializeField] private float hardSpeed = 11f;

    [Header("Ventanas de Juicio por Dificultad")]
    [SerializeField] private float easyEarlyWindow = 0.6f;
    [SerializeField] private float easyLateWindow = 0.45f;
    [SerializeField] private float normalEarlyWindow = 0.4f;
    [SerializeField] private float normalLateWindow = 0.3f;
    [SerializeField] private float hardEarlyWindow = 0.28f;
    [SerializeField] private float hardLateWindow = 0.21f;

    [Header("Tutorial")]
    [Tooltip("Cuántos obstáculos al inicio llevan texto de tutorial")]
    [SerializeField] public int tutorialObstacleCount = 10;

    // Velocidad actual en runtime
    private float obstacleSpeed = 8f;
    private float _currentEarlyWindow = 0.4f;
    private float _currentLateWindow = 0.3f;

    private List<BeatEvent> _pendingEvents;
    private int _nextEventIndex = 0;
    private bool _running = false;
    private int _spawnedCount = 0;

    // Momento real en que el juego fue iniciado por el jugador
    private float _gameStartRealTime = -1f;

    private float TravelTime => Mathf.Abs(spawnZ) / obstacleSpeed;

    private void Start()
    {
        Debug.Log($"[Spawner] BeatMap: {beatMap} | Prefab: {obstaclePrefab} | Player: {player} | TravelTime: {TravelTime:F2}s");

        if (beatMap == null && easyBeatMap == null && normalBeatMap == null && hardBeatMap == null)
        {
            Debug.LogError("[ObstacleSpawner] No se asignó ningún Beatmap en el Inspector.");
            return;
        }

        if (obstaclePrefab == null || player == null)
        {
            Debug.LogError("[ObstacleSpawner] Falta asignar referencias críticas (Prefab/Player) en el Inspector.");
            return;
        }

        // Si no se asignó nada en dificultades específicas, rellenamos con el default
        if (normalBeatMap == null) normalBeatMap = beatMap;
        if (easyBeatMap == null) easyBeatMap = beatMap;
        if (hardBeatMap == null) hardBeatMap = beatMap;

        // Por seguridad, inicializamos con el beatmap por defecto hasta que se elija uno.
        InitializeBeatmap(normalBeatMap);
    }

    /// <summary>
    /// Configura las variables rítmicas del Spawner en base a la dificultad seleccionada.
    /// </summary>
    public void SetDifficulty(Difficulty difficulty)
    {
        Beatmap chosenMap = null;

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
        {
            InitializeBeatmap(chosenMap);
        }

        Debug.Log($"[Spawner] Dificultad {difficulty} cargada. Velocidad: {obstacleSpeed}. Ventana Early: {_currentEarlyWindow}s / Late: {_currentLateWindow}s.");
    }

    private void InitializeBeatmap(Beatmap targetBeatmap)
    {
        beatMap = targetBeatmap;
        _pendingEvents = new List<BeatEvent>(beatMap.events);
        _pendingEvents.Sort((a, b) => a.beat.CompareTo(b.beat));
        _nextEventIndex = 0;
        _spawnedCount = 0;
    }

    public void StartGame()
    {
        if (_running) return;
        StartCoroutine(RunSpawner());
    }

    private IEnumerator RunSpawner()
    {
        if (beatMap == null)
        {
            Debug.LogError("[Spawner] No se puede iniciar el spawner porque no hay ningún Beatmap cargado.");
            yield break;
        }

        // Esperar el offset inicial (con timeScale = 1)
        yield return new WaitForSeconds(beatMap.startOffsetSeconds);

        // Iniciar música
        if (musicSource != null && beatMap.song != null)
        {
            musicSource.clip = beatMap.song;
            musicSource.Play();
        }

        // Guardar tiempo real de inicio para sincronización
        _gameStartRealTime = Time.time;
        _running = true;

        while (_nextEventIndex < _pendingEvents.Count)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            {
                musicSource?.Stop();
                yield break;
            }

            float elapsed = Time.time - _gameStartRealTime;
            float audioTime = (musicSource != null && musicSource.isPlaying)
                ? musicSource.time
                : Mathf.Max(0f, elapsed);

            BeatEvent ev = _pendingEvents[_nextEventIndex];
            float beatTime = beatMap.BeatToSeconds(ev.beat);
            float spawnTime = beatTime - TravelTime;

            if (audioTime >= spawnTime)
            {
                SpawnObstacle(ev);
                _nextEventIndex++;
            }

            yield return null;
        }

        // Esperar a que la música termine Y no queden obstáculos en escena
        yield return new WaitUntil(() =>
            (musicSource == null || !musicSource.isPlaying) &&
            FindObjectsByType<Obstacle>(FindObjectsSortMode.None).Length == 0);

        _running = false;
        Debug.Log("[ObstacleSpawner] Canción terminada.");

        if (GameManager.Instance != null)
            GameManager.Instance.OnSongFinished();
    }

    private void SpawnObstacle(BeatEvent ev)
    {
        float holePosX = ev.IsHoleRandom
            ? Random.Range(holeXMin, holeXMax)
            : ev.holePositionX;

        Vector3 spawnPos = new Vector3(0f, 0f, spawnZ);
        GameObject go = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        Obstacle obstacle = go.GetComponent<Obstacle>();

        if (obstacle != null)
        {
            bool isTutorial = _spawnedCount < tutorialObstacleCount;
            // Pasar los valores dinámicos de velocidad y ventanas de juicio basados en la dificultad
            obstacle.Initialize(
                ev.requiredPose,
                holePosX,
                player,
                obstacleSpeed,
                _currentEarlyWindow,
                _currentLateWindow,
                isTutorial
            );
        }

        _spawnedCount++;
        Debug.Log($"[ObstacleSpawner] Spawn beat {ev.beat} | Pose: {ev.requiredPose} | HoleX: {holePosX:F2}");
    }

    public void SetPaused(bool paused)
    {
        if (paused) StopAllCoroutines();
        else StartCoroutine(RunSpawner());
    }

#if UNITY_EDITOR
    [ContextMenu("Generar BeatMap Completo")]
    private void GenerateFullBeatMap()
    {
        if (beatMap == null)
        {
            Debug.LogError("[Spawner] No hay un Beatmap asignado.");
            return;
        }

        if (musicSource == null || musicSource.clip == null)
        {
            Debug.LogError("[Spawner] No hay AudioSource con Clip asignado.");
            return;
        }

        UnityEditor.Undo.RecordObject(beatMap, "Generar BeatMap Completo");
        beatMap.events.Clear();

        float songDuration = musicSource.clip.length;
        float beatsPerSecond = beatMap.bpm / 60f;
        int totalBeats = Mathf.FloorToInt(songDuration * beatsPerSecond);

        PoseType[] poses = { PoseType.PoseA, PoseType.PoseB, PoseType.PoseC, PoseType.PoseD };

        for (int i = 1; i <= totalBeats; i++)
        {
            if (i % 4 == 0)
            {
                beatMap.events.Add(new BeatEvent
                {
                    beat = i,
                    requiredPose = poses[Random.Range(0, poses.Length)],
                    holePositionX = -999f
                });
            }
        }

        UnityEditor.EditorUtility.SetDirty(beatMap);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log($"[Spawner] Generados {beatMap.events.Count} obstáculos.");
    }
#endif
}
