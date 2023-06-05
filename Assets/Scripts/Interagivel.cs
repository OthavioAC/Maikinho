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
    [SerializeField] private int[] custoTinta = { 0, 0, 0, 0, 0, 0 };

    public TipoInteragivel GetTipoInteragivel()
    {
        return tipoInteragivel;
    }

    public CorTinta GetCorTinta()
    {
        return corTinta;
    }

    public bool SetCorTinta(CorTinta novaCor)
    {
        corTinta = novaCor;
        return true;
    }

    public int[] GetCustoTinta()
    {
        return custoTinta;
    }

    private void Start()
    {
        if (tipoInteragivel == TipoInteragivel.Tinta)
        {
            switch(corTinta)
            { // mudar sprite aki
                case CorTinta.VERMELHA:
                    this.GetComponent<SpriteRenderer>().color = Color.red; // placeholder
                    break;
                case CorTinta.AMARELA:
                    this.GetComponent<SpriteRenderer>().color = Color.yellow; // placeholder
                    break;
                case CorTinta.VERDE:
                    this.GetComponent<SpriteRenderer>().color = Color.green; // placeholder
                    break;
                case CorTinta.CIANO:
                    this.GetComponent<SpriteRenderer>().color = Color.cyan; // placeholder
                    break;
                case CorTinta.AZUL:
                    this.GetComponent<SpriteRenderer>().color = Color.blue; // placeholder
                    break;
                case CorTinta.MAGENTA:
                    this.GetComponent<SpriteRenderer>().color = Color.magenta; // placeholder
                    break;
                default:
                    this.GetComponent<SpriteRenderer>().color = Color.black; // placeholder
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
                this.GetComponent<AudioSource>().Play();
                this.GetComponent<SpriteRenderer>().enabled = false;
                this.GetComponent<BoxCollider2D>().enabled = false;
                return true;
            case TipoInteragivel.Grafitavel:
                Core.SetIndicadorGrafite(true, custoTinta);
                return false;
            case TipoInteragivel.Barzin:
                Core.SetIndicadorBarzin(true);
                return false;
            case TipoInteragivel.Portal:
                if (maicon.primeiroGrafite) return false;
                this.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                return false;
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
            case TipoInteragivel.Grafitavel: return Core.SetIndicadorGrafite(false, custoTinta);
            case TipoInteragivel.Barzin: return Core.SetIndicadorBarzin(false);
            case TipoInteragivel.Portal:
                if (maicon.primeiroGrafite) return false;
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
            case TipoInteragivel.Grafitavel:
                if (maicon.InteracaoGrafite())
                {
                    this.GetComponent<AudioSource>().Play();
                    return true;
                }
                return false;
            case TipoInteragivel.Portal: return maicon.InteracaoMudarRua();
            case TipoInteragivel.Esconderijo: return maicon.InteracaoEsconder();
            case TipoInteragivel.Barzin: return maicon.InteracaoBarzin();
            default: return false;
        }
    }
}
