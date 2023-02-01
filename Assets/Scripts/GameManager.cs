using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utility.Patterns;

public class GameManager : Singleton<GameManager>
{
    public static UnityEvent<bool> KeyBoard_Controller = new UnityEvent<bool>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
