using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoInteragivel
{
    EscalavelVertical,
    EscalavelHorizontal,
    EscalavelOmnidirecional,
    Tinta,
    Moeda,
    Grafitavel,
    Portal,
    Esconderijo,
    Barzin
}

public class Interagivel : MonoBehaviour
{
    /* Propriedades */
    [SerializeField] private TipoInteragivel tipoInteragivel;
    [SerializeField] private CorTinta corTinta;

    public TipoInteragivel GetTipoInteragivel()
    {
        return tipoInteragivel;
    }

    public CorTinta GetCorTinta()
    {
        return corTinta;
    }

    private void Start()
    {
        if (tipoInteragivel == TipoInteragivel.Tinta)
        {
            switch(corTinta)
            { // mudar sprite aki
                case CorTinta.VERMELHA:
                    break;
                case CorTinta.AMARELA:
                    break;
                case CorTinta.VERDE:
                    this.GetComponent<SpriteRenderer>().color = Color.green; // placeholder
                    break;
                case CorTinta.CIANO:
                    break;
                case CorTinta.AZUL:
                    break;
                case CorTinta.MAGENTA:
                    break;
                default:
                    break;
            }
        }
    }

    public bool InteracaoOnTriggerEnter(MaiconController maicon)
    {
        switch (tipoInteragivel)
        {
            case TipoInteragivel.Tinta:
                Core.IncrementaQuantidadeTinta(corTinta, 1);
                GameObject.Destroy(this.gameObject);
                return true;
            case TipoInteragivel.Grafitavel: return Core.SetIndicadorGrafite(true);
            case TipoInteragivel.Portal:
                this.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                return true;
            default:
                return false;
        }
    }

    public bool InteracaoOnTriggerExit(MaiconController maicon)
    {
        switch (tipoInteragivel)
        {
            case TipoInteragivel.EscalavelVertical: return maicon.InteracaoEscalavelVertical(true);
            case TipoInteragivel.EscalavelHorizontal: return maicon.InteracaoEscalavelHorizontal(true);
            case TipoInteragivel.EscalavelOmnidirecional: return maicon.InteracaoEscalavelOmnidirecional(true);
            case TipoInteragivel.Grafitavel: return Core.SetIndicadorGrafite(false);
            case TipoInteragivel.Portal:
                this.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().enabled = false;
                return true;
            default: return false;
        }
    }

    public bool InteracaoOnButtonDown(MaiconController maicon)
    {
        switch (tipoInteragivel)
        {
            case TipoInteragivel.EscalavelVertical: return maicon.InteracaoEscalavelVertical();
            case TipoInteragivel.EscalavelHorizontal: return maicon.InteracaoEscalavelHorizontal();
            case TipoInteragivel.EscalavelOmnidirecional: return maicon.InteracaoEscalavelOmnidirecional();
            case TipoInteragivel.Grafitavel: return maicon.InteracaoGrafite();
            case TipoInteragivel.Portal: return maicon.InteracaoMudarRua();
            case TipoInteragivel.Esconderijo: return maicon.InteracaoEsconder();
            case TipoInteragivel.Barzin: return maicon.InteracaoBarzin();
            default: return false;
        }
    }
}
