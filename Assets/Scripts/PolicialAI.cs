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

public class PolicialAI : MonoBehaviour // esse codigo da um caos, eu tenho que organizar depois
{
    [SerializeField] Transform maicon;
    private SpriteRenderer copSprite;
    private Rigidbody2D corpoPolicial;
    private BoxCollider2D copCollider;
    private PolicialAIState currentAIState;
    private TipoMovimento TipoMovimentoAtual;
    // ph
    public void SetMovimento(TipoMovimento novoTipo)
    {
        TipoMovimentoAtual = novoTipo;
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

    private float copSpeed = 4f; // 4 - 6
    private float copClimbSpeed = 6f; // 6 - 8

    private float aceleracao = 1f; // 1
    private float multiplicadorAceleracao = 3f; // 3

    private Transform target;

    private float phTimer = 2f;

    private GameObject escalavelDisponivel = null; // mudar pra lista dps... ou bool, mas acho que lista

    public void SetEscalavel(GameObject newEscalavel)
    {
        escalavelDisponivel = newEscalavel;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public Transform GetTarget()
    {
        return target;
    }

    Vector2 currentDirection;
    Vector2 movimentoFinal;

    public bool isGrounded = true;

    private void Start()
    {
        copCollider = this.transform.GetChild(0).GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(maicon.GetChild(1).GetComponent<BoxCollider2D>(), copCollider, true); // PH, usar PhysicsLayer dps
        corpoPolicial = GetComponent<Rigidbody2D>();
        copSprite = this.GetComponent<SpriteRenderer>();
        currentAIState = PolicialAIState.Idle;
        TipoMovimentoAtual = TipoMovimento.Livre;
        currentDirection = this.transform.position;
        movimentoFinal = Vector2.zero;
    }


    private void Update()
    {
        switch (currentAIState)
        {
            case PolicialAIState.Idle:
                phTimer -= Time.deltaTime;
                if (phTimer <= 0)
                {
                    copSprite.flipX = !copSprite.flipX;
                    phTimer = 2f;
                }
                break;
            case PolicialAIState.Busca:
                if (target != null)
                {
                    currentDirection = target.position - this.transform.position;
                    if (TipoMovimentoAtual == TipoMovimento.Livre)
                    {
                        copSprite.flipX = currentDirection.x < 0 ? true : (currentDirection.x > 0 ? false : copSprite.flipX);

                        if (escalavelDisponivel != null && Mathf.Abs(currentDirection.x) < 3f && currentDirection.y > 2f)
                        {
                            this.SetMovimento(TipoMovimento.EscaladaOmnidirecional);
                            copCollider.isTrigger = true;
                        }
                    }
                    if (TipoMovimentoAtual == TipoMovimento.EscaladaOmnidirecional)
                    {
                        this.transform.position += (Vector3)(currentDirection + Vector2.up).normalized * copClimbSpeed * Time.deltaTime;
                    }
                }
                break;
            case PolicialAIState.Apreensao:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentAIState)
        {
            case PolicialAIState.Busca:
                if (TipoMovimentoAtual == TipoMovimento.Livre)
                {
                    /* CALCULO X */
                    // x inicial, levando em conta aceleracao
                    movimentoFinal.x = Core.AceleracaoDeDepieri(movimentoFinal.x, currentDirection.normalized.x, aceleracao, multiplicadorAceleracao);
                    // cap aceleracao
                    movimentoFinal.x = movimentoFinal.x > -aceleracao && movimentoFinal.x < aceleracao ? 0 : movimentoFinal.x;
                    /* CALCULO Y */
                    // y inicial, levando em conta amortecimento
                    movimentoFinal.y -= Core.gravidade;
                    // ignorar gravidade se nao houver diferencial vertical
                    movimentoFinal.y = isGrounded && movimentoFinal.y <= 0 ? -0.01f : movimentoFinal.y;
                    // atualizar velocidade do policial
                    if (Mathf.Abs(currentDirection.x) > 3f)
                    {
                        corpoPolicial.velocity = new Vector2(movimentoFinal.x * copSpeed, movimentoFinal.y);
                    }
                }
                break;
        }
        
    }

    public void SetState(PolicialAIState newState)
    {
        this.currentAIState = newState;
    }
}
