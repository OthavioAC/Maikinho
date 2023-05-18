using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MOVIMENTACAO
{
    Livre,
    Escalada,
}

public class MaiconController : MonoBehaviour
{
    /* UI */
    private Transform uiReference;
    /* Flags */
    private bool isGrounded = false;
    private bool interactionAvlb = false;
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
    private MOVIMENTACAO movimentacaoAtual;
    [SerializeField] private float baseCoyoteTime = 0f;
    private GameObject interactionObject;
    private Vector2 movimentoFinal;

    private Animator animator;

    private bool estaInteragindo;

    private void Start()
    {
        uiReference = Camera.main.transform.GetChild(0);
        maiconSprite = this.GetComponent<SpriteRenderer>();
        corpoMaicon = this.GetComponent<Rigidbody2D>();
        pehMaicon = this.transform.GetChild(0).GetComponent<BoxCollider2D>();
        movimentacaoAtual = MOVIMENTACAO.Livre;
        animator = this.GetComponent<Animator>();
    }
    
    private void Update()
    {
        // setar se maicon ta correndo ou trotando
        velocidadeAtual = velocidadeBase * (estaCorrendo ? 2f : 1f);
        // calcular tempo de atraso para pula apos queda de plataforma
        // deteccao de entrada geral
        movimentoHorizontal = Input.GetAxis("HORIZONTAL1");
        movimentoVertical = Input.GetAxis("VERTICAL1");
        estaInteragindo = Input.GetButtonDown("VERDE0");
        // atualizacao da direcao da animacao do maicon
        maiconSprite.flipX = movimentoHorizontal < 0 ? true : (movimentoHorizontal > 0 ? false : maiconSprite.flipX);
        // movimento
        switch (movimentacaoAtual)
        {
            case MOVIMENTACAO.Livre:
                // deteccao de entrada do movimento livre
                estaCorrendo = Input.GetButton("VERMELHO0");
                if (Input.GetButtonDown("AZUL0") && isGrounded)
                {
                    puloAgendado = true;
                    animator.Play(Animator.StringToHash("maiconPular"));
                }
                puloCancelado = Input.GetButtonUp("AZUL0") ? true : puloCancelado;
                // atualizacao da animacao no movimento livre
                if (!estaPulando) animator.Play(Animator.StringToHash(movimentoHorizontal != 0 ? "maiconCorrer" : "maiconIdle"));
                if (!isGrounded) animator.Play(Animator.StringToHash(corpoMaicon.velocity.y < 0 ? "maiconCair" : "maiconPular"));
                // interacao no movimento livre
                if (estaInteragindo && interactionAvlb)
                {
                    if (interactionObject.CompareTag("Grafiti"))
                    {
                        if (Core.GetQuantidadeTinta() > 0 && interactionObject.transform.GetChild(0).gameObject.activeSelf)
                        {
                            Core.IncrementaQuantidadeTinta(-1);
                            interactionObject.transform.GetChild(0).gameObject.SetActive(false); // desliga pichacao
                            interactionObject.transform.GetChild(1).gameObject.SetActive(true); // liga grafite
                        }
                        break;
                    }
                    if (interactionObject.CompareTag("Interactive"))
                    {
                        SegurarCalha();
                        break;
                    }
                    if (interactionObject.CompareTag("Rua"))
                    {
                        //Core.MudarRua(interactionObject.name); // dar tp aki dps
                        break;
                    }
                }
                break;
            case MOVIMENTACAO.Escalada:
                // atualizacao do movimento de escalada
                this.transform.position += new Vector3(0f, movimentoVertical * velocidadeDeEscalada, 0f) * Time.deltaTime;
                // interacao no movimento de escalada
                if (estaInteragindo && interactionAvlb)
                    SoltarCalha();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (movimentacaoAtual == MOVIMENTACAO.Livre)
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

    private void SegurarCalha()
    {
        isGrounded = false;
        movimentacaoAtual = MOVIMENTACAO.Escalada;
        corpoMaicon.velocity = Vector2.zero;
        this.transform.position = new Vector3(interactionObject.transform.position.x, this.transform.position.y, this.transform.position.z);
        pehMaicon.isTrigger = true;
        animator.Play(Animator.StringToHash("maiconEscalar"));
    }

    private void SoltarCalha()
    {
        movimentacaoAtual = MOVIMENTACAO.Livre;
        pehMaicon.isTrigger = false;
        animator.Play(Animator.StringToHash(isGrounded ? "maiconIdle" : "maiconCair"));
    }

    /* TRIGGER */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Grafiti")) uiReference.GetChild(1).gameObject.SetActive(true);
        if (collision.gameObject.CompareTag("Collectible"))
        {
            GameObject.Destroy(collision.gameObject);
            Core.IncrementaQuantidadeTinta(1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactive") || collision.gameObject.CompareTag("Grafiti") || collision.gameObject.CompareTag("Rua"))
        {
            interactionObject = collision.gameObject;
            interactionAvlb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactionAvlb = collision.gameObject.CompareTag("Interactive") || collision.gameObject.CompareTag("Grafiti") || collision.gameObject.CompareTag("Rua") ? false : interactionAvlb;
        if (collision.gameObject.CompareTag("Interactive")) SoltarCalha();
        if (collision.gameObject.CompareTag("Grafiti")) uiReference.GetChild(1).gameObject.SetActive(false);
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
