using UnityEngine;


public class PanelDificultad : MonoBehaviour
{
    [Header("Paneles de controles")]
    [Tooltip("Panel con las instrucciones/controles para dificultad Fácil")]
    [SerializeField] private GameObject easyControlsPanel;
    [Tooltip("Panel con las instrucciones/controles para dificultad Normal y Difícil")]
    [SerializeField] private GameObject normalHardControlsPanel;

    private void Start()
    {
        // Estado inicial: ambos paneles ocultos hasta que se elija una dificultad
        easyControlsPanel?.SetActive(false);
        normalHardControlsPanel?.SetActive(false);
    }

  
    public void OnEasyPressed()
    {
        ShowPanel(easyControlsPanel);
    }

    public void OnNormalPressed()
    {
        ShowPanel(normalHardControlsPanel);
    }

    public void OnHardPressed()
    {
        ShowPanel(normalHardControlsPanel);
    }

    private void ShowPanel(GameObject panelToShow)
    {
        if (easyControlsPanel != null)
            easyControlsPanel.SetActive(panelToShow == easyControlsPanel);

        if (normalHardControlsPanel != null)
            normalHardControlsPanel.SetActive(panelToShow == normalHardControlsPanel);
    }
}