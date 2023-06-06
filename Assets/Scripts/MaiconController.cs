using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public enum TIPO_PISO
{
    Asfalto,
    Porcelana,
    Metal,
    Pano,
}

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
    [SerializeField] private GameObject coinTarget;
    [SerializeField] private GameObject coinPrefab;

    [SerializeField] private GameObject lifeTarget;
    [SerializeField] private GameObject lifePrefab;

    [SerializeField] private DogController dogCaramelo;


    private GameObject tutorial;
    private float speedTest = 0f;
    /* Audio */
    private AudioClip audioPulo;
    private AudioClip audioEscalar;
    private AudioClip audioAgarrar;
    /*  */
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
    /* Movimento Vertical */
    [SerializeField] private float pulo = 30f;
    [SerializeField] private float constanteDeDepieri = 1f; //usada para amortecer a queda
    [SerializeField] private float velocidadeDeEscalada = 7f;
    private float movimentoVertical;
    /* Movimento Horizontal */
    [SerializeField] private float velocidadeBase = 5f;
    [SerializeField] private float aceleracao = 1f;
    [SerializeField] private float multiplicadorAceleracao = 3f;
    private float movimentoHorizontal;
    private float velocidadeAtual = 0f;
    /*  */
    private TipoMovimento TipoMovimentoAtual;
    private Vector2 movimentoFinal;
    private float defaultAnimSpeed = .5f;
    //[SerializeField] private float baseCoyoteTime = 0f;

    public bool primeiroGrafite = true;
    
    private List<Interagivel> objetosInteragiveis = new List<Interagivel>();
    private Interagivel interagivelAtual;

    private List<(GameObject, float)> cooldownLixeiras = new List<(GameObject, float)>();

    public void AddObjetoInteragivel(Interagivel objetoInteragivel)
    {
        if (objetosInteragiveis.Count<Interagivel>() == 0)
        {
            interagivelAtual = objetoInteragivel;
        }
        objetosInteragiveis.Add(objetoInteragivel);
    }

    public void RemoveObjetoInteragivel(Interagivel objetoInteragivel)
    {
        objetosInteragiveis.Remove(objetoInteragivel);
        if (objetosInteragiveis.Count<Interagivel>() == 1)
        {
            interagivelAtual = objetosInteragiveis[0];
        }
        if (objetosInteragiveis.Count<Interagivel>() == 0)
        {
            interagivelAtual = null;
        }
    }
    
    public TipoMovimento GetTipoMovimento()
    {
        return TipoMovimentoAtual;
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
        Core.SetPontosDeVida(6);
        Core.SetQuantidadeTinta(CorTinta.TODAS, 0);
        interagivelAtual = null;

        audioSource = this.transform.GetChild(3).GetComponent<AudioSource>();
        audioPulo = Resources.Load<AudioClip>("Audio/Pulo");
        audioEscalar = Resources.Load<AudioClip>("Audio/EscalarCano");
        audioAgarrar = Resources.Load<AudioClip>("Audio/AgarrarCano");

        tutorial = Camera.main.transform.GetChild(0).GetChild(6).gameObject;
    }
    
    private void Update()
    {
        tutorial.SetActive(Input.GetButton("AZUL0") || Input.GetButton("AZUL1"));
        /* GAMBETA MASTER pra lixeira */
        for (int index = 0;  index < cooldownLixeiras.Count; index++)
        {
            if (cooldownLixeiras[index].Item1 != null)
            {
                if (cooldownLixeiras[index].Item2 <= 0f)
                {
                    cooldownLixeiras[index].Item1.GetComponent<Animator>().Play("abrirLixeira");
                    cooldownLixeiras.RemoveAt(index);
                    break;
                }
                if (cooldownLixeiras[index].Item2 == lixeiraDefaultCooldown)
                {
                    cooldownLixeiras[index] = (cooldownLixeiras[index].Item1, 40f);
                    
                    continue;
                }
                if(cooldownLixeiras[index].Item2 == 40f)
                {
                    cooldownLixeiras[index].Item1.GetComponent<Animator>().Play("fecharLixeira");
                }
                cooldownLixeiras[index] = (cooldownLixeiras[index].Item1, cooldownLixeiras[index].Item2 - Time.deltaTime);
            }
        }

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
        if (interagivelAtual != null)
        {
            bool interacaoDirecional = false;
            if (objetosInteragiveis.Count<Interagivel>() > 1)
            {
                bool interacaoVertical = interagivelAtual.GetTipoInteragivel() == TipoInteragivel.EscalavelHorizontal && (movimentoHorizontal == 0 && movimentoVertical != 0);
                bool interacaoHorizontal = interagivelAtual.GetTipoInteragivel() == TipoInteragivel.EscalavelVertical && (movimentoHorizontal != 0 && movimentoVertical == 0);
                bool interacaoOmni = interagivelAtual.GetTipoInteragivel() == TipoInteragivel.EscalavelOmnidirecional && (movimentoHorizontal != 0 || movimentoVertical != 0);


                Interagivel buffer = interagivelAtual;
                foreach (Interagivel objetoAlvo in objetosInteragiveis)
                {
                    if (buffer == null)
                    {
                        buffer = objetoAlvo;
                        break;
                    }
                    
                    bool flagEscalavel = 
                        objetoAlvo.GetTipoInteragivel() == TipoInteragivel.EscalavelHorizontal ||
                        objetoAlvo.GetTipoInteragivel() == TipoInteragivel.EscalavelVertical ||
                        objetoAlvo.GetTipoInteragivel() == TipoInteragivel.EscalavelOmnidirecional;
                    if (TipoMovimentoAtual != TipoMovimento.Livre && objetoAlvo != interagivelAtual && flagEscalavel)
                    {
                        if (objetoAlvo.GetTipoInteragivel() == TipoInteragivel.EscalavelVertical && interacaoVertical)
                        {
                            interacaoDisponivel = true;
                            estaInteragindo = true;
                            interacaoDirecional = true;
                            buffer = objetoAlvo;
                            break;
                        }
                        if (objetoAlvo.GetTipoInteragivel() == TipoInteragivel.EscalavelHorizontal && interacaoHorizontal)
                        {
                            if (this.transform.position.y - 1.5f > objetoAlvo.transform.position.y)
                            {
                                break;
                            }
                            interacaoDisponivel = true;
                            estaInteragindo = true;
                            interacaoDirecional = true;
                            buffer = objetoAlvo;
                            break;
                        }
                        if (objetoAlvo.GetTipoInteragivel() == TipoInteragivel.EscalavelOmnidirecional && interacaoOmni)
                        {
                            interacaoDisponivel = true;
                            estaInteragindo = true;
                            interacaoDirecional = true;
                            buffer = objetoAlvo;
                            break;
                        }
                    }
                    
                    if (objetoAlvo.GetTipoInteragivel() == TipoInteragivel.EscalavelHorizontal && this.transform.position.y - 1.5f > objetoAlvo.transform.position.y && movimentoVertical <= -1)
                    {
                        buffer = objetoAlvo;
                        break;
                    }

                    if (objetoAlvo.GetTipoInteragivel() == TipoInteragivel.Grafitavel && buffer.GetTipoInteragivel() == TipoInteragivel.EscalavelHorizontal)
                    {
                        if (this.transform.position.y - 1.5f > buffer.transform.position.y && movimentoVertical != -1)
                        {
                            buffer = objetoAlvo;
                            break;
                        }
                    }
                }
                interagivelAtual = buffer;
            }

            if (interacaoDisponivel && estaInteragindo)
            {
                if (interagivelAtual.GetTipoInteragivel() == TipoInteragivel.EscalavelHorizontal)
                {
                    if (this.transform.position.y - 1.5f > interagivelAtual.transform.position.y && movimentoVertical != -1) return;
                    if (!interacaoDirecional && this.transform.position.y <= interagivelAtual.transform.position.y && movimentoVertical == 1)
                    {
                        this.transform.position += Vector3.up * 2.5f;
                    }
                }
                interacaoDisponivel = !interacaoDirecional ? false : interacaoDisponivel;
                interagivelAtual.InteracaoOnButtonDown(this);
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
                    TocarAnimacao(corpoMaicon.velocity.y <= 0 ? Animacao.Cair : Animacao.Pular);
                }
                break;
            case TipoMovimento.EscaladaVertical:
                // atualizacao do movimento de escalada
                this.transform.position += new Vector3(0f, movimentoVertical * velocidadeDeEscalada, 0f) * Time.deltaTime;
                maiconAnimator.SetFloat("playbackSpeed", Mathf.Abs(movimentoVertical) * defaultAnimSpeed);
                TocarAnimacao(Animacao.Escalar);
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
                TocarAnimacao(Animacao.Escalar);
                if (botaoPulo)
                {
                    puloAgendado = true;
                    puloCancelado = false;
                    TocarAnimacao(Animacao.Pular);
                    this.transform.position += Vector3.up * 2.5f;
                    InteracaoEscalavelHorizontal(true);
                }
                break;
            case TipoMovimento.EscaladaOmnidirecional:
                this.transform.position += new Vector3(movimentoHorizontal, movimentoVertical, 0f).normalized * velocidadeDeEscalada * Time.deltaTime;
                maiconAnimator.SetFloat("playbackSpeed", Mathf.Max(Mathf.Abs(movimentoHorizontal), Mathf.Abs(movimentoVertical)) * defaultAnimSpeed);
                TocarAnimacao(Animacao.Escalar);
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
            // movimentoFinal.x = movimentoFinal.x + ((movimentoFinal.x < movimentoHorizontal * multiplicadorAceleracao) ? aceleracao : ((movimentoFinal.x > movimentoHorizontal * multiplicadorAceleracao) ? -aceleracao : -(movimentoHorizontal / 10f))) + (movimentoHorizontal / 10f);
            movimentoFinal.x = Core.AceleracaoDeDepieri(movimentoFinal.x, movimentoHorizontal, aceleracao, multiplicadorAceleracao);
            // cap aceleracao
            movimentoFinal.x = movimentoFinal.x > -aceleracao && movimentoFinal.x < aceleracao ? 0 : movimentoFinal.x;
            /* CALCULO Y */
            // y inicial, levando em conta amortecimento
            movimentoFinal.y -= Core.GetGravidade() / (estaAmortecendo ? constanteDeDepieri : 1f);
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
            // ignorar Core.gravidade se nao houver diferencial vertical
            movimentoFinal.y = isGrounded && movimentoFinal.y <= 0 ? -0.01f : movimentoFinal.y;
            // atualizar velocidade do maicon
            corpoMaicon.velocity = new Vector2(movimentoFinal.x * velocidadeAtual, movimentoFinal.y);
        }
    }

    private AudioSource audioSource;

    public void TocarAnimacao(Animacao animacao, bool tocarOneShot = false)
    {
        maiconAnimator.Play(Animator.StringToHash("maicon" + skinAtual.ToString() + animacao.ToString()));
        switch (animacao)
        {
            default:
                audioSource.Stop();
                break;
            case Animacao.Correr:
                AudioClip toLoad = Resources.Load<AudioClip>("Audio/" + animacao.ToString() + pisoAtual.ToString());
                if (audioSource.isPlaying && toLoad == audioSource.clip) break;
                else audioSource.clip = toLoad;
                audioSource.Play();
                break;
            case Animacao.Cair:
                break;
            case Animacao.Pular:
                if (audioSource.isPlaying && audioPulo == audioSource.clip) break;
                else audioSource.clip = audioPulo;
                audioSource.Play();
                break;
            case Animacao.Aterrissar:
                audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/" + animacao.ToString() + pisoAtual.ToString()));
                break;
            case Animacao.Escalar:
                if (tocarOneShot)
                {
                    audioSource.PlayOneShot(audioAgarrar);
                    break;
                }
                if (maiconAnimator.GetFloat("playbackSpeed") != 0)
                {
                    if (audioSource.isPlaying)
                    {
                        audioSource.clip = audioEscalar;
                        break;
                    }
                    audioSource.Play();
                    break;
                }
                if (audioSource.isPlaying && audioEscalar == audioSource.clip)
                {
                    audioSource.Stop();
                    break;
                }
                break;
        }
    }

    /* METODOS DE INTERACAO */

    public bool InteracaoBarzin()
    {
        if (Core.GetQuantidadeMoeda() < 5)
        {
            return false;
        }
        float totalVida = Core.AgendarIncrementoVida();
        if (totalVida < 13)
        {
            Core.IncrementaQuantidadeMoeda(-5);
            GameObject newLifeTarget = lifeTarget;
            switch (totalVida)
            {
                case 2:
                    newLifeTarget = lifeTarget.transform.GetChild(0).gameObject;
                    break;
                case 3:
                case 4:
                    newLifeTarget = lifeTarget.transform.GetChild(1).gameObject;
                    break;
                case 5:
                case 6:
                    newLifeTarget = lifeTarget.transform.GetChild(2).gameObject;
                    break;
                case 7:
                case 8:
                    newLifeTarget = lifeTarget.transform.GetChild(3).gameObject;
                    break;
                case 9:
                case 10:
                    newLifeTarget = lifeTarget.transform.GetChild(4).gameObject;
                    break;
                case 11:
                case 12:
                    newLifeTarget = lifeTarget.transform.GetChild(5).gameObject;
                    break;
            }
            StartCoroutine(SpawnLife(newLifeTarget));
        } 
        return true;
    }

    public bool InteracaoMudarRua()
    {
        if (primeiroGrafite)
        {
            return false;
        }
        int index = interagivelAtual.transform.parent.childCount - interagivelAtual.transform.GetSiblingIndex() - 1;
        this.transform.position = new Vector3(interagivelAtual.transform.parent.GetChild(index).position.x, this.transform.position.y, 0f);
        dogCaramelo.SeguirMaicon();
        return true;
    }

    private float lixeiraDefaultCooldown = 41f;

    public bool InteracaoEsconder(bool forceReset = false)
    {
        Animator lixeiraAnimation = interagivelAtual.GetComponent<Animator>();
        if (TipoMovimentoAtual == TipoMovimento.Livre && !forceReset)
        {
            foreach ((GameObject, float) lixeiraCD in cooldownLixeiras)
            {
                if (lixeiraCD.Item1 == interagivelAtual.gameObject) return false;
            }
            StartCoroutine(this.SpawnCoin(UnityEngine.Random.Range(1, 4)));
            //Core.IncrementaQuantidadeMoeda(1);
            lixeiraAnimation.Play("fecharLixeira");
            TipoMovimentoAtual = TipoMovimento.Escondido;
            corpoMaicon.velocity = Vector2.zero;
            movimentoFinal = Vector2.zero;
            maiconSprite.enabled = false;
            this.transform.position = interagivelAtual.transform.position;
            return true;
        }
        lixeiraAnimation.Play("abrirLixeira");
        TipoMovimentoAtual = TipoMovimento.Livre;
        corpoMaicon.velocity = Vector2.zero;
        movimentoFinal = Vector2.zero;
        maiconSprite.enabled = true;
        cooldownLixeiras.Add((interagivelAtual.gameObject, lixeiraDefaultCooldown));
        return true;
    }

    public bool InteracaoGrafite()
    {
        SpriteRenderer grafiteSpriteRenderer = interagivelAtual.GetComponent<SpriteRenderer>();
        int[] custoTinta = interagivelAtual.GetCustoTinta();
        for (int index = 0; index < (int)CorTinta.TODAS; index++)
        {
            if (Core.GetQuantidadeTinta((CorTinta)index) < custoTinta[index]) return false;
        }
        if (grafiteSpriteRenderer.sprite.name.EndsWith("_0")) //mudar o final
        {
            if (primeiroGrafite)
            {
                primeiroGrafite = false;
                StartCoroutine(this.SpawnCoin(9));
                //Core.IncrementaQuantidadeMoeda(10);
            }
            bool versionFlag = UnityEngine.Random.Range(0f, 1f) > .5f || grafiteSpriteRenderer.sprite.name.Contains("TORRE") || grafiteSpriteRenderer.sprite.name.Contains("DOG");
            string newSprite = grafiteSpriteRenderer.sprite.name.Replace('0', (versionFlag ? '1' : '2'));
            if (skinAtual == Skin.EasterEgg)
            {
                newSprite = "GRAFITE_SUS";
            }
            /* SPLACEHOLDER */
            foreach (Sprite spritePart in Resources.LoadAll<Sprite>("GRAFITE"))
            {
                if (spritePart.name == newSprite)
                {
                    grafiteSpriteRenderer.sprite = spritePart;
                    for (int index = 0; index < (int)CorTinta.TODAS; index++)
                    {
                        Core.IncrementaQuantidadeTinta((CorTinta)index, -custoTinta[index]);
                    }
                    break;
                }
            }
            StartCoroutine(this.SpawnCoin(1));
            Core.IncrementaGrafitesFeitos();
            return true;
        }
        return false;
    }

    public bool InteracaoEscalavelVertical(bool forceReset = false)
    {
        if (TipoMovimentoAtual != TipoMovimento.EscaladaVertical && !forceReset)
        {
            PadraoEscalada();
            TipoMovimentoAtual = TipoMovimento.EscaladaVertical;
            this.transform.position = new Vector3(interagivelAtual.transform.position.x, this.transform.position.y, this.transform.position.z);
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
            this.transform.position = new Vector3(this.transform.position.x, interagivelAtual.transform.position.y, this.transform.position.z);
            
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
        TocarAnimacao(Animacao.Escalar, true);
    }

    private void PadraoLivre()
    {
        TipoMovimentoAtual = TipoMovimento.Livre;
        pehMaicon.isTrigger = false;
        TocarAnimacao(isGrounded ? Animacao.Idle : Animacao.Cair);
    }

    /* COLLIDER */

    private TIPO_PISO GetTipoPiso(string tag)
    {
        // depois mudar
        switch (tag.Substring(6))
        {
            default:
            case "Asfalto":
                return TIPO_PISO.Asfalto;
            case "Metal":
                return TIPO_PISO.Metal;
            case "Porcelana":
                return TIPO_PISO.Porcelana;
            case "Pano":
                return TIPO_PISO.Pano;
        }
    }

    TIPO_PISO pisoAtual;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.StartsWith("Ground"))
        {
            pisoAtual = GetTipoPiso(collision.gameObject.tag);
            TocarAnimacao(Animacao.Aterrissar);
            estaPulando = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = collision.gameObject.tag.StartsWith("Ground") && pehMaicon.transform.position.y - collision.transform.position.y > 0 ? true : isGrounded; // por sleep dps
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.StartsWith("Ground"))
        {
            pisoAtual = GetTipoPiso(collision.gameObject.tag);
            isGrounded = false;
        }
    }

    private IEnumerator SpawnCoin(int quantidade)
    {
        for (int index = 0; index < quantidade; index++)
        {
            GameObject newCoin = GameObject.Instantiate(coinPrefab, Camera.main.transform, false);
            newCoin.transform.position = this.transform.position;
            newCoin.GetComponent<CoinController>().SetTarget(coinTarget);
            yield return new WaitForSeconds(.2f);
        }
        yield return null;
    }

    private IEnumerator SpawnLife(GameObject target)
    {
        GameObject newLife = GameObject.Instantiate(lifePrefab, Camera.main.transform, false);
        newLife.transform.position = this.transform.position;
        newLife.GetComponent<LifeController>().SetTarget(target);
        yield return null;
    }
}
