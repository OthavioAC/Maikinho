using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoedaAnim : MonoBehaviour
{
    private Image image;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        image = this.GetComponent<Image>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        image.sprite = spriteRenderer.sprite;
    }
}
