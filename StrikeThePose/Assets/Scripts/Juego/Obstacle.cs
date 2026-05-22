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

    [Header("Línea de juicio")]
    [SerializeField] private float judgeWindowSeconds = 0.15f;

    [Header("Destrucción")]
    [SerializeField] private float destroyOffset = 3f;

    [Header("Tutorial")]
    [Tooltip("Transform hijo vacío posicionado encima de la pared donde aparecerá el texto")]
    [SerializeField] private Transform tutorialTextAnchor;

    public PoseType RequiredPose { get; private set; }
    public float HolePositionX { get; private set; }

    private bool _evaluated = false;
    private PoseController _player;
    private float _judgeLineZ;
    private float _destroyZ;
    private GameObject _tutorialLabel;

    public void Initialize(PoseType requiredPose, float holePosX, PoseController player, bool showTutorial = false)
    {
        RequiredPose = requiredPose;
        HolePositionX = holePosX;
        _player = player;
        _judgeLineZ = player.transform.position.z;
        _destroyZ = _judgeLineZ + destroyOffset;

        BuildWall();
        ApplyColor();

        if (showTutorial)
            SpawnTutorialLabel(requiredPose);
    }

    private void Update()
    {
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;

        if (!_evaluated && transform.position.z >= _judgeLineZ)
            Evaluate();

        if (transform.position.z >= _destroyZ)
            Destroy(gameObject);
    }

    private void Evaluate()
    {
        _evaluated = true;

        bool positionOk = IsPlayerInHole(_player.transform.position.x);
        float timeSinceInput = Time.time - _player.LastInputTime;
        bool withinWindow = timeSinceInput >= 0f && timeSinceInput <= judgeWindowSeconds;
        bool poseOk = withinWindow && _player.LastInputPose == RequiredPose;
        bool success = positionOk && poseOk;

        if (GameManager.Instance != null)
            GameManager.Instance.OnObstacleResult(success, RequiredPose);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowHitFeedback(success);

        if (success) Destroy(gameObject);

        string timing = timeSinceInput <= judgeWindowSeconds
            ? $"{timeSinceInput * 1000f:F0}ms antes"
            : $"{timeSinceInput * 1000f:F0}ms (fuera de ventana)";

        Debug.Log($"[Obstacle] Pose: {(poseOk ? "OK" : "MAL")} | Posicion: {(positionOk ? "OK" : "MAL")} | Input: {timing}");
    }

    private bool IsPlayerInHole(float playerX)
    {
        float halfHole = holeWidth / 2f;
        return playerX >= HolePositionX - halfHole &&
               playerX <= HolePositionX + halfHole;
    }

    private void SpawnTutorialLabel(PoseType pose)
    {
        string keyName = pose switch
        {
            PoseType.PoseA => "W",
            PoseType.PoseB => "A",
            PoseType.PoseC => "S",
            PoseType.PoseD => "D",
            _ => "?"
        };

        string poseName = pose switch
        {
            PoseType.PoseA => "Pose A",
            PoseType.PoseB => "Pose B",
            PoseType.PoseC => "Pose C",
            PoseType.PoseD => "Pose D",
            _ => "Pose"
        };

        _tutorialLabel = new GameObject("TutorialLabel");
        _tutorialLabel.transform.SetParent(transform);

        Vector3 anchorPos = tutorialTextAnchor != null
            ? tutorialTextAnchor.position
            : transform.position + new Vector3(HolePositionX, wallHeight + 5f, 0f);
        _tutorialLabel.transform.position = anchorPos;

        //  Orientar el canvas hacia la cámara (mirando hacia +Z donde está el jugador)
        _tutorialLabel.transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);

        Canvas canvas = _tutorialLabel.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        
        RectTransform rt = _tutorialLabel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600f, 200f);
        rt.localScale = Vector3.one * 0.005f;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(_tutorialLabel.transform, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = $"Presioná  <b>{keyName}</b>\n<size=60%>{poseName}</size>";
        tmp.fontSize = 60;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

       
        tmp.enableWordWrapping = false;
        tmp.overflowMode = TextOverflowModes.Overflow;

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
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
        float windowDepth = judgeWindowSeconds * moveSpeed;
        float halfW = totalWidth / 2f;
        float halfH = wallHeight;

        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(
            transform.position + new Vector3(HolePositionX, 2.2f, 0f),
            new Vector3(holeWidth, wallHeight, wallDepth));

        Gizmos.color = new Color(0f, 1f, 0f, 0.18f);
        Gizmos.DrawCube(
            new Vector3(0f, halfH / 2f, jz - windowDepth / 2f),
            new Vector3(totalWidth, halfH, windowDepth));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz - windowDepth), new Vector3(halfW, 0f, jz - windowDepth));
        Gizmos.DrawLine(new Vector3(-halfW, halfH, jz - windowDepth), new Vector3(halfW, halfH, jz - windowDepth));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz), new Vector3(halfW, 0f, jz));
        Gizmos.DrawLine(new Vector3(-halfW, halfH, jz), new Vector3(halfW, halfH, jz));
        Gizmos.DrawLine(new Vector3(-halfW, 0f, jz), new Vector3(-halfW, halfH, jz));
        Gizmos.DrawLine(new Vector3(halfW, 0f, jz), new Vector3(halfW, halfH, jz));

#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.Label(new Vector3(halfW + 0.3f, 0f, jz), "JUDGE");
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.Label(
            new Vector3(halfW + 0.3f, 0f, jz - windowDepth),
            $"PRESS HERE -{judgeWindowSeconds * 1000f:F0}ms");
#endif
    }
}