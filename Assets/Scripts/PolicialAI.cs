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
    private BoxCollider2D copCollider;
    private PolicialAIState currentAIState;
    private TipoMovimento currentMovimento;
    // ph
    public void SetMovimento(TipoMovimento novoTipo)
    {
        currentMovimento = novoTipo;
        switch (novoTipo)
        {
            case TipoMovimento.Livre:
                copBody.gravityScale = 1;
                break;
            case TipoMovimento.EscaladaOmnidirecional:
                copBody.gravityScale = 0;
                break;
        }
    }

    private float copSpeed = 20f; // ph -> [4 - 6]
    private float copClimbSpeed = 5f; // ph -> [6 - 8]

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

    private void Start()
    {
        copCollider = this.transform.GetChild(0).GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(maicon.GetChild(1).GetComponent<BoxCollider2D>(), copCollider, true); // PH, usar PhysicsLayer dps
        copBody = GetComponent<Rigidbody2D>();
        copSprite = this.GetComponent<SpriteRenderer>();
        currentAIState = PolicialAIState.Idle;
        currentMovimento = TipoMovimento.Livre;
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
                    Vector2 currentDirection = target.position - this.transform.position;
                    if (currentMovimento == TipoMovimento.Livre)
                    {
                        float currentDistance = currentDirection.x;
                        copSprite.flipX = currentDistance < 0 ? true : (currentDistance > 0 ? false : copSprite.flipX);
                        if (Mathf.Abs(currentDistance) > 3f && copBody.velocity.magnitude < copSpeed)
                        {
                            copBody.velocity += new Vector2(currentDistance, 0f).normalized * copSpeed * Time.deltaTime;
                        }

                        if(escalavelDisponivel != null && Mathf.Abs(currentDistance) < 3f && currentDirection.y > 2f)
                        {
                            this.SetMovimento(TipoMovimento.EscaladaOmnidirecional);
                            copCollider.isTrigger = true;
                        }
                    }
                    if (currentMovimento == TipoMovimento.EscaladaOmnidirecional)
                    {
                        this.transform.position += (Vector3)(currentDirection + Vector2.up).normalized * copSpeed * Time.deltaTime;
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
