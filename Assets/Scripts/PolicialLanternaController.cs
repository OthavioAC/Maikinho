using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicialLanternaController : MonoBehaviour
{
    SpriteRenderer copSprite;
    SpriteRenderer lanternaSprite;

    private void Start()
    {
        copSprite = this.transform.parent.GetComponent<SpriteRenderer>();
        lanternaSprite = this.GetComponent<SpriteRenderer>();
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
                Debug.Log("maikin on");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if ((!lanternaSprite.flipX && collision.transform.position.x > this.transform.position.x) || (lanternaSprite.flipX && collision.transform.position.x < this.transform.position.x))
            {
                Debug.Log("maikin off");
            }
        }
    }
}
