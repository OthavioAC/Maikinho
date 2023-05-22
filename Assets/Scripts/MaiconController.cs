using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum TipoMovimento
{
    Livre,
    EscaladaVertical,
    EscaladaHorizontal,
    EscaladaOmnidirecional,
    Escondido,
}

public enum Animacao
{
    Idle,
    Correr,
    Equilibrar,
    Pular,
    Cair,
    Aterrissar,
    Escalar
}

public enum Skin
{
    Default,
    EasterEgg
}

public class MaiconController : MonoBehaviour
{
    private Skin skinAtual;
    private float skinTimer = 3f;
    /* Flags */
    private bool isGrounded = false;
    private bool estaAmortecendo = false;
    /* Input Flags */
    private bool estaCorrendo = false;
    private bool botaoPulo = false;
    private bool puloAgendado = false;
    private bool estaPulando = false;
    private bool puloCancelado = false;
    private bool estaInteragindo = false;
    private bool interacaoDisponivel = true;

    private bool estaEquilibrando = false;
    public void SetEstaEquilibrando(bool newEstaEquilibrando)
    {
        estaEquilibrando = newEstaEquilibrando;
    }
    /* Maicon Components */
    private Rigidbody2D corpoMaicon;
    private BoxCollider2D pehMaicon;
    private SpriteRenderer maiconSprite;
    private Animator maiconAnimator;
    /* Y */
    [SerializeField] private float gravidade = 0f;
    [SerializeField] private float pulo = 0f;
    [SerializeField] private float constanteDeDepieri = 0f; // usada para amortecer a queda
    [SerializeField] private float velocidadeDeEscalada = 0f;
    private float movimentoVertical;
    /* X */
    [SerializeField] private float velocidadeBase = 0f;
    [SerializeField] private float aceleracao = 0f;
    [SerializeField] private float multiplicadorAceleracao = 0f;
    private float movimentoHorizontal;
    private float velocidadeAtual = 0f;
    /*  */
    private TipoMovimento TipoMovimentoAtual;
    private Vector2 movimentoFinal;
    private float defaultAnimSpeed = .5f;
    //[SerializeField] private float baseCoyoteTime = 0f;

    private Interagivel objetoInteragivel;
    public void SetObjetoInteragivel(Interagivel novoObjetoInteragivel)
    {
        objetoInteragivel = novoObjetoInteragivel;
    }
    
    public bool GetSpriteFlipX()
    {
        return maiconSprite.flipX;
    }

    private void Start()
    {
        skinAtual = Skin.Default;
        maiconSprite = this.GetComponent<SpriteRenderer>();
        corpoMaicon = this.GetComponent<Rigidbody2D>();
        pehMaicon = this.transform.GetChild(1).GetComponent<BoxCollider2D>();
        TipoMovimentoAtual = TipoMovimento.Livre;
        maiconAnimator = this.GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (Input.GetButton("BRANCO0") || Input.GetButton("BRANCO1")) skinTimer -= Time.deltaTime;
        if (Input.GetButtonUp("BRANCO0") || Input.GetButtonUp("BRANCO1")) skinTimer = 3f;

        if (skinTimer <= 0f)
        {
            skinAtual = skinAtual == Skin.Default ? Skin.EasterEgg : Skin.Default;
            skinTimer = 3f;
        }

        if (Input.GetButtonDown("MENU"))
        {
            SceneManager.LoadScene("Menu");
        }
        // setar se maicon ta correndo ou trotando
        velocidadeAtual = velocidadeBase * (estaCorrendo ? 1.5f : 1f);
        // calcular tempo de atraso para pula apos queda de plataforma
        // deteccao de entrada geral
        movimentoHorizontal = Input.GetAxis("HORIZONTAL0") + Input.GetAxis("HORIZONTAL1");
        movimentoHorizontal = movimentoHorizontal > 1 ? 1 : (movimentoHorizontal < -1 ? -1 : movimentoHorizontal); // cap
        movimentoVertical = Input.GetAxis("VERTICAL0") + Input.GetAxis("VERTICAL1");
        movimentoVertical = movimentoVertical > 1 ? 1 : (movimentoVertical < -1 ? -1 : movimentoVertical); // cap
        estaInteragindo = Input.GetButton("VERDE0") || Input.GetButton("VERDE1");
        interacaoDisponivel = Input.GetButtonUp("VERDE0") || Input.GetButtonUp("VERDE1") ? true : interacaoDisponivel;
        botaoPulo = Input.GetButtonDown("VERMELHO0") || Input.GetButtonDown("VERMELHO1");
        estaCorrendo = Input.GetButton("PRETO0") || Input.GetButton("PRETO1");
        // interacao
        if (objetoInteragivel != null)
        {
            if (objetoInteragivel.GetTipoInteragivel() == TipoInteragivel.Portal && (Input.GetButtonDown("VERTICAL0") || Input.GetButtonDown("VERTICAL1")))
            {
                objetoInteragivel.InteracaoOnButtonDown(this);
            }
            if (interacaoDisponivel && estaInteragindo)
            {
                interacaoDisponivel = false;
                objetoInteragivel.InteracaoOnButtonDown(this);
            }

            if (TipoMovimentoAtual == TipoMovimento.EscaladaHorizontal || TipoMovimentoAtual == TipoMovimento.EscaladaVertical || TipoMovimentoAtual == TipoMovimento.EscaladaOmnidirecional)
            {
                if (TipoMovimentoAtual != TipoMovimento.EscaladaHorizontal && objetoInteragivel.GetTipoInteragivel() == TipoInteragivel.EscalavelHorizontal && movimentoHorizontal != 0)
                {
                    objetoInteragivel.InteracaoOnButtonDown(this);
                }

                if (TipoMovimentoAtual != TipoMovimento.EscaladaVertical && objetoInteragivel.GetTipoInteragivel() == TipoInteragivel.EscalavelVertical && movimentoVertical != 0)
                {
                    objetoInteragivel.InteracaoOnButtonDown(this);
                }

                if (TipoMovimentoAtual != TipoMovimento.EscaladaOmnidirecional && objetoInteragivel.GetTipoInteragivel() == TipoInteragivel.EscalavelOmnidirecional && (movimentoHorizontal != 0 || movimentoVertical != 0))
                {
                    objetoInteragivel.InteracaoOnButtonDown(this);
                }
            }
        }
        // movimento
        switch (TipoMovimentoAtual)
        {
            case TipoMovimento.Livre:
                // atualizacao da direcao da animacao do maicon
                maiconSprite.flipX = movimentoHorizontal < 0 ? true : (movimentoHorizontal > 0 ? false : maiconSprite.flipX);
                // deteccao de entrada do movimento livre
                if (botaoPulo && isGrounded)
                {
                    puloAgendado = true;
                    puloCancelado = false;
                    TocarAnimacao(Animacao.Pular);
                }
                puloCancelado = Input.GetButtonUp("VERMELHO0") || Input.GetButtonUp("VERMELHO1") ? true : puloCancelado;
                // atualizacao da animacao no movimento livre
                if (isGrounded && !estaPulando)
                {
                    maiconAnimator.SetFloat("playbackSpeed", Mathf.Max(defaultAnimSpeed, Mathf.Min(1f, Mathf.Abs(corpoMaicon.velocity.x) / aceleracao)));
                    TocarAnimacao(!estaEquilibrando ? (movimentoHorizontal != 0 ? Animacao.Correr : Animacao.Idle) : Animacao.Equilibrar);
                }
                if (!isGrounded)
                {
                    maiconAnimator.SetFloat("playbackSpeed", defaultAnimSpeed);
                    TocarAnimacao(corpoMaicon.velocity.y < 0 ? Animacao.Cair : Animacao.Pular);
                }
                break;
            case TipoMovimento.EscaladaVertical:
                // atualizacao do movimento de escalada
                this.transform.position += new Vector3(0f, movimentoVertical * velocidadeDeEscalada, 0f) * Time.deltaTime;
                maiconAnimator.SetFloat("playbackSpeed", Mathf.Abs(movimentoVertical) * defaultAnimSpeed);
                if (botaoPulo)
                {
                    puloAgendado = true;
                    puloCancelado = false;
                    TocarAnimacao(Animacao.Pular);
                    InteracaoEscalavelVertical(true);
                }
                break;
            case TipoMovimento.EscaladaHorizontal:
                this.transform.position += new Vector3(movimentoHorizontal * velocidadeDeEscalada, 0f, 0f) * Time.deltaTime;
                maiconAnimator.SetFloat("playbackSpeed", Mathf.Abs(movimentoHorizontal) * defaultAnimSpeed);
                if (botaoPulo)
                {
                    puloAgendado = true;
                    puloCancelado = false;
                    TocarAnimacao(Animacao.Pular);
                    InteracaoEscalavelHorizontal(true);
                }
                break;
            case TipoMovimento.EscaladaOmnidirecional:
                this.transform.position += new Vector3(movimentoHorizontal * velocidadeDeEscalada, movimentoVertical * velocidadeDeEscalada, 0f) * Time.deltaTime;
                maiconAnimator.SetFloat("playbackSpeed", (Mathf.Abs(movimentoHorizontal) + Mathf.Abs(movimentoVertical)) * defaultAnimSpeed);
                if (botaoPulo)
                {
                    puloAgendado = true;
                    puloCancelado = false;
                    TocarAnimacao(Animacao.Pular);
                    InteracaoEscalavelOmnidirecional(true);
                }
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        if (TipoMovimentoAtual == TipoMovimento.Livre)
        {
            /* CALCULO X */
            // x inicial, levando em conta aceleracao
            movimentoFinal.x = movimentoFinal.x + ((movimentoFinal.x < movimentoHorizontal * multiplicadorAceleracao) ? aceleracao : ((movimentoFinal.x > movimentoHorizontal * multiplicadorAceleracao) ? -aceleracao : -(movimentoHorizontal / 10f))) + (movimentoHorizontal / 10f);
            // cap aceleracao
            movimentoFinal.x = movimentoFinal.x > -aceleracao && movimentoFinal.x < aceleracao ? 0 : movimentoFinal.x;
            /* CALCULO Y */
            // y inicial, levando em conta amortecimento
            movimentoFinal.y -= gravidade / (estaAmortecendo ? constanteDeDepieri : 1f);
            estaAmortecendo = false;
            // adicao de altura no pulo se houver movimento horizontal
            if (/*isGrounded &&*/ puloAgendado)
            {
                puloAgendado = false;
                estaPulando = true;
                movimentoFinal.y = pulo + Math.Abs(movimentoFinal.x * velocidadeAtual / 3f);
            }
            // amortecimento do pulo (soltar mais cedo e pular menos)
            if (puloCancelado && movimentoFinal.y > 0)
            {
                puloCancelado = false;
                estaAmortecendo = true;
                movimentoFinal.y /= 2f;
            }
            // ignorar gravidade se nao houver diferencial vertical
            movimentoFinal.y = isGrounded && movimentoFinal.y <= 0 ? -0.01f : movimentoFinal.y;
            // atualizar velocidade do maicon
            corpoMaicon.velocity = new Vector2(movimentoFinal.x * velocidadeAtual, movimentoFinal.y);
        }
    }

    public void TocarAnimacao(Animacao animacao)
    {
        maiconAnimator.Play(Animator.StringToHash("maicon" + skinAtual.ToString() + animacao.ToString()));
    }

    /* METODOS DE INTERACAO */

    public bool InteracaoMudarRua()
    {
        int index = objetoInteragivel.transform.parent.childCount - objetoInteragivel.transform.GetSiblingIndex() - 1;
        this.transform.position = new Vector3(objetoInteragivel.transform.parent.GetChild(index).position.x, this.transform.position.y, 0f);
        return true;
    }
    
    public bool InteracaoEsconder(bool forceReset = false)
    {
        Animator lixeiraAnimation = objetoInteragivel.GetComponent<Animator>();
        if (TipoMovimentoAtual == TipoMovimento.Livre && !forceReset)
        {
            lixeiraAnimation.Play("fecharLixeira");
            TipoMovimentoAtual = TipoMovimento.Escondido;
            corpoMaicon.velocity = Vector2.zero;
            movimentoFinal = Vector2.zero;
            maiconSprite.enabled = false;
            this.transform.position = objetoInteragivel.transform.position;
            return true;
        }
        lixeiraAnimation.Play("abrirLixeira");
        TipoMovimentoAtual = TipoMovimento.Livre;
        corpoMaicon.velocity = Vector2.zero;
        movimentoFinal = Vector2.zero;
        maiconSprite.enabled = true;
        return true;
    }

    public bool InteracaoGrafite()
    {
        SpriteRenderer grafiteSpriteRenderer = objetoInteragivel.GetComponent<SpriteRenderer>();
        if (Core.GetQuantidadeTinta() > 0 && grafiteSpriteRenderer.sprite.name == "GRAFITE_PH_0")
        {
            /* SPLACEHOLDER */
            foreach(Sprite spritePart in Resources.LoadAll<Sprite>("GRAFITE_PH"))
            {
                if (spritePart.name == "GRAFITE_PH_1")
                {
                    grafiteSpriteRenderer.sprite = spritePart;
                    Core.IncrementaQuantidadeTinta(-1);
                }
            }
        }
        return true;
    }

    public bool InteracaoEscalavelVertical(bool forceReset = false)
    {
        if (TipoMovimentoAtual != TipoMovimento.EscaladaVertical && !forceReset)
        {
            PadraoEscalada();
            TipoMovimentoAtual = TipoMovimento.EscaladaVertical;
            this.transform.position = new Vector3(objetoInteragivel.transform.position.x, this.transform.position.y, this.transform.position.z);
            return true;
        }
        if (TipoMovimentoAtual == TipoMovimento.EscaladaVertical)
        {
            PadraoLivre();
            return true;
        }
        return true;
    }

    public bool InteracaoEscalavelHorizontal(bool forceReset = false)
    {
        if (TipoMovimentoAtual != TipoMovimento.EscaladaHorizontal && !forceReset)
        {
            PadraoEscalada();
            TipoMovimentoAtual = TipoMovimento.EscaladaHorizontal;
            this.transform.position = new Vector3(this.transform.position.x, objetoInteragivel.transform.position.y, this.transform.position.z);
            
            return true;
        }
        if (TipoMovimentoAtual == TipoMovimento.EscaladaHorizontal)
        {
            PadraoLivre();
            return true;
        }
        return true;
    }

    public bool InteracaoEscalavelOmnidirecional(bool forceReset = false)
    {
        if (TipoMovimentoAtual != TipoMovimento.EscaladaOmnidirecional && !forceReset)
        {
            PadraoEscalada();
            TipoMovimentoAtual = TipoMovimento.EscaladaOmnidirecional;
            return true;
        }
        if (TipoMovimentoAtual == TipoMovimento.EscaladaOmnidirecional)
        {
            PadraoLivre();
            return true;
        }
        return true;
    }

    /* PADROES DE MOVIMENTO */

    private void PadraoEscalada()
    {
        isGrounded = false;
        maiconSprite.flipX = false;
        corpoMaicon.velocity = Vector2.zero;
        movimentoFinal = Vector2.zero;
        pehMaicon.isTrigger = true;
        TocarAnimacao(Animacao.Escalar);
    }

    private void PadraoLivre()
    {
        TipoMovimentoAtual = TipoMovimento.Livre;
        pehMaicon.isTrigger = false;
        TocarAnimacao(isGrounded ? Animacao.Idle : Animacao.Cair);
    }

    /* COLLIDER */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            TocarAnimacao(Animacao.Aterrissar);
            estaPulando = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = collision.gameObject.CompareTag("Ground") && pehMaicon.transform.position.y - collision.transform.position.y > 0 ? true : isGrounded; // por sleep dps
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }
}
