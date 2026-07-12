using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [Header("Nombres de las Escenas")]
    [Tooltip("Nombre exacto de la escena del Tutorial en las Build Settings")]
    [SerializeField] private string tutorialSceneName = "Tutorial";
    [Tooltip("Nombre exacto de la escena del Nivel 1 en las Build Settings")]
    [SerializeField] private string level1SceneName = "Nivel1";
    [Tooltip("Nombre exacto de la escena del Menú Principal por si el jugador desea regresar")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string versusSceneName = "Versus";
    [Header("Botones de la UI (Asignación Automática)")]
    [Tooltip("Arrastra aquí el botón de la UI para abrir el tutorial")]
    [SerializeField] private Button tutorialButton;
    [Tooltip("Arrastra aquí el botón de la UI para abrir el Nivel 1")]
    [SerializeField] private Button level1Button;
    [Tooltip("Arrastra aquí el botón de volver atrás (Opcional)")]
    [SerializeField] private Button backButton;

    private void Start()
    {
        // Si asignas los botones directamente en el Inspector, el script les asociará las funciones automáticamente
        if (tutorialButton != null)
            tutorialButton.onClick.AddListener(LoadTutorial);

        if (level1Button != null)
            level1Button.onClick.AddListener(LoadLevel1);

        if (backButton != null)
            backButton.onClick.AddListener(GoBackToMainMenu);
    }

 
    public void LoadTutorial()
    {
        LoadScene(tutorialSceneName);
    }

 
    public void LoadLevel1()
    {
        LoadScene(level1SceneName);
    }

  
    public void GoBackToMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }

    public void LoadVersus()
    {
        LoadScene(versusSceneName);
    }
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[LevelSelector] Intentaste cargar una escena con un nombre vacío o nulo.");
            return;
        }

        Debug.Log($"[LevelSelector] Cargando la escena: '{sceneName}'...");
        SceneManager.LoadScene(sceneName);
    }
}
