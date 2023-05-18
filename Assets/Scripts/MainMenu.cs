using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(1280, 1200, true);
    }

    private void Start()
    {
        Core.Reset();
    }

    private void Update()
    {
        if (Input.GetButtonDown("MENU"))
        {
            SceneManager.LoadScene("Rua");
        }
    }
}
