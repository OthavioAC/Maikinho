using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoMovimento
{
    Livre,
    EscaladaVertical,
    EscaladaHorizontal,
    EscaladaOmnidirecional,
    Escondido,
}

public class MaiconController : MonoBehaviour
{
    /* Flags */
    private bool isGrounded = false;
    //private bool interactionAvlb = false;
    private bool estaAmortecendo = false;
    /* Input Flags */
    private bool estaCorrendo = false;
    private bool puloAgendado = false;
    private bool estaPulando = false;
    private bool puloCancelado = false;
    /* Maicon Components */
    private Rigidbody2D corpoMaicon;
    private BoxCollider2D pehMaicon;
    private SpriteRenderer maiconSprite;
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
    [SerializeField] private float baseCoyoteTime = 0f;

    private Interagivel objetoInteragivel;
    public void SetObjetoInteragivel(Interagivel novoObjetoInteragivel)
    {
        objetoInteragivel = novoObjetoInteragivel;
    }

    private float defaultAnimSpeed = .5f;

    private Vector2 movimentoFinal;

    private Animator animator;

    private bool estaInteragindo;

    private void Start()
    {
        maiconSprite = this.GetComponent<SpriteRenderer>();
        corpoMaicon = this.GetComponent<Rigidbody2D>();
        pehMaicon = this.transform.GetChild(1).GetComponent<BoxCollider2D>();
        TipoMovimentoAtual = TipoMovimento.Livre;
        animator = this.GetComponent<Animator>();
    }
    
    private void Update()
    {
        // setar se maicon ta correndo ou trotando
        velocidadeAtual = velocidadeBase * (estaCorrendo ? 2f : 1f);
        // calcular tempo de atraso para pula apos queda de plataforma
        // deteccao de entrada geral
        movimentoHorizontal = Input.GetAxis("HORIZONTAL0");
        movimentoVertical = Input.GetAxis("VERTICAL0");
        estaInteragindo = Input.GetButtonDown("PRETO0");
        // movimento
        if (estaInteragindo && objetoInteragivel != null)
        {
            objetoInteragivel.InteracaoOnButtonDown(this);
        }
        switch (TipoMovimentoAtual)
        {
            case TipoMovimento.Livre:
                // atualizacao da direcao da animacao do maicon
                maiconSprite.flipX = movimentoHorizontal < 0 ? true : (movimentoHorizontal > 0 ? false : maiconSprite.flipX);
                // deteccao de entrada do movimento livre
                estaCorrendo = Input.GetButton("VERMELHO0");
                if (Input.GetButtonDown("AZUL0") && isGrounded)
                {
                    puloAgendado = true;
                    puloCancelado = false;
                    animator.Play(Animator.StringToHash("maiconPular"));
                }
                puloCancelado = Input.GetButtonUp("AZUL0") ? true : puloCancelado;
                // atualizacao da animacao no movimento livre
                if (isGrounded && !estaPulando)
                {
                    animator.SetFloat("playbackSpeed", Mathf.Max(defaultAnimSpeed, Mathf.Min(1f, Mathf.Abs(corpoMaicon.velocity.x) / aceleracao)));
                    animator.Play(Animator.StringToHash(movimentoHorizontal != 0 ? "maiconCorrer" : "maiconIdle"));
                }
                if (!isGrounded)
                {
                    animator.SetFloat("playbackSpeed", defaultAnimSpeed);
                    animator.Play(Animator.StringToHash(corpoMaicon.velocity.y < 0 ? "maiconCair" : "maiconPular"));
                }
                break;
            case TipoMovimento.EscaladaVertical:
                // atualizacao do movimento de escalada
                this.transform.position += new Vector3(0f, movimentoVertical * velocidadeDeEscalada, 0f) * Time.deltaTime;
                animator.SetFloat("playbackSpeed", Mathf.Abs(movimentoVertical) * defaultAnimSpeed);
                break;
            case TipoMovimento.EscaladaHorizontal:
                this.transform.position += new Vector3(movimentoHorizontal * velocidadeDeEscalada, 0f, 0f) * Time.deltaTime;
                animator.SetFloat("playbackSpeed", Mathf.Abs(movimentoHorizontal) * defaultAnimSpeed);
                break;
            case TipoMovimento.EscaladaOmnidirecional:
                this.transform.position += new Vector3(movimentoHorizontal * velocidadeDeEscalada, movimentoVertical * velocidadeDeEscalada, 0f) * Time.deltaTime;
                animator.SetFloat("playbackSpeed", (Mathf.Abs(movimentoHorizontal) + Mathf.Abs(movimentoVertical)) * defaultAnimSpeed);
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
            if (isGrounded && puloAgendado)
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

    /* METODOS DE INTERACAO */

    public bool InteracaoMudarRua()
    {
        int index = objetoInteragivel.transform.parent.childCount - objetoInteragivel.transform.GetSiblingIndex() - 1;
        this.transform.position = new Vector3(objetoInteragivel.transform.parent.GetChild(index).position.x, this.transform.position.y, 0f);
        return true;
    }
    
    public bool InteracaoEsconder(bool forceReset = false)
    {
        if (TipoMovimentoAtual == TipoMovimento.Livre && !forceReset)
        {
            TipoMovimentoAtual = TipoMovimento.Escondido;
            corpoMaicon.velocity = Vector2.zero;
            movimentoFinal = Vector2.zero;
            maiconSprite.enabled = false;
            this.transform.position = objetoInteragivel.transform.position;
            return true;
        }
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
        if (TipoMovimentoAtual == TipoMovimento.Livre && !forceReset)
        {
            PadraoEscalada();
            TipoMovimentoAtual = TipoMovimento.EscaladaVertical;
            this.transform.position = new Vector3(objetoInteragivel.transform.position.x, this.transform.position.y, this.transform.position.z);
            return true;
        }
        PadraoLivre();
        return true;
    }

    public bool InteracaoEscalavelHorizontal(bool forceReset = false)
    {
        if (TipoMovimentoAtual == TipoMovimento.Livre && !forceReset)
        {
            PadraoEscalada();
            TipoMovimentoAtual = TipoMovimento.EscaladaHorizontal;
            this.transform.position = new Vector3(this.transform.position.x, objetoInteragivel.transform.position.y, this.transform.position.z);
            
            return true;
        }
        PadraoLivre();
        return true;
    }

    public bool InteracaoEscalavelOmnidirecional(bool forceReset = false)
    {
        if (TipoMovimentoAtual == TipoMovimento.Livre && !forceReset)
        {
            PadraoEscalada();
            TipoMovimentoAtual = TipoMovimento.EscaladaOmnidirecional;
            return true;
        }
        PadraoLivre();
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
        animator.Play(Animator.StringToHash("maiconEscalar"));
    }

    private void PadraoLivre()
    {
        TipoMovimentoAtual = TipoMovimento.Livre;
        pehMaicon.isTrigger = false;
        animator.Play(Animator.StringToHash(isGrounded ? "maiconIdle" : "maiconCair"));
    }

    /* COLLIDER */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.Play(Animator.StringToHash("maiconAterrissar"));
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
