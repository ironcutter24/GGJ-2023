using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerReveal : MonoBehaviour
{
    [SerializeField]
    private Sprite xbox;
    [SerializeField]
    private Sprite keyboard;
    [SerializeField]
    private Image button;



    void Start()
    {
        GameManager.KeyBoard_Controller.AddListener(ChangeController);
    }
    private void OnEnable()
    {
        GameManager.KeyBoard_Controller.AddListener(ChangeController);
    }

    private void OnDisable()
    {
        GameManager.KeyBoard_Controller.RemoveListener(ChangeController);
    }

    void Update()
    {
        
    }

    private void ChangeController(bool controller)
    {
        if (controller)
        {
            button.sprite = xbox;
        }
        else
        {
            button.sprite = keyboard;
        }
    }

}
