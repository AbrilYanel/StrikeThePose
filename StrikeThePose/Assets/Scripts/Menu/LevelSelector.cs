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
    [SerializeField] private string level2SceneName = "Nivel2";
    [SerializeField] private string level3SceneName = "Nivel3";
    [Tooltip("Nombre exacto de la escena del Menú Principal por si el jugador desea regresar")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string versus1SceneName = "Versus";
    [SerializeField] private string versus2SceneName = "Versus2";
    [SerializeField] private string versus3SceneName = "Versus3";


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
    public void LoadLevel2()
    {
        LoadScene(level2SceneName);
    }

    public void LoadLevel3()
    {
        LoadScene(level3SceneName);
    }


    public void GoBackToMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }

    public void LoadVersus1()
    {
        LoadScene(versus1SceneName);
    }
    public void LoadVersus2()
    {
        LoadScene(versus2SceneName);
    }
    public void LoadVersus3()
    {
        LoadScene(versus3SceneName);
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
