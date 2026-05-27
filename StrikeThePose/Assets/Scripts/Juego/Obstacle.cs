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
    [SerializeField] private float totalWidth = 14f;
    [SerializeField] private float holeWidth = 2.5f;
    [SerializeField] private float wallHeight = 4f;
    [SerializeField] private float wallDepth = 0.5f;

    [Header("Colores — Poses simples")]
    [SerializeField] private Color colorPoseA = new Color(0.2f, 0.6f, 1f);    // azul
    [SerializeField] private Color colorPoseB = new Color(1f, 0.4f, 0.2f);   // naranja
    [SerializeField] private Color colorPoseC = new Color(0.3f, 0.9f, 0.3f);  // verde
    [SerializeField] private Color colorPoseD = new Color(0.9f, 0.2f, 0.8f);  // magenta

    [Header("Colores — Poses combinadas")]
    [SerializeField] private Color colorPoseAB = new Color(0.6f, 0.3f, 1f);   // violeta  (W+A)
    [SerializeField] private Color colorPoseAD = new Color(1f, 0.9f, 0.1f);   // amarillo (W+D)
    [SerializeField] private Color colorPoseBC = new Color(0.1f, 0.9f, 0.9f); // cyan     (A+S)
    [SerializeField] private Color colorPoseCD = new Color(1f, 0.5f, 0.7f);   // rosa     (S+D)

    [Header("Ventana de juicio")]
    [Tooltip("Segundos ANTES de la línea en los que se acepta input")]
    [SerializeField] private float earlyWindowSeconds = 0.4f;
    [Tooltip("Segundos DESPUÉS de la línea en los que se acepta input")]
    [SerializeField] private float lateWindowSeconds = 0.3f;

    [Header("Destrucción")]
    [SerializeField] private float destroyOffset = 3f;

    public PoseType RequiredPose { get; private set; }
    public float HolePositionX { get; private set; }

    // Nueva propiedad para marcar si este obstáculo es falso/trampa
    public bool IsFake { get; private set; } = false;

    private bool _evaluated = false;
    private bool _ownsHint = false;
    private bool _enteredWindow = false;
    private PoseController _player;

    private float _judgeLineZ;
    private float _destroyZ;
    private float _windowStartZ;
    private float _windowEndZ;

    public void Initialize(
        PoseType requiredPose,
        float holePosX,
        PoseController player,
        float speed,
        float earlyWindow,
        float lateWindow,
        bool showTutorial = false,
        bool isFake = false
    )
    {
        RequiredPose = requiredPose;
        HolePositionX = holePosX;
        _player = player;
        IsFake = isFake;

        // Configuraciones dinámicas de velocidad y juicio
        moveSpeed = speed;
        earlyWindowSeconds = earlyWindow;
        lateWindowSeconds = lateWindow;

        _judgeLineZ = player.transform.position.z;
        _destroyZ = _judgeLineZ + destroyOffset;
        _windowStartZ = _judgeLineZ - (earlyWindowSeconds * moveSpeed);
        _windowEndZ = _judgeLineZ + (lateWindowSeconds * moveSpeed);

        BuildWall();
        ApplyColor();

        // En obstáculos fake no mostramos el texto de tutorial de ayuda
        if (showTutorial && !IsFake && UIManager.Instance != null && !UIManager.Instance.IsTutorialHintActive)
        {
            _ownsHint = true;
            UIManager.Instance.ShowTutorialHint(RequiredPose);
        }
    }

    private void Update()
    {
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
        float z = transform.position.z;

        // Mostrar hint si todavía no se tomó
        if (!_ownsHint && !_evaluated && !IsFake)
        {
            if (UIManager.Instance != null && !UIManager.Instance.IsTutorialHintActive)
            {
                // Solo para obstáculos de tutorial
            }
        }

        // Dentro de la ventana de juicio
        if (!_evaluated && z >= _windowStartZ && z <= _windowEndZ)
        {
            _enteredWindow = true;

            if (IsFake)
            {
                // Si el obstáculo es FAKE, presionar CUALQUIER tecla de juego activa un fallo inmediato!
                if (_player.CurrentPose != PoseType.Idle)
                {
                    EvaluateResult(false); // Falló por presionar una tecla
                }
            }
            else
            {
                // Comportamiento normal para obstáculos reales
                float timeSinceInput = Time.time - _player.LastInputTime;
                bool recentInput = timeSinceInput <= Time.deltaTime * 2f;

                if (recentInput && _player.LastInputPose == RequiredPose)
                {
                    bool posOk = IsPlayerInHole(_player.transform.position.x);
                    EvaluateResult(posOk);
                }
            }
        }

        // Salió de la ventana sin que se haya evaluado
        if (!_evaluated && _enteredWindow && z > _windowEndZ)
        {
            if (IsFake)
            {
                // ¡Si el jugador esquivó con éxito (no presionó nada), cuenta como acierto!
                EvaluateResult(true);
            }
            else
            {
                // Si era un obstáculo real y no lo presionó -> Fallo
                EvaluateResult(false);
            }
        }

        if (z >= _destroyZ)
        {
            ReleaseHint();
            Destroy(gameObject);
        }
    }

    private void EvaluateResult(bool success)
    {
        _evaluated = true;

        if (GameManager.Instance != null)
            GameManager.Instance.OnObstacleResult(success, RequiredPose);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowHitFeedback(success);

        if (success)
        {
            ReleaseHint();
            Destroy(gameObject);
        }

        Debug.Log($"[Obstacle] {(success ? "ACIERTO" : "FALLO")} | Pose: {RequiredPose} | ¿Era Fake?: {IsFake}");
    }

    private void ReleaseHint()
    {
        if (_ownsHint && UIManager.Instance != null)
        {
            _ownsHint = false;
            UIManager.Instance.HideTutorialHint();
        }
    }

    private void OnDestroy() => ReleaseHint();

    private bool IsPlayerInHole(float playerX)
    {
        float half = holeWidth / 2f;
        return playerX >= HolePositionX - half && playerX <= HolePositionX + half;
    }

    private void BuildWall()
    {
        float halfHole = holeWidth / 2f;
        float leftEnd = HolePositionX - halfHole;
        float rightStart = HolePositionX + halfHole;
        float leftOrigin = -totalWidth / 2f;
        float rightEnd = totalWidth / 2f;

        float leftWidth = leftEnd - leftOrigin;
        float rightWidth = rightEnd - rightStart;

        if (wallLeft != null)
        {
            wallLeft.localScale = new Vector3(Mathf.Max(leftWidth, 0f), wallHeight, wallDepth);
            wallLeft.localPosition = new Vector3(leftOrigin + leftWidth / 2f, 2.2f, 0f);
        }

        if (wallRight != null)
        {
            wallRight.localScale = new Vector3(Mathf.Max(rightWidth, 0f), wallHeight, wallDepth);
            wallRight.localPosition = new Vector3(rightStart + rightWidth / 2f, 2.2f, 0f);
        }

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
            PoseType.PoseAB => colorPoseAB,
            PoseType.PoseAD => colorPoseAD,
            PoseType.PoseBC => colorPoseBC,
            PoseType.PoseCD => colorPoseCD,
            _ => Color.white,
        };

        if (IsFake)
        {
            // Apagamos/opacamos el color base multiplicando el RGB por un factor para que se vea más oscuro y apagado
            c = new Color(c.r * 0.55f, c.g * 0.55f, c.b * 0.55f, c.a);
        }

        foreach (var mr in GetComponentsInChildren<MeshRenderer>())
            mr.material.color = c;
    }

    private void OnDrawGizmos()
    {
        float jz = Application.isPlaying ? _judgeLineZ : 0f;
        float earlyD = earlyWindowSeconds * moveSpeed;
        float lateD = lateWindowSeconds * moveSpeed;
        float halfW = totalWidth / 2f;
        float halfH = wallHeight;

        Gizmos.color = IsFake ? new Color(0.3f, 0.3f, 0.3f, 0.3f) : new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position + new Vector3(HolePositionX, 2.2f, 0f),
                        new Vector3(holeWidth, wallHeight, wallDepth));

        float winDepth = earlyD + lateD;
        float winCenterZ = jz - earlyD + winDepth / 2f;

        Gizmos.color = IsFake ? new Color(0.4f, 0f, 0f, 0.15f) : new Color(0f, 1f, 0f, 0.18f);
        Gizmos.DrawCube(new Vector3(0f, halfH / 2f, winCenterZ),
                        new Vector3(totalWidth, halfH, winDepth));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz - earlyD), new Vector3(halfW, 0f, jz - earlyD));
        Gizmos.DrawLine(new Vector3(-halfW, halfH, jz - earlyD), new Vector3(halfW, halfH, jz - earlyD));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz), new Vector3(halfW, 0f, jz));
        Gizmos.DrawLine(new Vector3(-halfW, halfH, jz), new Vector3(halfW, halfH, jz));
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz), new Vector3(-halfW, halfH, jz));
        Gizmos.DrawLine(new Vector3(halfW, 0f, jz), new Vector3(halfW, halfH, jz));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz + lateD), new Vector3(halfW, 0f, jz + lateD));
        Gizmos.DrawLine(new Vector3(-halfW, halfH, jz + lateD), new Vector3(halfW, halfH, jz + lateD));

#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.Label(new Vector3(halfW + 0.3f, 0f, jz), IsFake ? "FAKE JUDGE" : "JUDGE");
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.Label(new Vector3(halfW + 0.3f, 0f, jz - earlyD),
            $"EARLY -{earlyWindowSeconds * 1000f:F0}ms");
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.Label(new Vector3(halfW + 0.3f, 0f, jz + lateD),
            $"LATE +{lateWindowSeconds * 1000f:F0}ms");
#endif
    }
}