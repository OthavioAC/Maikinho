using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField] private Transform maicon;
    [SerializeField] private float dogSpeed = 0f; // 6
    [SerializeField] private float comfortableDistance = 0f; // 7

    private bool estaSeguindo = false;

    private SpriteRenderer spriteDog;
    private Rigidbody2D corpoDog;
    private Animator dogAnimation;

    private void Start()
    {
        Physics2D.IgnoreCollision(maicon.GetChild(1).GetComponent<BoxCollider2D>(), this.GetComponent<BoxCollider2D>(), true);
        corpoDog = this.GetComponent<Rigidbody2D>();
        dogAnimation = this.GetComponent<Animator>();
        spriteDog = this.GetComponent<SpriteRenderer>();
    }
    
    private float aceleracao = 1f; // 1
    private float multiplicadorAceleracao = 3f; // 3

    private Vector2 currentDirection;

    private void Update()
    {
        currentDirection = maicon.position - this.transform.position;
        float currentDistance = currentDirection.x;
        if (!estaSeguindo && Mathf.Abs(currentDistance) < 1)
        {
            estaSeguindo = true;
        }

        spriteDog.flipX = currentDistance < 0 ? true : (currentDistance > 0 ? false : spriteDog.flipX);
        
        if(corpoDog.velocity.magnitude != 0f)
        {
            dogAnimation.Play(Animator.StringToHash("doginCarameloRun"));
        }
        else
        {
            dogAnimation.Play(Animator.StringToHash("doginCarameloIdle"));
        }
    }

    Vector2 movimentoFinal;

    private void FixedUpdate()
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
        // atualizar velocidade do dog
        if (estaSeguindo && Math.Abs(currentDirection.x) > comfortableDistance)
        {
            corpoDog.velocity = new Vector2(movimentoFinal.x * dogSpeed, movimentoFinal.y);
        }
    }

    private bool isGrounded = true;

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
}
