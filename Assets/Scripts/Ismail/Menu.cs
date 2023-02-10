using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.LoadLevelScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
