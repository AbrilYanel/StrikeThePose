using UnityEngine;

/// <summary>
/// Mueve al jugador lateralmente siguiendo el mouse.
/// La detección de poses se delega completamente a PoseController.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento lateral")]
    [Tooltip("Velocidad de seguimiento del mouse (lerp)")]
    [SerializeField] private float followSpeed = 8f;

    [Tooltip("Límites del movimiento en X")]
    [SerializeField] private float xMin = -4f;
    [SerializeField] private float xMax = 4f;

    [Header("Referencias")]
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleMouseMovement();
        // ⚠️ Las poses son manejadas exclusivamente por PoseController.Update()
    }

    /// <summary>
    /// Proyecta la posición X del mouse al plano del personaje
    /// y lo mueve suavemente hacia ese punto.
    /// </summary>
    private void HandleMouseMovement()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);

        float targetX = Mathf.Clamp(mouseWorld.x, xMin, xMax);

        Vector3 newPos = transform.position;
        newPos.x = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * followSpeed);
        transform.position = newPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((xMin + xMax) / 2f, transform.position.y, transform.position.z);
        Gizmos.DrawWireCube(center, new Vector3(xMax - xMin, 0.1f, 0.1f));
    }
}