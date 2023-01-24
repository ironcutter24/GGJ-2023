using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Patterns;

public class GameManager : Singleton<GameManager>
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
