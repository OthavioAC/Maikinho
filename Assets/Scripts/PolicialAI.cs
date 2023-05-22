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

public class PolicialAI : MonoBehaviour
{
    [SerializeField] Transform maicon;
    private SpriteRenderer copSprite;
    private Rigidbody2D copBody;
    private PolicialAIState currentAIState;
    private float copSpeed = 20f; // ph

    private Transform target;

    private float phTimer = 2f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public Transform GetTarget()
    {
        return target;
    }

    private void Start()
    {
        Physics2D.IgnoreCollision(maicon.GetChild(1).GetComponent<BoxCollider2D>(), this.GetComponentInChildren<BoxCollider2D>(), true); // PH
        copBody = GetComponent<Rigidbody2D>();
        copSprite = this.GetComponent<SpriteRenderer>();
        currentAIState = PolicialAIState.Idle;
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
                    float currentDistance = target.position.x - this.transform.position.x;
                    copSprite.flipX = currentDistance < 0 ? true : (currentDistance > 0 ? false : copSprite.flipX);
                    if (Mathf.Abs(currentDistance) > 1f && copBody.velocity.magnitude < copSpeed)
                    {
                        copBody.velocity += new Vector2(currentDistance, 0f).normalized * copSpeed * Time.deltaTime;
                    }
                }
                break;
            case PolicialAIState.Apreensao:
                break;
        }
    }

    public void SetState(PolicialAIState newState)
    {
        this.currentAIState = newState;
    }
}
