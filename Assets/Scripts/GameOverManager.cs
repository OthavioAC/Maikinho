using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private void Start()
    {
        this.transform.GetChild(0).GetChild((int)Core.GetGameOverState()).gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetButtonDown("MENU"))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
