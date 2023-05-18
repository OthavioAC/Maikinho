using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Core.Reset();
    }

    private void Update()
    {
        if (Input.GetButtonDown("MENU"))
        {
            Core.MudarRua("Rua");
        }
    }
}
