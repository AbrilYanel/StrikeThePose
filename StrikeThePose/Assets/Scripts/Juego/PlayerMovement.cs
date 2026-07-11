using UnityEngine;

/// <summary>
/// Mueve al jugador lateralmente siguiendo el mouse.
/// La detección de poses se delega completamente a PoseController.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private MonoBehaviour inputSourceComponent;

    [Header("Movimiento lateral")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float xMin = -4f;
    [SerializeField] private float xMax = 4f;

    private IPlayerInputSource _inputSource;

    private void Awake()
    {
        _inputSource = inputSourceComponent as IPlayerInputSource;

        if (_inputSource == null)
        {
            Debug.LogError(
                $"[{name}] El componente de input no implementa IPlayerInputSource.",
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

        Vector3 position = transform.localPosition;
        position.x += direction * moveSpeed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, xMin, xMax);
        transform.localPosition = position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 center = transform.parent != null
            ? transform.parent.TransformPoint(
                new Vector3((xMin + xMax) * 0.5f, transform.localPosition.y, transform.localPosition.z)
            )
            : new Vector3((xMin + xMax) * 0.5f, transform.position.y, transform.position.z);

        Gizmos.DrawWireCube(
            center,
            new Vector3(xMax - xMin, 0.1f, 0.1f)
        );
    }
}
