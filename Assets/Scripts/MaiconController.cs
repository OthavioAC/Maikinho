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
    /* GAMBETA */
    [SerializeField] private UIController uiReference;
    /* Propriedades */
    private int Paint { get; set; } // pra adicionar as diferentes cores dps
    /* Flags */
    private bool isGrounded = true;
    private bool interactionAvlb = false;
    private bool hasCoyoteTime = true;
    private bool estaAmortecendo = false;
    /* Input Flags */
    private bool estaCorrendo = false;
    private bool estaPulando = false;
    private bool puloSolto = false;
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
    private float coyoteTime = 0f;
    private Vector2 movimentoFinal;

    private void Start()
    {
        maiconSprite = this.GetComponent<SpriteRenderer>();
        corpoMaicon = this.GetComponent<Rigidbody2D>();
        pehMaicon = this.transform.GetChild(0).GetComponent<BoxCollider2D>();
        movimentacaoAtual = MOVIMENTACAO.Livre;
    }
    
    private void Update()
    {
        // setar se maicon ta correndo ou trotando
        velocidadeAtual = velocidadeBase * (estaCorrendo ? 2f : 1f);
        // calcular tempo de atraso para pula apos queda de plataforma
        coyoteTime -= coyoteTime > 0f ? Time.deltaTime : 0f; // ainda nao ta sendo usado
        // deteccao de entrada de movimento
        estaCorrendo = Input.GetKey(KeyCode.LeftShift);
        estaPulando = Input.GetKey(KeyCode.UpArrow); // mudar pra eixo
        puloSolto = Input.GetKeyUp(KeyCode.UpArrow); // mudar pra eixo
        movimentoHorizontal = Input.GetAxis("Horizontal");
        movimentoVertical = Input.GetAxis("Vertical");
        // atualizacao da direcao da animacao do maicon
        maiconSprite.flipX = movimentoHorizontal < 0 ? true : (movimentoHorizontal > 0 ? false : maiconSprite.flipX);
        // deteccao de entrada de interacao
        if (Input.GetKeyDown(KeyCode.E) && interactionAvlb)
        {
            // verificar dps qual tipo de objeto pra escolher a acao... no momento, apenas troca entre subir calha e andar normal
            switch (movimentacaoAtual)
            {
                case MOVIMENTACAO.Livre:
                    if (interactionObject.GetComponent<GrafitiController>())
                    {
                        if (Paint > 0 && interactionObject.transform.GetChild(0).gameObject.activeSelf)
                        {
                            UpdatePainValue(-1);
                            interactionObject.transform.GetChild(0).gameObject.SetActive(false);
                            interactionObject.transform.GetChild(1).gameObject.SetActive(true);
                        }
                        break;
                    }
                    SegurarCalha(); break;
                case MOVIMENTACAO.Escalada: SoltarCalha(); break;
            }
        }
        
        switch (movimentacaoAtual) // como separei o switch em dois, talvez seja melhor transformar em if
        {
             case MOVIMENTACAO.Escalada:
                 this.transform.position += new Vector3(0f, movimentoVertical * velocidadeDeEscalada, 0f) * Time.deltaTime;
                 break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        // atualiacao do movimento com base em tipo de movimentacao
        switch (movimentacaoAtual) // como separei o switch em dois, talvez seja melhor transformar em if
        {
            case MOVIMENTACAO.Livre:
                /* CALCULO X */
                // x inicial, levando em conta aceleracao
                movimentoFinal.x = Acelerar(movimentoFinal.x, movimentoHorizontal, aceleracao);
                // cap aceleracao
                if (movimentoFinal.x > -aceleracao && movimentoFinal.x < aceleracao)
                    movimentoFinal.x = 0;
                /* CALCULO Y */
                // y inicial, levando em conta amortecimento
                movimentoFinal.y -= gravidade / (estaAmortecendo ? constanteDeDepieri : 1f);
                estaAmortecendo = false;
                // adicao de altura no pulo se houver movimento horizontal
                if (isGrounded && estaPulando)
                    movimentoFinal.y = pulo + Math.Abs(movimentoFinal.x * velocidadeAtual / 3f);
                // amortecimento do pulo (soltar mais cedo e pular menos)
                if (puloSolto && movimentoFinal.y > 0)
                {
                    puloSolto = false;
                    movimentoFinal.y /= 2f;
                    estaAmortecendo = true;
                }
                // ignorar gravidade se nao houver diferencial vertical
                if (isGrounded && !(movimentoFinal.y > 0))
                    movimentoFinal.y = -0.01f;
                // atualizar velocidade do maicon
                corpoMaicon.velocity = new Vector2(movimentoFinal.x * velocidadeAtual, movimentoFinal.y);
                break;
            default:
                break;
        }
    }

    private float Acelerar(float movimentoFinalX, float movimentoHorizontalX, float aceleracao)
    {
        return movimentoFinalX + ((movimentoFinalX < movimentoHorizontalX * multiplicadorAceleracao) ? aceleracao : ((movimentoFinalX > movimentoHorizontalX * multiplicadorAceleracao) ? -aceleracao : -(movimentoHorizontalX / 10f))) + (movimentoHorizontalX / 10f);
    }

    private void SegurarCalha()
    {
        isGrounded = false;
        movimentacaoAtual = MOVIMENTACAO.Escalada;
        corpoMaicon.velocity = Vector2.zero;
        this.transform.position = new Vector3(interactionObject.transform.position.x, this.transform.position.y, this.transform.position.z);
        pehMaicon.isTrigger = true;
    }

    private void SoltarCalha()
    {
        movimentacaoAtual = MOVIMENTACAO.Livre;
        pehMaicon.isTrigger = false;
    }

    private void UpdatePainValue(int increment)
    {
        Paint += increment;
        uiReference.UpdatePaintMeter(Paint);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Grafiti")) uiReference.transform.GetChild(1).gameObject.SetActive(true);
        if (collision.gameObject.CompareTag("Collectible"))
        {
            GameObject.Destroy(collision.gameObject);
            UpdatePainValue(1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactive") || collision.gameObject.CompareTag("Grafiti"))
        {
            interactionObject = collision.gameObject;
            interactionAvlb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        interactionAvlb = collision.gameObject.CompareTag("Interactive") || collision.gameObject.CompareTag("Grafiti") ? false : interactionAvlb;
        if (collision.gameObject.CompareTag("Interactive")) SoltarCalha();
        if (collision.gameObject.CompareTag("Grafiti")) uiReference.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hasCoyoteTime = collision.gameObject.CompareTag("Ground") ? true : hasCoyoteTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = collision.gameObject.CompareTag("Ground") && pehMaicon.transform.position.y > collision.transform.position.y ? true : isGrounded; // por sleep dps
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            coyoteTime = hasCoyoteTime ? baseCoyoteTime : 0f;
            isGrounded = false;
        }
    }
}
