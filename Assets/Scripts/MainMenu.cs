using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform menuSelector;
    [SerializeField] private Transform textoInicial;
    private float menuAnimationFrame = 0f;
    private float menuAnimationTime = 0.3f;
    private bool selected = false;
    private float eixo;

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
        if (Input.GetButton("MENU"))
        {
            if (menuAnimationFrame < menuAnimationTime) { menuAnimationFrame += Time.deltaTime; }
            else menuAnimationFrame = menuAnimationTime;
        }
        else
        {
            if (menuAnimationFrame > 0f) { menuAnimationFrame -= Time.deltaTime; }
            else menuAnimationFrame = 0f;
        }

        textoInicial.localScale = Vector3.one * (1 - (menuAnimationFrame / menuAnimationTime));
        menuSelector.localScale = Vector3.one * (menuAnimationFrame / menuAnimationTime);

        eixo = Input.GetAxis("VERTICAL0") + Input.GetAxis("VERTICAL1");
        eixo = eixo > 1 ? 1 : (eixo < -1 ? -1 : eixo); // cap
        if (selected && eixo == 0) selected = false;
        if (menuAnimationFrame / menuAnimationTime == 1)
        {
            if (eixo >= 1 && !selected)
            {
                selected = true;
                SceneManager.LoadScene("Rua");
            }
            if (eixo <= -1 && !selected)
            {
                selected = true;
                Debug.Log("SAI DO JOOJ");
            }
        }
    }
}
