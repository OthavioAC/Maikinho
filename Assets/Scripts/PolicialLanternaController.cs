using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicialLanternaController : MonoBehaviour
{
    SpriteRenderer copSprite;
    SpriteRenderer lanternaSprite;
    PolicialAI policialAI;

    private void Start()
    {
        copSprite = this.transform.parent.GetComponent<SpriteRenderer>();
        lanternaSprite = this.GetComponent<SpriteRenderer>();
        policialAI = this.GetComponentInParent<PolicialAI>();
    }

    private void Update()
    {
        lanternaSprite.flipX = copSprite.flipX;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if ((!lanternaSprite.flipX && collision.transform.position.x > this.transform.position.x) || (lanternaSprite.flipX && collision.transform.position.x < this.transform.position.x))
            {
                policialAI.SetTarget(collision.transform);
                policialAI.SetState(PolicialAIState.Busca);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && policialAI.GetTarget() == null)
        {
            if ((!lanternaSprite.flipX && collision.transform.position.x > this.transform.position.x) || (lanternaSprite.flipX && collision.transform.position.x < this.transform.position.x))
            {
                policialAI.SetTarget(collision.transform);
                policialAI.SetState(PolicialAIState.Busca);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            policialAI.SetTarget(null);
            policialAI.SetState(PolicialAIState.Idle);
        }
    }
}
