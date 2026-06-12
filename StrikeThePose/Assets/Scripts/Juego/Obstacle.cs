using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Partes visuales")]
    [SerializeField] private Transform wallLeft;
    [SerializeField] private Transform wallRight;
    [SerializeField] private BoxCollider holeZoneTrigger;

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

    // Propiedad para marcar si este obstáculo es falso/trampa
    public bool IsFake { get; private set; } = false;

    // ── Notas Largas (Hold Notes) ─────────────────────────────────────────────
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

    // Estados de notas largas
    private bool _isHolding = false;
    private float _holdTimer = 0f;
    private float _pointsTickTimer = 0f;

    // Referencia a las partículas activas durante hold
    private GameObject _activeHoldParticles;

    // Guardamos el último color aplicado para dárselo a las partículas
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

        // Notas largas
        IsHold = holdDuration > 0f;
        HoldDuration = holdDuration;

        // Configuraciones dinámicas de velocidad y juicio
        moveSpeed = speed;
        earlyWindowSeconds = earlyWindow;
        lateWindowSeconds = lateWindow;

        _judgeLineZ = player.transform.position.z;

        // Si es una nota larga, aumentamos la distancia de destrucción según su longitud física
        float physicalLength = IsHold ? (HoldDuration * moveSpeed) : 0f;
        _destroyZ = _judgeLineZ + destroyOffset + physicalLength;
        _windowStartZ = _judgeLineZ - (earlyWindowSeconds * moveSpeed);
        _windowEndZ = _judgeLineZ + (lateWindowSeconds * moveSpeed);

        BuildWall();
        ApplyColor();

        // En obstáculos fake o notas largas complejas no mostramos tutorial de ayuda directo
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

        // ── 1. LÓGICA DE NOTA LARGA ACTIVA ──
        if (_isHolding)
        {
            _holdTimer += Time.deltaTime;

            // Hacer que las partículas del hold sigan al obstáculo mientras se mueve
            if (_activeHoldParticles != null)
            {
                Vector3 followPos = new Vector3(HolePositionX, wallHeight / 2f, transform.position.z) + hitParticleOffset;
                _activeHoldParticles.transform.position = followPos;
            }

            // Comprobar si el jugador sigue en la pose requerida y dentro del hueco
            bool holdingCorrectPose = _player.CurrentPose == RequiredPose;
            bool insideHole = IsPlayerInHole(_player.transform.position.x);

            if (!holdingCorrectPose || !insideHole)
            {
                // El jugador soltó el botón antes o se movió del carril -> FALLO INMEDIATO
                _isHolding = false;
                StopHoldParticles();
                EvaluateResult(false);
                return;
            }

            // Otorga ráfagas de puntos (ticks) cada 0.1 segundos por mantener con éxito
            _pointsTickTimer += Time.deltaTime;
            if (_pointsTickTimer >= 0.1f)
            {
                _pointsTickTimer = 0f;
                GameManager.Instance?.AddBonusPoints(5); // +5 puntos por cada tick
            }

            // Si se completó el tiempo requerido -> ÉXITO COMPLETO!
            if (_holdTimer >= HoldDuration)
            {
                _isHolding = false;
                StopHoldParticles();
                EvaluateResult(true);
            }
            return;
        }

        // ── 2. DETECCIÓN DE ENTRADA A LA VENTANA DE JUICIO ──
        if (!_evaluated && z >= _windowStartZ && z <= _windowEndZ)
        {
            _enteredWindow = true;

            if (IsFake)
            {
                // Si el obstáculo es FAKE, presionar cualquier tecla activa un fallo inmediato!
                if (_player.CurrentPose != PoseType.Idle)
                {
                    EvaluateResult(false);
                }
            }
            else
            {
                // Comportamiento normal para obstáculos reales (y notas largas al iniciar)
                float timeSinceInput = Time.time - _player.LastInputTime;
                bool recentInput = timeSinceInput <= Time.deltaTime * 2f;

                if (recentInput && _player.LastInputPose == RequiredPose)
                {
                    bool posOk = IsPlayerInHole(_player.transform.position.x);
                    if (posOk)
                    {
                        if (IsHold)
                        {
                            // Iniciar estado de retención (Holding) + partículas continuas
                            _isHolding = true;
                            _holdTimer = 0f;
                            _pointsTickTimer = 0f;

                            // ════════════════════════════════════════════
                            //  🎵 Iniciar partículas continuas para hold
                            // ════════════════════════════════════════════
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

        // Salió de la ventana sin que se haya evaluado
        if (!_evaluated && !_isHolding && _enteredWindow && z > _windowEndZ)
        {
            if (IsFake)
            {
                // ¡Si el jugador esquivó con éxito (no presionó nada), cuenta como acierto!
                EvaluateResult(true);
            }
            else
            {
                // Si era un obstáculo real/long y no se presionó en su ventana de inicio -> Fallo
                EvaluateResult(false);
            }
        }

        // ── Destrucción al pasar el destroyZ (solo si NO fue evaluado como acierto) ──
        // Aquí NO se instancian partículas — es el caso de "miss" o paso silencioso
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
            // ══════════════════════════════════════════════════════════
            //  🎉 ACIERTO → Instanciar partículas de éxito
            //     (Solo para notas normales; las hold ya tienen sus partículas)
            // ══════════════════════════════════════════════════════════
            if (!IsHold)
            {
                SpawnHitParticles(looping: false);
            }

            ReleaseHint();
            Destroy(gameObject);
        }

        Debug.Log($"[Obstacle] {(success ? "ACIERTO" : "FALLO")} | Pose: {RequiredPose} | ¿Era Hold?: {IsHold}");
    }

    // ── Spawn de partículas de acierto ────────────────────────────────────────
    //  looping = true  → para notas largas: emisión continua hasta que se pare
    //  looping = false → para notas normales: burst de 1 segundo
    private GameObject SpawnHitParticles(bool looping = false)
    {
        if (hitParticlePrefab == null)
        {
            Debug.LogWarning("[Obstacle] No hay prefab de partículas asignado (hitParticlePrefab).");
            return null;
        }

        // Posición central del hueco (donde estaba el "agujero" que el jugador atravesó)
        Vector3 spawnPos = new Vector3(HolePositionX, wallHeight / 2f, transform.position.z) + hitParticleOffset;

        GameObject particles = Instantiate(hitParticlePrefab, spawnPos, Quaternion.identity);

        // Aplicar escala
        particles.transform.localScale = Vector3.one * hitParticleScale;

        // ── Configurar cada ParticleSystem ──
        ParticleSystem[] psArray = particles.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in psArray)
        {
            var main = ps.main;

            // Tintar con el color del obstáculo
            Color targetColor = _appliedColor;
            targetColor.a = main.startColor.color.a;
            main.startColor = new ParticleSystem.MinMaxGradient(targetColor);

            if (looping)
            {
                // ── Modo hold: emisión continua ──
                main.loop = true;
                // Prewarm = true hace que las partículas aparezcan al instante
                main.prewarm = true;
            }
            else
            {
                // ── Modo normal: burst que se apaga ──
                main.loop = false;
                // Simular hacia adelante para que aparezca instantáneamente
                // (prewarm no funciona bien en runtime con loop=false)
                ps.Simulate(hitParticlePrewarmTime, true, true, false);
            }

            // Asegurarse de que esté reproduciéndose
            ps.Play(true);
        }

        // Auto-destruir las partículas (para hold se destruye manualmente via StopHoldParticles)
        if (!looping)
        {
            Destroy(particles, hitParticleDuration);
        }

        return particles;
    }

    // ── Detener partículas de hold y dejar que se desvanezcan ────────────────
    private void StopHoldParticles()
    {
        if (_activeHoldParticles == null) return;

        ParticleSystem[] psArray = _activeHoldParticles.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in psArray)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        // Destruir el objeto de partículas después de que se desvanezcan
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

        // Si es una nota larga, su grosor físico en Z representa el largo que debe sostenerse
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
            c = new Color(c.r * 0.35f, c.g * 0.35f, c.b * 0.35f, c.a);
        }
        else if (IsHold)
        {
            c = new Color(Mathf.Min(c.r * 1.3f, 1f), Mathf.Min(c.g * 1.3f, 1f), Mathf.Min(c.b * 1.3f, 1f), c.a);
        }

        // Guardamos el color para usarlo en las partículas
        _appliedColor = c;

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
