using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Datos rítmicos")]
    [SerializeField] private Beatmap beatMap;

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

    [Header("Velocidad del obstáculo")]
    [Tooltip("Debe coincidir con moveSpeed en el prefab Obstacle")]
    [SerializeField] private float obstacleSpeed = 8f;

    [Header("Tutorial")]
    [Tooltip("Cuántos obstáculos al inicio llevan texto de tutorial")]
    [SerializeField] public int tutorialObstacleCount = 10;

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

        if (beatMap == null || obstaclePrefab == null || player == null)
        {
            Debug.LogError("[ObstacleSpawner] Falta asignar una referencia en el Inspector.");
            return;
        }

        _pendingEvents = new List<BeatEvent>(beatMap.events);
        _pendingEvents.Sort((a, b) => a.beat.CompareTo(b.beat));

        
    }

   
    public void StartGame()
    {
        if (_running) return;
        StartCoroutine(RunSpawner());
    }

    private IEnumerator RunSpawner()
    {
        // Esperar el offset inicial (ya con timeScale = 1)
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

        // Esperar a que el último obstáculo pase antes de declarar victoria
        yield return new WaitForSeconds(TravelTime + 1f);

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
            obstacle.Initialize(ev.requiredPose, holePosX, player, isTutorial);
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