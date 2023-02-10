using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utility.Patterns;

public class GameManager : Singleton<GameManager>
{
    public static UnityEvent<bool> KeyboardOrController = new UnityEvent<bool>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMenuScene();
        }
    }

    public void LoadLevelScene()
    {
        SceneManager.LoadScene("LEVEL_SCENE");
        AudioManager.Instance.GoToLevel();
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("Start_Scene");
        AudioManager.Instance.GoToMenu();
    }
}
