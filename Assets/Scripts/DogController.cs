using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField] private Transform maicon;
    [SerializeField] private float dogSpeed = 0f;
    [SerializeField] private float comfortableDistance = 0f;

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

    private void Update()
    {
        float currentDistance = maicon.position.x - this.transform.position.x;
        if (!estaSeguindo && Mathf.Abs(currentDistance) < 1)
        {
            estaSeguindo = true;
        }

        spriteDog.flipX = currentDistance < 0 ? true : (currentDistance > 0 ? false : spriteDog.flipX);
        if (estaSeguindo && Math.Abs(currentDistance) > comfortableDistance && corpoDog.velocity.magnitude < dogSpeed)
        {
            corpoDog.velocity += new Vector2(currentDistance, 0f).normalized * dogSpeed * Time.deltaTime;
        }

        if(corpoDog.velocity.magnitude != 0f)
        {
            dogAnimation.Play(Animator.StringToHash("doginCarameloRun"));
        }
        else
        {
            dogAnimation.Play(Animator.StringToHash("doginCarameloIdle"));
        }
    }
}
