using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private MaiconController maicon;
    private Interagivel objetoInteragivel;

    private void Start()
    {
        maicon = this.GetComponentInParent<MaiconController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objetoInteragivel = collision.gameObject.GetComponent<Interagivel>();
        if (objetoInteragivel != null && !objetoInteragivel.InteracaoOnTriggerEnter(maicon))
        {
            maicon.SetObjetoInteragivel(objetoInteragivel);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        objetoInteragivel = collision.gameObject.GetComponent<Interagivel>();
        if (objetoInteragivel != null)
        {
            maicon.SetObjetoInteragivel(objetoInteragivel);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objetoInteragivel = collision.gameObject.GetComponent<Interagivel>();
        if (objetoInteragivel != null)
        {
            objetoInteragivel.InteracaoOnTriggerExit(maicon);
            maicon.SetObjetoInteragivel(null);
        }
    }
}
