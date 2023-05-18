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
        if (Input.GetKeyDown(KeyCode.E))
        {
            Core.MudarRua("Rua");
        }
    }
}
