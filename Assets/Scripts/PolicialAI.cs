using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum PolicialAIState
{
    Idle,
    Busca,
    Apreensao,
}

public enum TipoPolicial
{
    Frakin,
    Fortin
}

public class PolicialAI : MonoBehaviour // esse codigo da um caos, eu tenho que organizar depois
{
    /* Propriedades */
    [SerializeField] Transform maicon;
    private SpriteRenderer spritePolicial;
    private Rigidbody2D corpoPolicial;
    private BoxCollider2D colisorPolicial;
    private PolicialAIState estadoAtualDaIA;
    private TipoMovimento tipoMovimentoAtual;

    private float velocidadePolicial; // 4 - 6
    private float velocidadeEscaladaPolicial; // 6 - 8

    private float aceleracao = 1f;
    private float multiplicadorAceleracao = 3f;

    private Transform objetoAlvo;

    private float cooldownVirar = 2.5f;

    private GameObject escalavelDisponivel = null; // mudar pra lista dps... ou bool, mas acho que lista

    Vector2 currentDirection;
    Vector2 movimentoFinal;

    [SerializeField] private TipoPolicial tipoPolicial;

    public bool isGrounded = true;
    public void SetMovimento(TipoMovimento novoTipo)
    {
        tipoMovimentoAtual = novoTipo;
        switch (novoTipo)
        {
            case TipoMovimento.Livre:
                corpoPolicial.gravityScale = 1;
                break;
            case TipoMovimento.EscaladaOmnidirecional:
                corpoPolicial.gravityScale = 0;
                break;
        }
    }

    public void SetEscalavel(GameObject newEscalavel)
    {
        escalavelDisponivel = newEscalavel;
    }

    public void SetObjetoAlvo(Transform newobjetoAlvo)
    {
        objetoAlvo = newobjetoAlvo;
    }

    public Transform GetObjetoAlvo()
    {
        return objetoAlvo;
    }

    public void SetState(PolicialAIState newState)
    {
        this.estadoAtualDaIA = newState;
    }

    private void Start()
    {
        colisorPolicial = this.transform.GetChild(0).GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(maicon.GetChild(1).GetComponent<BoxCollider2D>(), colisorPolicial, true); // PH, usar PhysicsLayer dps
        corpoPolicial = GetComponent<Rigidbody2D>();
        spritePolicial = this.GetComponent<SpriteRenderer>();
        estadoAtualDaIA = PolicialAIState.Idle;
        tipoMovimentoAtual = TipoMovimento.Livre;
        currentDirection = this.transform.position;
        movimentoFinal = Vector2.zero;

        velocidadePolicial = tipoPolicial == TipoPolicial.Frakin ? 4 : 6;
        velocidadeEscaladaPolicial = tipoPolicial == TipoPolicial.Frakin ? 6 : 8;
    }

    private void Update()
    {
        switch (estadoAtualDaIA)
        {
            case PolicialAIState.Idle:
                cooldownVirar -= Time.deltaTime;
                if (cooldownVirar <= 0)
                {
                    spritePolicial.flipX = !spritePolicial.flipX;
                    cooldownVirar = 2f;
                }
                break;
            case PolicialAIState.Busca:
                if (objetoAlvo != null)
                {
                    currentDirection = objetoAlvo.position - this.transform.position;
                    if (tipoMovimentoAtual == TipoMovimento.Livre)
                    {
                        spritePolicial.flipX = currentDirection.x < 0 ? true : (currentDirection.x > 0 ? false : spritePolicial.flipX);

                        if (escalavelDisponivel != null && Mathf.Abs(currentDirection.x) < 3f && currentDirection.y > 2f)
                        {
                            this.SetMovimento(TipoMovimento.EscaladaOmnidirecional);
                            colisorPolicial.isTrigger = true;
                        }
                    }
                    if (tipoMovimentoAtual == TipoMovimento.EscaladaOmnidirecional)
                    {
                        this.transform.position += (Vector3)(currentDirection + Vector2.up).normalized * velocidadeEscaladaPolicial * Time.deltaTime;
                    }
                }
                break;
            case PolicialAIState.Apreensao:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (estadoAtualDaIA)
        {
            case PolicialAIState.Busca:
                if (tipoMovimentoAtual == TipoMovimento.Livre)
                {
                    /* CALCULO X */
                    // x inicial, levando em conta aceleracao
                    movimentoFinal.x = Core.AceleracaoDeDepieri(movimentoFinal.x, currentDirection.normalized.x, aceleracao, multiplicadorAceleracao);
                    // cap aceleracao
                    movimentoFinal.x = movimentoFinal.x > -aceleracao && movimentoFinal.x < aceleracao ? 0 : movimentoFinal.x;
                    /* CALCULO Y */
                    // y inicial, levando em conta amortecimento
                    movimentoFinal.y -= Core.GetGravidade();
                    // ignorar gravidade se nao houver diferencial vertical
                    movimentoFinal.y = isGrounded && movimentoFinal.y <= 0 ? -0.01f : movimentoFinal.y;
                    // atualizar velocidade do policial
                    if (Mathf.Abs(currentDirection.x) > 3f)
                    {
                        corpoPolicial.velocity = new Vector2(movimentoFinal.x * velocidadePolicial, movimentoFinal.y);
                    }
                }
                break;
        }
        
    }
}
