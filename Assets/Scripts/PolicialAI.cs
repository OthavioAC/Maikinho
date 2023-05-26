using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum AnimacaoPolicial
{
    Idle,
    Correr,
    Atacar,
    Escalar,
}

public enum PolicialAIState
{
    Patrulha, // estado padrao, indo e voltando entre dois pontos especificos
    Atencao, // estado de atencao em relacao ao maikinho
    Ataque, // notou que maikinho fez merda e vai atras
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
    private Transform[] pontosDeAncora;
    private MaiconController maiconController;

    private float velocidadePolicial; // 4 - 6
    private float velocidadeEscaladaPolicial; // 6 - 8

    private float velocidadeAtualPolicial;
    private float multiplicadorVelocidade = 1f;

    private float aceleracao = 1f;
    private float multiplicadorAceleracao = 3f;

    private Transform objetoAlvo;

    private int ancoraAtual = 1; // indice da ancora atual
    private float cooldownVirar; // tempo que vira, parado no anchor point
    private float tempoParaTroca = 10f; // tempo que fica parado em cada anchor point
    private float tempoDeAtencao = 10f; // tempo que fica parado depois de perder o maikin de vista

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

    public PolicialAIState GetState()
    {
        return estadoAtualDaIA;
    }

    public void SetState(PolicialAIState newState)
    {
        this.estadoAtualDaIA = newState;
        tempoDeAtencao = 10f;
    }

    public TipoPolicial GetTipoPolicial()
    {
        return tipoPolicial;
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
        pontosDeAncora = this.transform.parent.GetChild(1).GetComponentsInChildren<Transform>();
        maiconController = maicon.GetComponent<MaiconController>();
        foreach(Transform nha in pontosDeAncora)
        {
            Debug.Log(nha.gameObject.name);
        }

        velocidadePolicial = tipoPolicial == TipoPolicial.Civil ? 4 : 6;
        velocidadeEscaladaPolicial = tipoPolicial == TipoPolicial.Civil ? 6 : 8;
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
                currentDirection = pontosDeAncora[ancoraAtual].position - this.transform.position;
                //Debug.Log(currentDirection.x);
                if (Mathf.Abs(currentDirection.x) <= 1f)
                {
                    multiplicadorVelocidade = 0;
                    TocarAnimacao(AnimacaoPolicial.Idle); // mudar dps
                    if (tempoParaTroca <= 0f)
                    {
                        ancoraAtual = ancoraAtual == 1 ? 2 : 1;
                        tempoParaTroca = Random.Range(5f, 10f);
                        break;
                    }
                    if (cooldownVirar <= 0)
                    {
                        spritePolicial.flipX = !spritePolicial.flipX;
                        cooldownVirar = Random.Range(2f, 3f);
                    }
                    tempoParaTroca -= Time.deltaTime;
                    cooldownVirar -= Time.deltaTime;
                    break;
                }
                multiplicadorVelocidade = 1f;
                TocarAnimacao(AnimacaoPolicial.Correr);
                spritePolicial.flipX = currentDirection.x < 0 ? true : (currentDirection.x > 0 ? false : spritePolicial.flipX);
                break;
            case PolicialAIState.Atencao:
                if (tempoDeAtencao > 0f)
                {
                    multiplicadorVelocidade = 0;
                    tempoDeAtencao -= Time.deltaTime;
                    TocarAnimacao(AnimacaoPolicial.Idle);
                    break;
                }
                multiplicadorVelocidade = .3f;
                currentDirection = pontosDeAncora[ancoraAtual].position - this.transform.position;
                TocarAnimacao(AnimacaoPolicial.Correr, multiplicadorVelocidade);
                spritePolicial.flipX = (maicon.position.x - this.transform.position.x) < 0 ? true : ((maicon.position.x - this.transform.position.x) > 0 ? false : spritePolicial.flipX);
                if (Mathf.Abs(currentDirection.x) <= 1f)
                {
                    estadoAtualDaIA = PolicialAIState.Patrulha;
                    tipoMovimentoAtual = TipoMovimento.Livre;
                    tempoDeAtencao = 10f;
                }
                break;
            case PolicialAIState.Ataque:
                if (objetoAlvo != null)
                {
                    if (maiconController.GetTipoMovimento() == TipoMovimento.Escondido)
                    {
                        this.SetMovimento(TipoMovimento.Livre);
                        colisorPolicial.isTrigger = false;
                        estadoAtualDaIA = PolicialAIState.Atencao;
                    }
                    currentDirection = objetoAlvo.position - this.transform.position;
                    if (tipoMovimentoAtual == TipoMovimento.Livre)
                    {
                        multiplicadorVelocidade = Mathf.Abs(currentDirection.x) <= 1f ? 0f : 1f;
                        TocarAnimacao(hitCooldown > 0 ? AnimacaoPolicial.Correr : AnimacaoPolicial.Atacar, multiplicadorVelocidade);
                        spritePolicial.flipX = currentDirection.x < 0 ? true : (currentDirection.x > 0 ? false : spritePolicial.flipX);

                        if (escalavelDisponivel != null && Mathf.Abs(currentDirection.x) < 5f && currentDirection.y > 6f)
                        {
                            this.SetMovimento(TipoMovimento.EscaladaOmnidirecional);
                            colisorPolicial.isTrigger = true;
                        }
                    }
                    if (tipoMovimentoAtual == TipoMovimento.EscaladaOmnidirecional)
                    {
                        TocarAnimacao(AnimacaoPolicial.Escalar);
                        this.transform.position += (Vector3)(currentDirection + Vector2.up).normalized * velocidadeEscaladaPolicial * Time.deltaTime;

                        if (Mathf.Abs(currentDirection.x) < 5f && currentDirection.y < -6f)
                        {
                            this.SetMovimento(TipoMovimento.Livre);
                            colisorPolicial.isTrigger = false;
                        }
                    }
                }
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
            velocidadeAtualPolicial = velocidadePolicial * multiplicadorVelocidade;
            corpoPolicial.velocity = new Vector2(movimentoFinal.x * velocidadeAtualPolicial, movimentoFinal.y);
        }
    }

    public bool HitPlayer(int dano)
    {
        if(!(hitCooldown <= 0)) { return false; }
        Core.IncrementaPontosDeVida(-dano);
        hitCooldown = 10f;
        if (this.tipoPolicial == TipoPolicial.Civil)
        {
            this.SetState(PolicialAIState.Patrulha);
            tipoMovimentoAtual = TipoMovimento.Livre;
        }
        return true;
    }

    public void TocarAnimacao(AnimacaoPolicial animacao, float playbackSpeed = 1f)
    {
        policialAnimator.SetFloat("playbackSpeed", playbackSpeed);
        policialAnimator.Play(Animator.StringToHash("policial" + tipoPolicial.ToString() + animacao.ToString()));
    }
}
