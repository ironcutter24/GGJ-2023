using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Patterns;

public class GameManager : Singleton<GameManager>
{
    public Vector2 lastCheckPointPos;
    private static GameManager instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
