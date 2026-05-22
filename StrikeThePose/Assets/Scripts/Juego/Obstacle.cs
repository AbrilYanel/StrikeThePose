using UnityEngine;
using TMPro;

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

    [Header("Colores por pose")]
    [SerializeField] private Color colorPoseA = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color colorPoseB = new Color(1f, 0.4f, 0.2f);
    [SerializeField] private Color colorPoseC = new Color(0.3f, 0.9f, 0.3f);
    [SerializeField] private Color colorPoseD = new Color(0.9f, 0.2f, 0.8f);

    [Header("Ventana de juicio")]
    [Tooltip("Segundos ANTES de la línea en los que se acepta input")]
    [SerializeField] private float earlyWindowSeconds = 0.4f;
    [Tooltip("Segundos DESPUÉS de la línea en los que se acepta input")]
    [SerializeField] private float lateWindowSeconds = 0.3f;

    [Header("Destrucción")]
    [SerializeField] private float destroyOffset = 3f;

    [Header("Tutorial")]
    [SerializeField] private float tutorialShowDistance = 8f;

    public PoseType RequiredPose { get; private set; }
    public float HolePositionX { get; private set; }

    private bool _evaluated = false;
    private bool _isTutorial = false;
    private bool _tutorialShown = false;
    private bool _ownsHint = false;
    private bool _enteredWindow = false;
    private PoseController _player;
    private float _judgeLineZ;
    private float _destroyZ;

    // ✅ Zona de evaluación en unidades de mundo
    private float _windowStartZ;
    private float _windowEndZ;

    public void Initialize(PoseType requiredPose, float holePosX, PoseController player, bool showTutorial = false)
    {
        RequiredPose = requiredPose;
        HolePositionX = holePosX;
        _player = player;
        _isTutorial = showTutorial;
        _judgeLineZ = player.transform.position.z;
        _destroyZ = _judgeLineZ + destroyOffset;

        // ✅ Calcular zona de evaluación
        _windowStartZ = _judgeLineZ - (earlyWindowSeconds * moveSpeed);
        _windowEndZ = _judgeLineZ + (lateWindowSeconds * moveSpeed);

        BuildWall();
        ApplyColor();

        if (_isTutorial && UIManager.Instance != null && !UIManager.Instance.IsTutorialHintActive)
        {
            _ownsHint = true;
            UIManager.Instance.ShowTutorialHint(RequiredPose);
        }
    }

    private void Update()
    {
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
        float z = transform.position.z;

        // Tutorial hint
        if (_isTutorial && !_ownsHint && !_evaluated)
        {
            if (UIManager.Instance != null && !UIManager.Instance.IsTutorialHintActive)
            {
                _ownsHint = true;
                UIManager.Instance.ShowTutorialHint(RequiredPose);
            }
        }

        // ✅ Verificar si estamos dentro de la ventana de juicio
        if (!_evaluated && z >= _windowStartZ && z <= _windowEndZ)
        {
            _enteredWindow = true;

            // Verificar si el jugador presionó la tecla correcta mientras está en la ventana
            float timeSinceInput = Time.time - _player.LastInputTime;
            bool recentInput = timeSinceInput <= Time.deltaTime * 2f; // Input de este frame o el anterior

            if (recentInput && _player.LastInputPose == RequiredPose)
            {
                bool positionOk = IsPlayerInHole(_player.transform.position.x);
                EvaluateResult(true && positionOk);
            }
        }

        // ✅ Si salió de la ventana sin acertar, es fallo
        if (!_evaluated && _enteredWindow && z > _windowEndZ)
        {
            EvaluateResult(false);
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

        Debug.Log($"[Obstacle] Resultado: {(success ? "ACIERTO" : "FALLO")} | Pose requerida: {RequiredPose}");
    }

    private void ReleaseHint()
    {
        if (_ownsHint && UIManager.Instance != null)
        {
            _ownsHint = false;
            UIManager.Instance.HideTutorialHint();
        }
    }

    private void OnDestroy()
    {
        ReleaseHint();
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
            _ => Color.white
        };

        foreach (var mr in GetComponentsInChildren<MeshRenderer>())
            mr.material.color = c;
    }

    private void OnDrawGizmos()
    {
        float jz = Application.isPlaying ? _judgeLineZ : 0f;
        float earlyDist = earlyWindowSeconds * moveSpeed;
        float lateDist = lateWindowSeconds * moveSpeed;
        float halfW = totalWidth / 2f;
        float halfH = wallHeight;

        // Hueco
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(
            transform.position + new Vector3(HolePositionX, 2.2f, 0f),
            new Vector3(holeWidth, wallHeight, wallDepth));

        // ✅ Zona de evaluación completa (early + late)
        float windowTotalDepth = earlyDist + lateDist;
        float windowCenterZ = jz - earlyDist + windowTotalDepth / 2f;
        Gizmos.color = new Color(0f, 1f, 0f, 0.18f);
        Gizmos.DrawCube(
            new Vector3(0f, halfH / 2f, windowCenterZ),
            new Vector3(totalWidth, halfH, windowTotalDepth));

        // Límite early (verde)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz - earlyDist), new Vector3(halfW, 0f, jz - earlyDist));
        Gizmos.DrawLine(new Vector3(-halfW, halfH, jz - earlyDist), new Vector3(halfW, halfH, jz - earlyDist));

        // Línea de juicio (amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz), new Vector3(halfW, 0f, jz));
        Gizmos.DrawLine(new Vector3(-halfW, halfH, jz), new Vector3(halfW, halfH, jz));
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz), new Vector3(-halfW, halfH, jz));
        Gizmos.DrawLine(new Vector3(halfW, 0f, jz), new Vector3(halfW, halfH, jz));

        // Límite late (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz + lateDist), new Vector3(halfW, 0f, jz + lateDist));
        Gizmos.DrawLine(new Vector3(-halfW, halfH, jz + lateDist), new Vector3(halfW, halfH, jz + lateDist));

#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.Label(new Vector3(halfW + 0.3f, 0f, jz), "JUDGE");
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.Label(
            new Vector3(halfW + 0.3f, 0f, jz - earlyDist),
            $"EARLY -{earlyWindowSeconds * 1000f:F0}ms");
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.Label(
            new Vector3(halfW + 0.3f, 0f, jz + lateDist),
            $"LATE +{lateWindowSeconds * 1000f:F0}ms");
#endif
    }
}