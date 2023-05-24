using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum PolicialAIState
{
    Patrulha, // estado padrao, indo e voltando entre dois pontos especificos
    Atencao, // estado de atencao em relacao ao maikinho (apenas policial civil usa)
    Ataque, // notou que maikinho fez merda e vai atras
    Procura // perdeu maikinho de vista e esta confuso
}

public enum TipoPolicial
{
    Civil, // tipo fraco de policial, nao agressivo em primeiro momento
    Militar // tipo forte de policial, agressivo
}

public class PolicialAI : MonoBehaviour // esse codigo da um caos, eu tenho que organizar depois
{
    private float hitCooldown = 10f; //ph
    /* Propriedades */
    [SerializeField] Transform maicon;
    private SpriteRenderer spritePolicial;
    private Rigidbody2D corpoPolicial;
    private BoxCollider2D colisorPolicial;
    private PolicialAIState estadoAtualDaIA;
    private TipoMovimento tipoMovimentoAtual;
    private Animator policialAnimator;

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
        estadoAtualDaIA = PolicialAIState.Patrulha;
        tipoMovimentoAtual = TipoMovimento.Livre;
        currentDirection = this.transform.position;
        movimentoFinal = Vector2.zero;
        policialAnimator = this.GetComponent<Animator>();

        velocidadePolicial = tipoPolicial == TipoPolicial.Civil ? 4 : 6;
        velocidadeEscaladaPolicial = tipoPolicial == TipoPolicial.Civil ? 6 : 8;


        policialAnimator.SetFloat("playbackSpeed", 1f);
    }

    private void Update()
    {
        hitCooldown -= hitCooldown > 0 ? Time.deltaTime : 0; // decremento e cap
        currentDirection = Vector3.zero;

        if (objetoAlvo == null)
        {
            this.SetMovimento(TipoMovimento.Livre);
            colisorPolicial.isTrigger = false;
        }

        switch (estadoAtualDaIA)
        {
            case PolicialAIState.Patrulha:
                policialAnimator.Play(Animator.StringToHash("policial" + tipoPolicial.ToString() + "Idle"));
                cooldownVirar -= Time.deltaTime;
                if (cooldownVirar <= 0)
                {
                    spritePolicial.flipX = !spritePolicial.flipX;
                    cooldownVirar = 2f;
                }
                break;
            case PolicialAIState.Atencao:
                break;
            case PolicialAIState.Ataque:
                if (objetoAlvo != null)
                {
                    currentDirection = objetoAlvo.position - this.transform.position;
                    if (tipoMovimentoAtual == TipoMovimento.Livre)
                    {
                        policialAnimator.Play(Animator.StringToHash("policial" + tipoPolicial.ToString() + (hitCooldown > 0 ? "Correr" : "Atacar")));
                        spritePolicial.flipX = currentDirection.x < 0 ? true : (currentDirection.x > 0 ? false : spritePolicial.flipX);

                        if (escalavelDisponivel != null && Mathf.Abs(currentDirection.x) < 5f && currentDirection.y > 6f)
                        {
                            this.SetMovimento(TipoMovimento.EscaladaOmnidirecional);
                            colisorPolicial.isTrigger = true;
                        }
                    }
                    if (tipoMovimentoAtual == TipoMovimento.EscaladaOmnidirecional)
                    {
                        policialAnimator.Play(Animator.StringToHash("policial" + tipoPolicial.ToString() + "Escalar"));
                        this.transform.position += (Vector3)(currentDirection + Vector2.up).normalized * velocidadeEscaladaPolicial * Time.deltaTime;

                        if (Mathf.Abs(currentDirection.x) < 5f && currentDirection.y < -6f)
                        {
                            this.SetMovimento(TipoMovimento.Livre);
                            colisorPolicial.isTrigger = false;
                        }
                    }
                }
                break;
            case PolicialAIState.Procura:
                break;
        }
    }

    private void FixedUpdate()
    {
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
            movimentoFinal.y = isGrounded ? -0.01f : movimentoFinal.y;
            // atualizar velocidade do policial
            switch (estadoAtualDaIA)
            {
                case PolicialAIState.Patrulha:
                case PolicialAIState.Ataque:
                    corpoPolicial.velocity = new Vector2(movimentoFinal.x * velocidadePolicial, movimentoFinal.y);
                    break;
            }
        }
    }

    public bool HitPlayer(int dano)
    {
        if(!(hitCooldown <= 0)) { return false; }
        Core.IncrementaPontosDeVida(-dano);
        hitCooldown = 10f;
        return true;
    }
}
