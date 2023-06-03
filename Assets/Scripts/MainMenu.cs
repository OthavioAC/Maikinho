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
    private bool animateMenu;
    private bool animationFinished;

    private void Awake()
    {
        Screen.SetResolution(1280, 1200, true);
    }

    private void Start()
    {
        Core.Reset();
        animateMenu = false;
        animationFinished = false;
    }

    private void Update()
    {
        if (!animationFinished && Input.GetButtonDown("MENU"))
        {
            animateMenu = true;
        }

        if (animateMenu)
        {
            if (menuAnimationFrame < menuAnimationTime) { menuAnimationFrame += Time.deltaTime; }
            else
            {
                menuAnimationFrame = menuAnimationTime;
                animateMenu = false;
                animationFinished = true;
            }
        }

        textoInicial.localScale = Vector3.one * (1 - (menuAnimationFrame / menuAnimationTime));
        menuSelector.localScale = Vector3.one * (menuAnimationFrame / menuAnimationTime);

        eixo = Input.GetAxis("VERTICAL0") + Input.GetAxis("VERTICAL1");
        eixo = eixo > 1 ? 1 : (eixo < -1 ? -1 : eixo); // cap
        if (selected && eixo == 0) selected = false;
        if (animationFinished)
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
