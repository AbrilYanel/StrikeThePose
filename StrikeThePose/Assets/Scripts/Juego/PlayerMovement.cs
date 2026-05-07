using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento lateral")]
    [Tooltip("Velocidad de seguimiento del mouse (lerp)")]
    [SerializeField] private float followSpeed = 8f;

    [Tooltip("Límites del movimiento en X (ej: -5 a 5)")]
    [SerializeField] private float xMin = -5f;
    [SerializeField] private float xMax = 5f;

    [Header("Referencias")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PoseController poseController;

  
    private float targetX;

 

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (poseController == null)
            poseController = GetComponent<PoseController>();
    }

    private void Update()
    {
        HandleMouseMovement();
        HandlePoseInput();
    }


    // Movimiento con mouse
  
    /// Convierte la posición X del mouse en pantalla a mundo
    /// y mueve el personaje suavemente hacia ese punto.

    private void HandleMouseMovement()
    {
        // Proyectar el mouse al plano del personaje
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);

        // Clampear dentro de los límites
        targetX = Mathf.Clamp(mouseWorld.x, xMin, xMax);

        // Mover suavemente (solo en X, Y y Z no se tocan)
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * followSpeed);
        transform.position = newPos;
    }

   
    // Entrada de poses (teclado)
    // Cada tecla corresponde a una pose.
   
    private void HandlePoseInput()
    {
        if (poseController == null) return;

        if (Input.GetKeyDown(KeyCode.W))
            poseController.SetPose(PoseType.PoseA);
        else if (Input.GetKeyDown(KeyCode.A))
            poseController.SetPose(PoseType.PoseB);
        else if (Input.GetKeyDown(KeyCode.S))
            poseController.SetPose(PoseType.PoseC);
        else if (Input.GetKeyDown(KeyCode.D))
            poseController.SetPose(PoseType.PoseD);

        // Volver a idle al soltar todas las teclas de pose
        if (!Input.GetKey(KeyCode.W) &&
            !Input.GetKey(KeyCode.A) &&
            !Input.GetKey(KeyCode.S) &&
            !Input.GetKey(KeyCode.D))
        {
            poseController.SetPose(PoseType.Idle);
        }
    }

   
    // Gizmos (editor)
  

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((xMin + xMax) / 2f, transform.position.y, transform.position.z);
        Vector3 size = new Vector3(xMax - xMin, 0.1f, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}