using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelectorModeSwitcher : MonoBehaviour
{
    [Header("Botones de selección de modo")]
    [Tooltip("Botón '1 Jugador'")]
    [SerializeField] private GameObject oneJugadorButton;
    [Tooltip("Botón '2 Jugadores'")]
    [SerializeField] private GameObject twoJugadoresButton;

    [Header("Volver al Main Menu")]
    [Tooltip("Nombre de la escena del Main Menu (debe estar agregada en Build Settings)")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Paneles de niveles")]
    [Tooltip("Panel con los botones de nivel para Singleplayer")]
    [SerializeField] private GameObject singlePlayerLevelsPanel;
    [Tooltip("Panel con los botones de nivel para 1v1")]
    [SerializeField] private GameObject versusLevelsPanel;

    private void Start()
    {
        // Estado inicial: solo los botones de modo visibles, ambos paneles ocultos
        ShowModeButtons();
    }

   
    public void OnOneJugadorPressed()
    {
        HideModeButtons();

        if (singlePlayerLevelsPanel != null) singlePlayerLevelsPanel.SetActive(true);
        if (versusLevelsPanel != null) versusLevelsPanel.SetActive(false);
    }

  
    public void OnTwoJugadoresPressed()
    {
        HideModeButtons();

        if (versusLevelsPanel != null) versusLevelsPanel.SetActive(true);
        if (singlePlayerLevelsPanel != null) singlePlayerLevelsPanel.SetActive(false);
    }

    
    public void OnBackToModeSelectionPressed()
    {
        if (singlePlayerLevelsPanel != null) singlePlayerLevelsPanel.SetActive(false);
        if (versusLevelsPanel != null) versusLevelsPanel.SetActive(false);

        ShowModeButtons();
    }

  
    public void OnBackToMainMenuPressed()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void HideModeButtons()
    {
        if (oneJugadorButton != null) oneJugadorButton.SetActive(false);
        if (twoJugadoresButton != null) twoJugadoresButton.SetActive(false);
    }

    private void ShowModeButtons()
    {
        if (oneJugadorButton != null) oneJugadorButton.SetActive(true);
        if (twoJugadoresButton != null) twoJugadoresButton.SetActive(true);

        if (singlePlayerLevelsPanel != null) singlePlayerLevelsPanel.SetActive(false);
        if (versusLevelsPanel != null) versusLevelsPanel.SetActive(false);
    }
}