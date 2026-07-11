using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxSpawn : MonoBehaviour
{
    [Header("Prefabs decorativos")]
    [Tooltip("Árboles, postes, rocas, carteles, etc. Se elige uno al azar en cada spawn.")]
    [SerializeField] private GameObject[] backgroundPrefabs;

    [Header("Referencias")]
    [SerializeField] private Camera targetCamera;

    [Header("Pista")]
    [Tooltip("Layer exclusiva de esta pista: Track_P1 o Track_P2.")]
    [SerializeField] private string trackLayerName = "Track_P1";

    [Header("Movimiento")]
    [Tooltip("Velocidad base con la que los objetos vienen hacia el jugador/cámara.")]
    [SerializeField] private float moveSpeed = 8f;
    [Tooltip("Multiplicador aleatorio mínimo de velocidad por objeto.")]
    [SerializeField] private float minSpeedMultiplier = 0.85f;
    [Tooltip("Multiplicador aleatorio máximo de velocidad por objeto.")]
    [SerializeField] private float maxSpeedMultiplier = 1.2f;

    [Header("Spawn en profundidad")]
    [Tooltip("Z donde aparecen los objetos. En tu juego los obstáculos aparecen en Z negativo y avanzan hacia Z positivo.")]
    [SerializeField] private float spawnZ = -35f;
    [Tooltip("Z donde se destruyen al pasar al jugador/cámara.")]
    [SerializeField] private float destroyZ = 8f;

    [Header("Spawn lateral")]
    [Tooltip("Distancia mínima desde el centro. Mantiene despejado el camino jugable.")]
    [SerializeField] private float sideXMin = 7f;
    [Tooltip("Distancia máxima desde el centro.")]
    [SerializeField] private float sideXMax = 13f;
    [Tooltip("Altura mínima de spawn.")]
    [SerializeField] private float yMin = 0f;
    [Tooltip("Altura máxima de spawn.")]
    [SerializeField] private float yMax = 0f;

    [Header("Frecuencia")]
    [Tooltip("Tiempo mínimo entre objetos por lado.")]
    [SerializeField] private float minSpawnInterval = 0.25f;
    [Tooltip("Tiempo máximo entre objetos por lado.")]
    [SerializeField] private float maxSpawnInterval = 0.65f;
    [Tooltip("Si está activo, intenta spawnear pares izquierda/derecha para reforzar sensación de velocidad.")]
    [SerializeField] private bool spawnBothSidesSometimes = true;
    [Range(0f, 1f)]
    [SerializeField] private float bothSidesChance = 0.45f;

    [Header("Escala y rotación")]
    [SerializeField] private Vector2 randomScaleRange = new Vector2(0.8f, 1.4f);
    [SerializeField] private bool randomYRotation = true;
    [SerializeField] private bool billboardToCamera = false;
    [Tooltip("Útil para sprites/planos: miran a cámara pero no se inclinan verticalmente.")]
    [SerializeField] private bool keepBillboardUpright = true;
    [Tooltip("Rotación decorativa aleatoria. Déjalo en 0 para árboles/sprites.")]
    [SerializeField] private float maxRandomRotationSpeed = 0f;

    [Header("Estado")]
    [SerializeField] private bool startAutomatically = false;

    private Coroutine _spawnRoutine;
    private bool _running;
    private int _trackLayer = -1;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        _trackLayer = LayerMask.NameToLayer(trackLayerName);

        if (_trackLayer < 0)
        {
            Debug.LogError(
                $"[ParallaxSpawn] La layer '{trackLayerName}' no existe. Creala en Tags and Layers.",
                this
            );
        }
    }

    private void Start()
    {
        if (startAutomatically)
            StartParallax();
    }

    public void StartParallax()
    {
        if (_running) return;
        if (backgroundPrefabs == null || backgroundPrefabs.Length == 0)
        {
            Debug.LogWarning("[ParallaxBackgroundSpawner] No hay prefabs decorativos asignados.");
            return;
        }

        _running = true;
        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void StopParallax(bool clearExistingObjects = false)
    {
        _running = false;

        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }

        if (clearExistingObjects)
        {
            ParallaxBackgroundObject[] ownObjects =
                GetComponentsInChildren<ParallaxBackgroundObject>(true);

            foreach (ParallaxBackgroundObject obj in ownObjects)
                Destroy(obj.gameObject);
        }
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    private IEnumerator SpawnRoutine()
    {
        while (_running)
        {
            bool spawnPair = spawnBothSidesSometimes && Random.value < bothSidesChance;

            if (spawnPair)
            {
                SpawnOne(-1);
                SpawnOne(1);
            }
            else
            {
                SpawnOne(Random.value < 0.5f ? -1 : 1);
            }

            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
        }
    }

    private void SpawnOne(int side)
    {
        if (backgroundPrefabs == null || backgroundPrefabs.Length == 0) return;

        GameObject prefab = backgroundPrefabs[Random.Range(0, backgroundPrefabs.Length)];
        if (prefab == null) return;

        float x = Random.Range(sideXMin, sideXMax) * Mathf.Sign(side);
        float y = Random.Range(yMin, yMax);
        Vector3 localSpawnPos = new Vector3(x, y, spawnZ);

        Quaternion localRotation = Quaternion.identity;
        if (randomYRotation)
            localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        GameObject go = Instantiate(prefab, transform);
        go.transform.localPosition = localSpawnPos;
        go.transform.localRotation = localRotation;
        TrackLayerUtility.SetLayerRecursively(go, _trackLayer);

        float scale = Random.Range(randomScaleRange.x, randomScaleRange.y);
        go.transform.localScale *= scale;

        float objectSpeed = moveSpeed * Random.Range(minSpeedMultiplier, maxSpeedMultiplier);
        float randomSpin = maxRandomRotationSpeed <= 0f
            ? 0f
            : Random.Range(-maxRandomRotationSpeed, maxRandomRotationSpeed);

        ParallaxBackgroundObject obj = go.GetComponent<ParallaxBackgroundObject>();
        if (obj == null)
            obj = go.AddComponent<ParallaxBackgroundObject>();

        obj.Initialize(
            objectSpeed,
            destroyZ,
            targetCamera,
            billboardToCamera,
            keepBillboardUpright,
            randomSpin
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
        Vector3 leftCenter = new Vector3(-(sideXMin + sideXMax) * 0.5f, (yMin + yMax) * 0.5f, (spawnZ + destroyZ) * 0.5f);
        Vector3 rightCenter = new Vector3((sideXMin + sideXMax) * 0.5f, (yMin + yMax) * 0.5f, (spawnZ + destroyZ) * 0.5f);
        Vector3 size = new Vector3(sideXMax - sideXMin, Mathf.Max(0.1f, yMax - yMin), Mathf.Abs(destroyZ - spawnZ));

        Gizmos.DrawCube(leftCenter, size);
        Gizmos.DrawCube(rightCenter, size);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(-sideXMax, yMin, spawnZ), new Vector3(sideXMax, yMin, spawnZ));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-sideXMax, yMin, destroyZ), new Vector3(sideXMax, yMin, destroyZ));
    }
}
