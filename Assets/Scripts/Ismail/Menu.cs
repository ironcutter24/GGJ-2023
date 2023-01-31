using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform player;
    private void Start()
    {
        //Time.timeScale = 0f;
    }
    public void StartGame()
    {
        gameObject.SetActive(false);
        // Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void CommandsList()
    {
        //TODO
    }
}
