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
    [Tooltip("Z donde aparece el obstáculo (delante del jugador)")]
    [SerializeField] private float spawnZ = 30f;

    [Header("Límites del hueco aleatorio")]
    [SerializeField] private float holeXMin = -4f;
    [SerializeField] private float holeXMax = 4f;

    

    private List<BeatEvent> _pendingEvents;
    private int _nextEventIndex = 0;
    private float _songTime = 0f;
    private bool _running = false;

    // Anticipación: el obstáculo nace en spawnZ y llega al jugador
    // en el tiempo del beat. Calculamos cuánto antes hay que spawnearlo.
    private float TravelTime
    {
        get
        {
            float speed = 8f; 
                             
            return Mathf.Abs(spawnZ) / speed;
        }
    }



    private void Start()
    {
        Debug.Log($"[Spawner] Start ejecutado. BeatMap: {beatMap}, Prefab: {obstaclePrefab}, Player: {player}");
        if (beatMap == null)
        {
            Debug.LogError("[ObstacleSpawner] No hay BeatMap asignado.");
            return;
        }

        // Copiar la lista y ordenar por beat por si el artista no los puso en orden
        _pendingEvents = new List<BeatEvent>(beatMap.events);
        _pendingEvents.Sort((a, b) => a.beat.CompareTo(b.beat));

        StartCoroutine(RunSpawner());
    }

   

    private IEnumerator RunSpawner()
    {
        // Esperar el offset inicial
        yield return new WaitForSeconds(beatMap.startOffsetSeconds);

        // Arrancar la música
        if (musicSource != null && beatMap.song != null)
        {
            musicSource.clip = beatMap.song;
            musicSource.Play();
        }

        _running = true;
        _songTime = 0f;
        float startRealTime = Time.time;

        while (_nextEventIndex < _pendingEvents.Count)
        {
            _songTime = Time.time - startRealTime;
            BeatEvent ev = _pendingEvents[_nextEventIndex];
            float beatTime = beatMap.BeatToSeconds(ev.beat);

            // Spawneamos el obstáculo con anticipación para que llegue justo en el beat
            float spawnTime = beatTime - TravelTime;

            if (_songTime >= spawnTime)
            {
                SpawnObstacle(ev);
                _nextEventIndex++;
            }
            yield return null;
        }

        _running = false;
        Debug.Log("[ObstacleSpawner] BeatMap terminado.");
    }

   

    private void SpawnObstacle(BeatEvent ev)
    {
        if (obstaclePrefab == null || player == null) return;

        // Determinar posición X del hueco
        float holePosX = ev.IsHoleRandom
            ? Random.Range(holeXMin, holeXMax)
            : ev.holePositionX;

        // Instanciar en spawnZ, Y=0, X=0 (la pared cubre todo el ancho)
        Vector3 spawnPos = new Vector3(0f, 0f, spawnZ);
        GameObject go = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        Obstacle obstacle = go.GetComponent<Obstacle>();
        if (obstacle != null)
            obstacle.Initialize(ev.requiredPose, holePosX, player);

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
            Debug.LogError("[Spawner] No hay un Beatmap asignado en el inspector.");
            return;
        }
        if (musicSource == null || musicSource.clip == null)
        {
            Debug.LogError("[Spawner] No hay un AudioSource con un Clip asignado.");
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
                BeatEvent newEvent = new BeatEvent();
                newEvent.beat = i;
                newEvent.requiredPose = poses[Random.Range(0, poses.Length)];
                newEvent.holePositionX = -999f; // Random X
                beatMap.events.Add(newEvent);
            }
        }

       
        UnityEditor.EditorUtility.SetDirty(beatMap);
        UnityEditor.AssetDatabase.SaveAssets();

        Debug.Log($"[Spawner] ¡Éxito! Generados {beatMap.events.Count} obstáculos para {songDuration:F2} segundos de música.");
    }
#endif
}