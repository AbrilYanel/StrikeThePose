using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    public GameObject panel;
    public void SiguienteEscena()
    {
        SceneManager.LoadScene("SelectorNivel");
    }

    public void CerrarJuego()
    {
        Application.Quit();
    }

    public void ShowCredits(string credits)
    {
        panel.SetActive(true);
    }

    public void CloseCredits(string credits)
    {
        panel.SetActive(false);
    }
}
