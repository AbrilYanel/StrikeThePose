using UnityEngine;

public enum PoseType
{
    Idle,
    // ── Poses simples ──────────────────────────────
    PoseA,   // W
    PoseB,   // A
    PoseC,   // S
    PoseD,   // D
    // ── Poses combinadas ──────────────────────────
    PoseAB,  // W + A
    PoseAD,  // W + D
    PoseBC,  // A + S
    PoseCD,  // S + D
}

public class PoseController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Input")]
    [Tooltip("Componente que implementa IPlayerInputSource, por ejemplo KeyboardInputSource.")]
    [SerializeField] private MonoBehaviour inputSourceComponent;

    [Header("Estado del jugador")]
    [SerializeField] private GameManager gameManager;

    [Header("Configuración del Bonus")]
    [Tooltip("Tiempo mínimo en segundos que debe pasar entre pulsaciones puntuables del Área Bonus (evita spam descontrolado)")]
    [SerializeField] private float bonusInputCooldown = 0.15f;

    public PoseType CurrentPose { get; private set; } = PoseType.Idle;
    public float LastInputTime { get; private set; } = -999f;
    public PoseType LastInputPose { get; private set; } = PoseType.Idle;

    // Estado de teclas del frame anterior (para detectar cambios)
    private bool _prevW, _prevA, _prevS, _prevD;

    // Guarda el timestamp del último bonus otorgado
    private float _lastBonusAwardTime = -999f;

    private IPlayerInputSource _inputSource;

    private void Awake()
    {
        _inputSource = inputSourceComponent as IPlayerInputSource;

        // Fallback: busca una fuente de input en el mismo GameObject.
        if (_inputSource == null)
        {
            MonoBehaviour[] components = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component is IPlayerInputSource source)
                {
                    inputSourceComponent = component;
                    _inputSource = source;
                    break;
                }
            }
        }

        if (_inputSource == null)
        {
            Debug.LogError(
                $"[PoseController] '{name}' no tiene una fuente que implemente IPlayerInputSource.",
                this
            );
            enabled = false;
        }
    }

    private void Start()
    {
        if (animator != null)
            animator.applyRootMotion = false;
    }

    private void Update()
    {
        bool w = _inputSource.PoseA();
        bool a = _inputSource.PoseB();
        bool s = _inputSource.PoseC();
        bool d = _inputSource.PoseD();

        // Detectar si alguna tecla relevante cambió este frame
        bool changed = (w != _prevW) || (a != _prevA) || (s != _prevS) || (d != _prevD);
        if (changed)
        {
            PoseType newPose = ResolvePose(w, a, s, d);
            SetPose(newPose);
        }

        _prevW = w;
        _prevA = a;
        _prevS = s;
        _prevD = d;
    }

    /// <summary>
    /// Resuelve qué pose corresponde a la combinación de teclas activas.
    /// Las combinaciones tienen prioridad sobre las simples.
    /// </summary>
    private PoseType ResolvePose(bool w, bool a, bool s, bool d)
    {
        // ── Combinadas (evaluar primero) ───────────
        if (w && a) return PoseType.PoseAB;
        if (w && d) return PoseType.PoseAD;
        if (a && s) return PoseType.PoseBC;
        if (s && d) return PoseType.PoseCD;

        // ── Simples ────────────────────────────────
        if (w) return PoseType.PoseA;
        if (a) return PoseType.PoseB;
        if (s) return PoseType.PoseC;
        if (d) return PoseType.PoseD;

        return PoseType.Idle;
    }

    /// <summary>
    /// Establece la pose activa y registra el timestamp de input.
    /// </summary>
    public void SetPose(PoseType pose)
    {
        PoseType oldPose = CurrentPose;
        CurrentPose = pose;
        LastInputPose = pose;
        LastInputTime = Time.time;

        // 💥 ÁREA BONUS: Premiamos los cambios de pose si estamos en zona de bonus y pasó el cooldown
        if (gameManager != null && gameManager.IsInBonusArea)
        {
            // Solo premiamos si el jugador entra en una pose de juego válida y ha cambiado respecto a la anterior
            if (pose != PoseType.Idle && pose != oldPose)
            {
                // Aplicamos el Cooldown de seguridad
                if (Time.time - _lastBonusAwardTime >= bonusInputCooldown)
                {
                    gameManager.AddBonusPoints(20); // Otorga los puntos
                    _lastBonusAwardTime = Time.time;         // Registra el tiempo actual
                }
                else
                {
                    Debug.Log("[PoseController] Pulsación ignorada por Cooldown del Bonus.");
                }
            }
        }

        if (animator != null)
        {
            // Resetear todos los triggers para evitar acumulación
            animator.ResetTrigger("PoseA");
            animator.ResetTrigger("PoseB");
            animator.ResetTrigger("PoseC");
            animator.ResetTrigger("PoseD");
            animator.ResetTrigger("PoseAB");
            animator.ResetTrigger("PoseAD");
            animator.ResetTrigger("PoseBC");
            animator.ResetTrigger("PoseCD");
            animator.ResetTrigger("Idle");

            if (pose != PoseType.Idle)
                animator.SetTrigger(pose.ToString());
            else
                animator.SetTrigger("Idle");
        }
        Debug.Log($"[Player] Pose: {pose} en t={Time.time:F2}");
    }

    public bool MatchesPose(PoseType required) => CurrentPose == required;
}