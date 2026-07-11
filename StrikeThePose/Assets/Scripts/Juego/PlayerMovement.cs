using UnityEngine;

/// <summary>
/// Mueve al jugador lateralmente utilizando una fuente de input configurable.
/// La detección de poses se delega completamente a PoseController.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Componente que implementa IPlayerInputSource, por ejemplo KeyboardInputSource.")]
    [SerializeField] private MonoBehaviour inputSourceComponent;

    [Header("Movimiento lateral")]
    [SerializeField] private float moveSpeed = 8f;
    [Tooltip("Límites locales del movimiento en X")]
    [SerializeField] private float xMin = -4f;
    [SerializeField] private float xMax = 4f;

    private IPlayerInputSource _inputSource;

    private void Awake()
    {
        ResolveInputSource();
    }

    private void ResolveInputSource()
    {
        _inputSource = inputSourceComponent as IPlayerInputSource;

        // Fallback útil para no dejar el jugador roto si todavía no se asignó
        // el componente manualmente en el Inspector.
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
                $"[PlayerMovement] '{name}' no tiene una fuente que implemente IPlayerInputSource.",
                this
            );
            enabled = false;
        }
    }

    private void Update()
    {
        float direction = 0f;

        if (_inputSource.MoveLeft())
            direction -= 1f;

        if (_inputSource.MoveRight())
            direction += 1f;

        Vector3 newPos = transform.localPosition;
        newPos.x += direction * moveSpeed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, xMin, xMax);
        transform.localPosition = newPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 localCenter = new Vector3(
            (xMin + xMax) * 0.5f,
            transform.localPosition.y,
            transform.localPosition.z
        );

        Vector3 worldCenter = transform.parent != null
            ? transform.parent.TransformPoint(localCenter)
            : localCenter;

        Gizmos.DrawWireCube(
            worldCenter,
            new Vector3(xMax - xMin, 0.1f, 0.1f)
        );
    }
}
