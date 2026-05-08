using UnityEngine;


public class Obstacle : MonoBehaviour
{
  
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Partes visuales")]
    [SerializeField] private Transform wallLeft;
    [SerializeField] private Transform wallRight;
    [SerializeField] private BoxCollider holeZoneTrigger;

    [Header("Dimensiones")]
    [Tooltip("Ancho total de la pared")]
    [SerializeField] private float totalWidth = 14f;
    [Tooltip("Ancho del hueco por donde pasa el personaje")]
    [SerializeField] private float holeWidth = 2.5f;
    [Tooltip("Alto de la pared")]
    [SerializeField] private float wallHeight = 4f;
    [Tooltip("Profundidad del cubo (Z)")]
    [SerializeField] private float wallDepth = 0.5f;

    [Header("Colores por pose")]
    [SerializeField] private Color colorPoseA = new Color(0.2f, 0.6f, 1f);   // azul
    [SerializeField] private Color colorPoseB = new Color(1f, 0.4f, 0.2f);   // naranja
    [SerializeField] private Color colorPoseC = new Color(0.3f, 0.9f, 0.3f); // verde
    [SerializeField] private Color colorPoseD = new Color(0.9f, 0.2f, 0.8f); // magenta

    [Header("Destrucción")]
    [Tooltip("Posición Z en la que el obstáculo se destruye (detrás del personaje)")]
    [SerializeField] private float destroyZ = -5f;

   
    public PoseType RequiredPose { get; private set; }
    public float HolePositionX { get; private set; }

    private bool _evaluated = false;   // ya se juzgó al jugador?
    private PoseController _player;

   
    public void Initialize(PoseType requiredPose, float holePosX, PoseController player)
    {
        RequiredPose = requiredPose;
        HolePositionX = holePosX;
        _player = player;

        BuildWall();
        ApplyColor();
    }


    private void Update()
    {
        // Avanzar hacia el jugador (hacia Z)
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;

        if (transform.position.z >= destroyZ)
            Destroy(gameObject);
    }

   
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Algo entró en el trigger: " + other.name);
        if (other.CompareTag("Player"))
        {
           
            bool success = _player != null && _player.CurrentPose == RequiredPose;

            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnObstacleResult(success, RequiredPose);
            }

           
            if (success)
            {
                Debug.Log("<color=green>¡POSTURA CORRECTA!</color>");
            }
            else
            {
                Debug.Log("<color=red>¡POSTURA INCORRECTA!</color>");
            }
        }
    }


    private void SpawnFeedbackEffect(bool success)
    {
        
        foreach (var mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.material.color = success ? Color.white : Color.black;
        }

        
    }
    private bool IsPlayerInHole(float playerX)
    {
        float halfHole = holeWidth / 2f;
        return playerX >= HolePositionX - halfHole &&
               playerX <= HolePositionX + halfHole;
    }

   
    private void BuildWall()
    {
        float halfHole = holeWidth / 2f;
        float leftEnd = HolePositionX - halfHole;   // borde derecho de WallLeft
        float rightStart = HolePositionX + halfHole;   // borde izquierdo de WallRight

        float leftOrigin = -totalWidth / 2f;
        float rightEnd = totalWidth / 2f;

        float leftWidth = leftEnd - leftOrigin;
        float rightWidth = rightEnd - rightStart;

        // --- WallLeft ---
        if (wallLeft != null)
        {
            wallLeft.localScale = new Vector3(Mathf.Max(leftWidth, 0f), wallHeight, wallDepth);
            wallLeft.localPosition = new Vector3(leftOrigin + leftWidth / 2f, 2.2f, 0f);
        }

        // --- WallRight ---
        if (wallRight != null)
        {
            wallRight.localScale = new Vector3(Mathf.Max(rightWidth, 0f), wallHeight, wallDepth);
            wallRight.localPosition = new Vector3(rightStart + rightWidth / 2f, 2.2f, 0f);
        }

        // --- HoleZone trigger ---
        if (holeZoneTrigger != null)
        {
            holeZoneTrigger.center = new Vector3(HolePositionX, 2.2f, 0f);
            holeZoneTrigger.size = new Vector3(holeWidth, wallHeight, wallDepth * 2f);
        }
    }

    private void ApplyColor()
    {
        Color c = RequiredPose switch
        {
            PoseType.PoseA => colorPoseA,
            PoseType.PoseB => colorPoseB,
            PoseType.PoseC => colorPoseC,
            PoseType.PoseD => colorPoseD,
            _ => Color.white
        };

        // Aplicar a ambas partes de la pared
        foreach (var mr in GetComponentsInChildren<MeshRenderer>())
            mr.material.color = c;
    }

   

    private void OnDrawGizmos()
    {
        // Visualiza el hueco en azul claro en el editor
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(
            transform.position + new Vector3(HolePositionX, 0f, 0f),
            new Vector3(holeWidth, wallHeight, wallDepth));
    }
}