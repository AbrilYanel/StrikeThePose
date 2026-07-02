using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Partes visuales")]
    [SerializeField] private Transform wallLeft;
    [SerializeField] private Transform wallRight;
    [SerializeField] private BoxCollider holeZoneTrigger;

    [Header("Visual de pose en el hueco")]
    [Tooltip("Prefab que se muestra en el hueco cuando la pose requerida es PoseA / W")]
    [SerializeField] private GameObject poseAPrefab;
    [Tooltip("Prefab que se muestra en el hueco cuando la pose requerida es PoseB / A")]
    [SerializeField] private GameObject poseBPrefab;
    [Tooltip("Prefab que se muestra en el hueco cuando la pose requerida es PoseC / S")]
    [SerializeField] private GameObject poseCPrefab;
    [Tooltip("Prefab que se muestra en el hueco cuando la pose requerida es PoseD / D")]
    [SerializeField] private GameObject poseDPrefab;

    [Header("Visual de poses combinadas")]
    [SerializeField] private GameObject poseABPrefab;
    [SerializeField] private GameObject poseADPrefab;
    [SerializeField] private GameObject poseBCPrefab;
    [SerializeField] private GameObject poseCDPrefab;

    [Header("Ajustes del visual de pose")]
    [Tooltip("Offset local desde el inicio/centro del hueco. X se suma a HolePositionX. Z suele quedar en 0 para marcar el inicio de la nota.")]
    [SerializeField] private Vector3 poseVisualLocalOffset = new Vector3(0f, 2.2f, 0f);
    [Tooltip("Rotación local del prefab de pose. Útil si el FBX mira hacia otro eje.")]
    [SerializeField] private Vector3 poseVisualLocalEuler = Vector3.zero;
    [Tooltip("Escala local multiplicadora para el prefab de pose.")]
    [SerializeField] private float poseVisualScale = 1f;

    [Header("Visual de nota sostenida")]
    [Tooltip("Si está activo, el prefab visual de pose se alarga desde el inicio hasta el final de la nota sostenida.")]
    [SerializeField] private bool stretchPoseVisualOnHold = true;
    [Tooltip("Eje local del prefab que se va a estirar. Usá Z si el FBX se alarga en profundidad; X/Y si tu modelo está orientado distinto.")]
    [SerializeField] private HoldStretchAxis holdStretchAxis = HoldStretchAxis.Z;
    [Tooltip("Ajuste del largo del visual sostenido. 1 = mismo largo físico que la nota. Bajalo/subilo si tu FBX tiene un tamaño base distinto.")]
    [SerializeField] private float holdStretchMultiplier = 1f;

    [Tooltip("Si está activo, el visual de pose se oculta en obstáculos fake para no revelar/trucar información.")]
    [SerializeField] private bool hidePoseVisualOnFake = true;

    private enum HoldStretchAxis
    {
        X,
        Y,
        Z
    }

    private GameObject _poseVisualInstance;

    [Header("Efecto de acierto")]
    [Tooltip("Prefab de partículas que aparece al acertar")]
    [SerializeField] private GameObject hitParticlePrefab;
    [Tooltip("Cuántos segundos dura el efecto antes de destruirse")]
    [SerializeField] private float hitParticleDuration = 1f;
    [Tooltip("Offset extra sobre la posición base del hueco (centro del hole)")]
    [SerializeField] private Vector3 hitParticleOffset = Vector3.zero;
    [Tooltip("Escala del efecto de partículas (1 = tamaño original del prefab)")]
    [SerializeField] private float hitParticleScale = 1f;
    [Tooltip("Cuántos segundos se simulan al instanciar para que aparezca al instante")]
    [SerializeField] private float hitParticlePrewarmTime = 0.15f;

    [Header("Dimensiones")]
    [SerializeField] private float totalWidth = 14f;
    [SerializeField] private float holeWidth = 2.5f;
    [SerializeField] private float wallHeight = 4f;
    [SerializeField] private float wallDepth = 0.5f;

    [Header("Color del obstáculo")]
    [Tooltip("Color neutro de las paredes. La pose ahora se comunica con el prefab del hueco, no con el color.")]
    [SerializeField] private Color wallNeutralColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color fakeWallColor = new Color(0.25f, 0.25f, 0.25f, 1f);
    [SerializeField] private Color holdWallColor = new Color(1f, 1f, 1f, 1f);

    [Header("Colores legacy para partículas")]
    [SerializeField] private Color colorPoseA = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color colorPoseB = new Color(1f, 0.4f, 0.2f);
    [SerializeField] private Color colorPoseC = new Color(0.3f, 0.9f, 0.3f);
    [SerializeField] private Color colorPoseD = new Color(0.9f, 0.2f, 0.8f);
    [SerializeField] private Color colorPoseAB = new Color(0.6f, 0.3f, 1f);
    [SerializeField] private Color colorPoseAD = new Color(1f, 0.9f, 0.1f);
    [SerializeField] private Color colorPoseBC = new Color(0.1f, 0.9f, 0.9f);
    [SerializeField] private Color colorPoseCD = new Color(1f, 0.5f, 0.7f);

    [Header("Ventana de juicio")]
    [Tooltip("Segundos ANTES de la línea en los que se acepta input")]
    [SerializeField] private float earlyWindowSeconds = 0.4f;
    [Tooltip("Segundos DESPUÉS de la línea en los que se acepta input")]
    [SerializeField] private float lateWindowSeconds = 0.3f;

    [Header("Destrucción")]
    [SerializeField] private float destroyOffset = 3f;

    public PoseType RequiredPose { get; private set; }
    public float HolePositionX { get; private set; }
    public bool IsFake { get; private set; } = false;
    public bool IsHold { get; private set; } = false;
    public float HoldDuration { get; private set; } = 0f;

    private bool _evaluated = false;
    private bool _ownsHint = false;
    private bool _enteredWindow = false;

    private PoseController _player;
    private float _judgeLineZ;
    private float _destroyZ;
    private float _windowStartZ;
    private float _windowEndZ;

    private bool _isHolding = false;
    private float _holdTimer = 0f;
    private float _pointsTickTimer = 0f;
    private GameObject _activeHoldParticles;
    private Color _appliedColor = Color.white;

    public void Initialize(
        PoseType requiredPose,
        float holePosX,
        PoseController player,
        float speed,
        float earlyWindow,
        float lateWindow,
        bool showTutorial = false,
        bool isFake = false,
        float holdDuration = 0f
    )
    {
        RequiredPose = requiredPose;
        HolePositionX = holePosX;
        _player = player;
        IsFake = isFake;

        IsHold = holdDuration > 0f;
        HoldDuration = holdDuration;

        moveSpeed = speed;
        earlyWindowSeconds = earlyWindow;
        lateWindowSeconds = lateWindow;

        _judgeLineZ = player.transform.position.z;

        float physicalLength = IsHold ? (HoldDuration * moveSpeed) : 0f;
        _destroyZ = _judgeLineZ + destroyOffset + physicalLength;
        _windowStartZ = _judgeLineZ - (earlyWindowSeconds * moveSpeed);
        _windowEndZ = _judgeLineZ + (lateWindowSeconds * moveSpeed);

        BuildWall();
        ApplyWallColor();
        BuildPoseVisual();

        if (showTutorial && !IsFake && !IsHold && UIManager.Instance != null && !UIManager.Instance.IsTutorialHintActive)
        {
            _ownsHint = true;
            UIManager.Instance.ShowTutorialHint(RequiredPose);
        }
    }

    private void Update()
    {
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
        float z = transform.position.z;

        if (_isHolding)
        {
            _holdTimer += Time.deltaTime;

            if (_activeHoldParticles != null)
            {
                Vector3 followPos = new Vector3(HolePositionX, wallHeight / 2f, transform.position.z) + hitParticleOffset;
                _activeHoldParticles.transform.position = followPos;
            }

            bool holdingCorrectPose = _player.CurrentPose == RequiredPose;
            bool insideHole = IsPlayerInHole(_player.transform.position.x);

            if (!holdingCorrectPose || !insideHole)
            {
                _isHolding = false;
                StopHoldParticles();
                EvaluateResult(false);
                return;
            }

            _pointsTickTimer += Time.deltaTime;
            if (_pointsTickTimer >= 0.1f)
            {
                _pointsTickTimer = 0f;
                GameManager.Instance?.AddBonusPoints(5);
            }

            if (_holdTimer >= HoldDuration)
            {
                _isHolding = false;
                StopHoldParticles();
                EvaluateResult(true);
            }
            return;
        }

        if (!_evaluated && z >= _windowStartZ && z <= _windowEndZ)
        {
            _enteredWindow = true;

            if (IsFake)
            {
                if (_player.CurrentPose != PoseType.Idle)
                {
                    EvaluateResult(false);
                }
            }
            else
            {
                float timeSinceInput = Time.time - _player.LastInputTime;
                bool recentInput = timeSinceInput <= Time.deltaTime * 2f;

                if (recentInput && _player.LastInputPose == RequiredPose)
                {
                    bool posOk = IsPlayerInHole(_player.transform.position.x);
                    if (posOk)
                    {
                        if (IsHold)
                        {
                            _isHolding = true;
                            _holdTimer = 0f;
                            _pointsTickTimer = 0f;
                            _activeHoldParticles = SpawnHitParticles(looping: true);
                            Debug.Log("[Obstacle] ¡Nota larga iniciada! Mantén presionado...");
                        }
                        else
                        {
                            EvaluateResult(true);
                        }
                    }
                    else
                    {
                        EvaluateResult(false);
                    }
                }
            }
        }

        if (!_evaluated && !_isHolding && _enteredWindow && z > _windowEndZ)
        {
            if (IsFake)
            {
                EvaluateResult(true);
            }
            else
            {
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
            if (!IsHold)
            {
                SpawnHitParticles(looping: false);
            }

            ReleaseHint();
            Destroy(gameObject);
        }

        Debug.Log($"[Obstacle] {(success ? "ACIERTO" : "FALLO")} | Pose: {RequiredPose} | ¿Era Hold?: {IsHold}");
    }

    private void BuildPoseVisual()
    {
        if (IsFake && hidePoseVisualOnFake) return;

        GameObject prefab = GetPosePrefab(RequiredPose);
        if (prefab == null)
        {
            Debug.LogWarning($"[Obstacle] No hay prefab visual asignado para la pose {RequiredPose}.");
            return;
        }

        Vector3 localPos = new Vector3(HolePositionX, 0f, 0f) + poseVisualLocalOffset;
        Quaternion localRot = Quaternion.Euler(poseVisualLocalEuler);

        _poseVisualInstance = Instantiate(prefab, transform);
        _poseVisualInstance.transform.localPosition = localPos;
        _poseVisualInstance.transform.localRotation = localRot;
        _poseVisualInstance.transform.localScale = Vector3.one * poseVisualScale;

        if (IsHold && stretchPoseVisualOnHold)
        {
            float holdLength = Mathf.Max(0.01f, HoldDuration * moveSpeed * holdStretchMultiplier);
            FitHoldVisualFromStartToEnd(_poseVisualInstance, localPos.z, holdLength);
        }

        // Evita que un prefab con collider interfiera con el jugador/obstáculo.
        foreach (Collider col in _poseVisualInstance.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }

    private GameObject GetPosePrefab(PoseType pose)
    {
        return pose switch
        {
            PoseType.PoseA => poseAPrefab,
            PoseType.PoseB => poseBPrefab,
            PoseType.PoseC => poseCPrefab,
            PoseType.PoseD => poseDPrefab,
            PoseType.PoseAB => poseABPrefab,
            PoseType.PoseAD => poseADPrefab,
            PoseType.PoseBC => poseBCPrefab,
            PoseType.PoseCD => poseCDPrefab,
            _ => null,
        };
    }

    private void FitHoldVisualFromStartToEnd(GameObject visual, float startZ, float holdLength)
    {
        // Queremos que el visual completo ocupe exactamente este tramo local:
        // inicio = startZ, final = startZ - holdLength.
        float desiredStartZ = startZ;
        float desiredEndZ = startZ - holdLength;
        float desiredCenterZ = (desiredStartZ + desiredEndZ) * 0.5f;
        float desiredDepth = Mathf.Abs(desiredStartZ - desiredEndZ);

        Bounds localBounds = GetLocalBoundsRelativeToObstacle(visual);
        float currentDepth = Mathf.Max(0.0001f, localBounds.size.z);
        float scaleFactor = desiredDepth / currentDepth;

        Vector3 scale = visual.transform.localScale;
        switch (holdStretchAxis)
        {
            case HoldStretchAxis.X:
                scale.x *= scaleFactor;
                break;
            case HoldStretchAxis.Y:
                scale.y *= scaleFactor;
                break;
            case HoldStretchAxis.Z:
                scale.z *= scaleFactor;
                break;
        }
        visual.transform.localScale = scale;

        // Tras escalar, recalculamos bounds reales y corregimos posición.
        // Esto elimina el desfase causado por pivots centrados o meshes desplazados dentro del FBX.
        localBounds = GetLocalBoundsRelativeToObstacle(visual);
        float currentCenterZ = localBounds.center.z;
        Vector3 pos = visual.transform.localPosition;
        pos.z += desiredCenterZ - currentCenterZ;
        visual.transform.localPosition = pos;
    }

    private Bounds GetLocalBoundsRelativeToObstacle(GameObject visual)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(visual.transform.localPosition, Vector3.one);

        Bounds bounds = new Bounds(transform.InverseTransformPoint(renderers[0].bounds.center), Vector3.zero);

        foreach (Renderer renderer in renderers)
        {
            Vector3 worldMin = renderer.bounds.min;
            Vector3 worldMax = renderer.bounds.max;

            bounds.Encapsulate(transform.InverseTransformPoint(new Vector3(worldMin.x, worldMin.y, worldMin.z)));
            bounds.Encapsulate(transform.InverseTransformPoint(new Vector3(worldMin.x, worldMin.y, worldMax.z)));
            bounds.Encapsulate(transform.InverseTransformPoint(new Vector3(worldMin.x, worldMax.y, worldMin.z)));
            bounds.Encapsulate(transform.InverseTransformPoint(new Vector3(worldMin.x, worldMax.y, worldMax.z)));
            bounds.Encapsulate(transform.InverseTransformPoint(new Vector3(worldMax.x, worldMin.y, worldMin.z)));
            bounds.Encapsulate(transform.InverseTransformPoint(new Vector3(worldMax.x, worldMin.y, worldMax.z)));
            bounds.Encapsulate(transform.InverseTransformPoint(new Vector3(worldMax.x, worldMax.y, worldMin.z)));
            bounds.Encapsulate(transform.InverseTransformPoint(new Vector3(worldMax.x, worldMax.y, worldMax.z)));
        }

        return bounds;
    }

    private GameObject SpawnHitParticles(bool looping = false)
    {
        if (hitParticlePrefab == null)
        {
            Debug.LogWarning("[Obstacle] No hay prefab de partículas asignado (hitParticlePrefab).");
            return null;
        }

        Vector3 spawnPos = new Vector3(HolePositionX, wallHeight / 2f, transform.position.z) + hitParticleOffset;
        GameObject particles = Instantiate(hitParticlePrefab, spawnPos, Quaternion.identity);
        particles.transform.localScale = Vector3.one * hitParticleScale;

        ParticleSystem[] psArray = particles.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in psArray)
        {
            var main = ps.main;
            Color targetColor = _appliedColor;
            targetColor.a = main.startColor.color.a;
            main.startColor = new ParticleSystem.MinMaxGradient(targetColor);

            if (looping)
            {
                main.loop = true;
                main.prewarm = true;
            }
            else
            {
                main.loop = false;
                ps.Simulate(hitParticlePrewarmTime, true, true, false);
            }

            ps.Play(true);
        }

        if (!looping)
        {
            Destroy(particles, hitParticleDuration);
        }

        return particles;
    }

    private void StopHoldParticles()
    {
        if (_activeHoldParticles == null) return;

        ParticleSystem[] psArray = _activeHoldParticles.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in psArray)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        Destroy(_activeHoldParticles, hitParticleDuration);
        _activeHoldParticles = null;
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

        float currentDepth = wallDepth;
        if (IsHold)
        {
            currentDepth = HoldDuration * moveSpeed;
        }

        if (wallLeft != null)
        {
            wallLeft.localScale = new Vector3(Mathf.Max(leftWidth, 0f), wallHeight, currentDepth);
            wallLeft.localPosition = new Vector3(leftOrigin + leftWidth / 2f, 2.2f, -currentDepth / 2f);
        }

        if (wallRight != null)
        {
            wallRight.localScale = new Vector3(Mathf.Max(rightWidth, 0f), wallHeight, currentDepth);
            wallRight.localPosition = new Vector3(rightStart + rightWidth / 2f, 2.2f, -currentDepth / 2f);
        }

        if (holeZoneTrigger != null)
        {
            holeZoneTrigger.center = new Vector3(HolePositionX, 2.2f, -currentDepth / 2f);
            holeZoneTrigger.size = new Vector3(holeWidth, wallHeight, currentDepth);
        }
    }

    private void ApplyWallColor()
    {
        // Este color se conserva para partículas. Las paredes ahora son neutras.
        _appliedColor = GetPoseColor(RequiredPose);

        Color wallColor = wallNeutralColor;
        if (IsFake)
        {
            wallColor = fakeWallColor;
        }
        else if (IsHold)
        {
            wallColor = holdWallColor;
        }

        ApplyColorToTransform(wallLeft, wallColor);
        ApplyColorToTransform(wallRight, wallColor);
    }

    private void ApplyColorToTransform(Transform target, Color color)
    {
        if (target == null) return;

        foreach (MeshRenderer mr in target.GetComponentsInChildren<MeshRenderer>())
            mr.material.color = color;
    }

    private Color GetPoseColor(PoseType pose)
    {
        return pose switch
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
    }

    private void OnDrawGizmos()
    {
        float jz = Application.isPlaying ? _judgeLineZ : 0f;
        float earlyD = earlyWindowSeconds * moveSpeed;
        float lateD = lateWindowSeconds * moveSpeed;
        float halfW = totalWidth / 2f;
        float halfH = wallHeight;

        float currentDepth = wallDepth;
        if (IsHold)
        {
            currentDepth = HoldDuration * moveSpeed;
        }

        Gizmos.color = IsFake ? new Color(0.3f, 0.3f, 0.3f, 0.3f) : new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position + new Vector3(HolePositionX, 2.2f, -currentDepth / 2f),
                        new Vector3(holeWidth, wallHeight, currentDepth));

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
        string labelText = IsFake ? "FAKE JUDGE" : (IsHold ? "HOLD START" : "JUDGE");
        UnityEditor.Handles.Label(new Vector3(halfW + 0.3f, 0f, jz), labelText);
#endif
    }
}
