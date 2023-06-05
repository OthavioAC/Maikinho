using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicialColliderController : MonoBehaviour
{
    private BoxCollider2D copCollider;
    private PolicialAI copAI;

    private AudioClip hitDamage;
    private AudioClip stunDamage;

    private void Start()
    {
        copCollider = this.transform.parent.GetChild(0).GetComponent<BoxCollider2D>();
        copAI = this.GetComponentInParent<PolicialAI>();

        hitDamage = Resources.Load<AudioClip>("Audio/Dano");
        stunDamage = Resources.Load<AudioClip>("Audio/Taser");
    }
    /* AKI VAO OS ONTRIGGER E ONCOLLISION PRA DETECTAR CONTATO COM CASAS E COM O MAIKIN */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.StartsWith("Ground"))
        {
            copAI.isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            if (copAI.HitPlayer(1))
            {
                this.GetComponent<AudioSource>().PlayOneShot(copAI.GetTipoPolicial() == TipoPolicial.Civil ? hitDamage : stunDamage);
            }
        }
        if (collision.gameObject.CompareTag("Casa"))
        {
            copAI.SetEscalavel(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.StartsWith("Ground"))
        {
            copAI.isGrounded = false;
            if (this.transform.position.y > collision.transform.position.y)
            {
                copCollider.isTrigger = false;
                copAI.SetMovimento(TipoMovimento.Livre);
            }
        }
        if (collision.gameObject.CompareTag("Casa"))
        {
            copCollider.isTrigger = false;
            copAI.SetEscalavel(null);
            copAI.SetMovimento(TipoMovimento.Livre);
        }
    }
}
