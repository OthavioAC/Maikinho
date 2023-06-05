using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField] private Transform maicon;
    [SerializeField] private float dogSpeed = 6f;
    [SerializeField] private float comfortableDistance = 7f;
    /* Componentes */
    private SpriteRenderer spriteDog;
    private Rigidbody2D corpoDog;
    private Animator animatorDog;
    private AudioSource audioDog;
    /* Movimento */
    private Vector2 direcaoAtual;
    private Vector2 movimentoFinal;
    private float aceleracao = 1f;
    private float multiplicadorAceleracao = 3f;
    /* Flags */
    private bool isGrounded = true;
    private bool estaSeguindo = false;
    private bool bark = false;

    private void Start()
    {
        Physics2D.IgnoreCollision(maicon.GetChild(1).GetComponent<BoxCollider2D>(), this.GetComponent<BoxCollider2D>(), true);
        corpoDog = this.GetComponent<Rigidbody2D>();
        animatorDog = this.GetComponent<Animator>();
        spriteDog = this.GetComponent<SpriteRenderer>();
        audioDog = this.GetComponent<AudioSource>();
    }

    private void Update()
    {
        direcaoAtual = maicon.position - this.transform.position;
        float currentDistance = direcaoAtual.x;
        if (!estaSeguindo && Mathf.Abs(currentDistance) < 1)
        {
            estaSeguindo = true;
        }

        spriteDog.flipX = currentDistance < 0 ? true : (currentDistance > 0 ? false : spriteDog.flipX);
        
        if(corpoDog.velocity.magnitude != 0f)
        {
            animatorDog.Play(Animator.StringToHash("doginCarameloRun"));
            if (bark)
            {
                bark = false;
                audioDog.Play();
            }
        }
        else
        {
            animatorDog.Play(Animator.StringToHash("doginCarameloIdle"));
            bark = true;
        }
    }

    private void FixedUpdate()
    {
        /* CALCULO X */
        // x inicial, levando em conta aceleracao
        movimentoFinal.x = Core.AceleracaoDeDepieri(movimentoFinal.x, direcaoAtual.normalized.x, aceleracao, multiplicadorAceleracao);
        // cap aceleracao
        movimentoFinal.x = movimentoFinal.x > -aceleracao && movimentoFinal.x < aceleracao ? 0 : movimentoFinal.x;
        /* CALCULO Y */
        // y inicial, levando em conta amortecimento
        movimentoFinal.y -= Core.GetGravidade();
        // ignorar gravidade se nao houver diferencial vertical
        movimentoFinal.y = isGrounded && movimentoFinal.y <= 0 ? -0.01f : movimentoFinal.y;
        // atualizar velocidade do dog
        if (estaSeguindo && Math.Abs(direcaoAtual.x) > comfortableDistance)
        {
            corpoDog.velocity = new Vector2(movimentoFinal.x * dogSpeed, movimentoFinal.y);
        }
    }

    // colisores
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void SeguirMaicon()
    {
        if (estaSeguindo)
        {
            this.transform.position = maicon.position - (Vector3)(direcaoAtual.normalized * 3);
            audioDog.Play();
        }
    }
}
