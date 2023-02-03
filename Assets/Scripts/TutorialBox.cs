using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class TutorialBox : MonoBehaviour
{
    [SerializeField]
    private GameObject keyboardButton, controllerButton;
    [SerializeField]
    private Sprite[] images;
    [SerializeField]
    int input_controller, input_keyboard;


    void Start()
    {
        keyboardButton.GetComponent<Image>().sprite = images[input_keyboard];
        controllerButton.GetComponent<Image>().sprite = images[input_controller];
    }

    private void OnEnable()
    {
        GameManager.KeyboardOrController.AddListener(SwapComand);   
    }

    private void OnDisable()
    {
        GameManager.KeyboardOrController.RemoveListener(SwapComand);
    }

    void Update()
    {
    }

    private void SwapComand(bool isController)
    {
        if (isController)
        {
            controllerButton.SetActive(true);
            keyboardButton.SetActive(false);
        }
        else
        {
            controllerButton.SetActive(false);
            keyboardButton.SetActive(true);
        }
    }
}
