using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicialLanternaController : MonoBehaviour
{
    SpriteRenderer spritePolicial;
    SpriteRenderer spriteLanterna;
    PolicialAI policialAI;

    private void Start()
    {
        spritePolicial = this.transform.parent.GetComponent<SpriteRenderer>();
        spriteLanterna = this.GetComponent<SpriteRenderer>();
        policialAI = this.GetComponentInParent<PolicialAI>();
    }

    private void Update()
    {
        spriteLanterna.flipX = spritePolicial.flipX;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if ((!spriteLanterna.flipX && collision.transform.position.x > this.transform.position.x) || (spriteLanterna.flipX && collision.transform.position.x < this.transform.position.x))
            {
                policialAI.SetObjetoAlvo(collision.transform);
                policialAI.SetState(PolicialAIState.Ataque);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && policialAI.GetObjetoAlvo() == null)
        {
            if ((!spriteLanterna.flipX && collision.transform.position.x > this.transform.position.x) || (spriteLanterna.flipX && collision.transform.position.x < this.transform.position.x))
            {
                policialAI.SetObjetoAlvo(collision.transform);
                policialAI.SetState(PolicialAIState.Ataque);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            policialAI.SetObjetoAlvo(null);
            policialAI.SetState(PolicialAIState.Patrulha);
        }
    }
}
