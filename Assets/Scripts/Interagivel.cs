using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoInteragivel
{
    EscalavelVertical,
    EscalavelHorizontal,
    EscalavelOmnidirecional,
    Coletavel,
    Grafitavel,
    Portal,
    Esconderijo,
    Barzin
}

public class Interagivel : MonoBehaviour
{
    [SerializeField] private TipoInteragivel tipoInteragivel;

    public TipoInteragivel GetTipoInteragivel()
    {
        return tipoInteragivel;
    }

    public bool InteracaoOnTriggerEnter(MaiconController maicon)
    {
        switch (tipoInteragivel)
        {
            case TipoInteragivel.Coletavel:
                Core.IncrementaQuantidadeTinta(1);
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
