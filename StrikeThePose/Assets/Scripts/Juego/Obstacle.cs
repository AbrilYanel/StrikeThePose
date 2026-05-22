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

    public PoseType RequiredPose { get; private set; }
    public float HolePositionX { get; private set; }

    private bool _evaluated = false;
    private bool _isTutorial = false;
    private bool _tutorialShown = false;
    private PoseController _player;
    private float _judgeLineZ;
    private float _destroyZ;

   
    [Header("Tutorial")]
    [SerializeField] private float tutorialShowDistance = 8f;

    public void Initialize(PoseType requiredPose, float holePosX, PoseController player, bool showTutorial = false)
    {
        RequiredPose = requiredPose;
        HolePositionX = holePosX;
        _player = player;
        _isTutorial = showTutorial;
        _judgeLineZ = player.transform.position.z;
        _destroyZ = _judgeLineZ + destroyOffset;

        BuildWall();
        ApplyColor();
    }

    private void Update()
    {
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;

       
        if (_isTutorial && !_tutorialShown && !_evaluated)
        {
            float distanceToJudge = Mathf.Abs(transform.position.z - _judgeLineZ);
            if (distanceToJudge <= tutorialShowDistance)
            {
              
                if (UIManager.Instance != null && !UIManager.Instance.IsTutorialHintActive)
                {
                    _tutorialShown = true;
                    UIManager.Instance.ShowTutorialHint(RequiredPose);
                }
            }
        }

        if (!_evaluated && transform.position.z >= _judgeLineZ)
            Evaluate();

        if (transform.position.z >= _destroyZ)
        {
            if (_isTutorial && _tutorialShown && UIManager.Instance != null)
                UIManager.Instance.HideTutorialHint();

            Destroy(gameObject);
        }
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

        if (success)
        {
            
            if (_isTutorial && UIManager.Instance != null)
                UIManager.Instance.HideTutorialHint();

            Destroy(gameObject);
        }

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