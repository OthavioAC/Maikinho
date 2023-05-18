using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ACTION
{
    FreeMovement,
    SubidoCalha,
}

public class MaiconController : MonoBehaviour
{
    [SerializeField] private UIController uiReference; //gambeta

    private Rigidbody2D maiconBody;

    [SerializeField] private float movementSpeed = 0f;
    [SerializeField] private float climbSpeed = 0f;
    [SerializeField] private float jumpForce = 0f;

    private int Paint = 0;

    private ACTION currentAction;

    private bool isGrounded = true;

    private bool hasCoyoteTime = true;
    [SerializeField] private float baseCoyoteTime = 0f;
    private float coyoteTime = 0f;

    private bool interactionAvlb = false;
    private GameObject interactionObject;

    private void Start()
    {
        maiconBody = this.GetComponent<Rigidbody2D>();
        currentAction = ACTION.FreeMovement;
    }
    
    private void Update()
    {
        coyoteTime -= coyoteTime > 0f ? Time.deltaTime : 0f;

        bool inputLeft = Input.GetKey(KeyCode.LeftArrow); // [ph] left key
        bool inputRight = Input.GetKey(KeyCode.RightArrow); // [ph] right key
        SpriteRenderer maiconSprite = this.GetComponent<SpriteRenderer>();
        maiconSprite.flipX = inputLeft ? true : (inputRight ? false : maiconSprite.flipX);
        Vector2 horizontalMovement = (inputLeft ? Vector2.left : Vector2.zero) + (inputRight ? Vector2.right : Vector2.zero);
        Vector3 verticalMovement = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow)) // [ph] up key
        {
            verticalMovement += Vector3.up;
        }
        if (Input.GetKey(KeyCode.DownArrow)) // [ph] down key
        {
            verticalMovement += Vector3.down;
        }

        if (Input.GetKeyDown(KeyCode.E) && interactionAvlb) // [ph] action key
        {
            // verificar dps qual tipo de objeto pra escolher a acao... no momento, só troca entre subir calha e andar normal
            switch (currentAction)
            {
                case ACTION.FreeMovement:
                    if (interactionObject.GetComponent<GrafitiController>())
                    {
                        if (Paint > 0 && interactionObject.transform.GetChild(0).gameObject.activeSelf)
                        {
                            UpdatePainValue(-1);
                            interactionObject.transform.GetChild(0).gameObject.SetActive(false);
                            interactionObject.transform.GetChild(1).gameObject.SetActive(true);
                        }
                        break;
                    }
                    HoldCalha(); break;
                case ACTION.SubidoCalha: ReleaseCalha(); break;
            }
        }

        // move maicon and interact
        switch (currentAction)
        {
            case ACTION.FreeMovement:
                if (Input.GetKeyDown(KeyCode.DownArrow)) this.transform.localScale = new Vector3(1f, this.transform.localScale.y / 2, 1f);
                if (Input.GetKeyUp(KeyCode.DownArrow)) this.transform.localScale = Vector3.one;
                if (Input.GetKeyDown(KeyCode.UpArrow) && (isGrounded || coyoteTime > 0f))
                {
                    hasCoyoteTime = false; // pulando do chao, nao seta coyote
                    coyoteTime = 0f; // pulando no ar, reseta coyote
                    maiconBody.AddForce(Vector2.up * jumpForce);
                }
                if (Math.Abs(maiconBody.velocity.x) < movementSpeed)
                {
                    maiconBody.velocity += horizontalMovement.normalized * movementSpeed * Time.deltaTime;
                }
                maiconBody.gravityScale = (maiconBody.velocity.y < 0 ? 5f : 1f);
                break;
            case ACTION.SubidoCalha:
                this.transform.position += verticalMovement * climbSpeed * Time.deltaTime;
                break;
        }

    }

    private void HoldCalha()
    {
        isGrounded = false;
        currentAction = ACTION.SubidoCalha;
        maiconBody.gravityScale = 0f;
        maiconBody.velocity = Vector2.zero;
        this.transform.position = new Vector3(interactionObject.transform.position.x, this.transform.position.y, this.transform.position.z);
        this.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void ReleaseCalha()
    {
        currentAction = ACTION.FreeMovement;
        maiconBody.gravityScale = 1f;
        this.GetComponent<BoxCollider2D>().isTrigger = false;
    }

    private void UpdatePainValue(int increment)
    {
        Paint += increment;
        uiReference.UpdatePaintMeter(Paint);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Grafiti")) uiReference.transform.GetChild(1).gameObject.SetActive(true);
        if (collision.gameObject.CompareTag("Collectible"))
        {
            GameObject.Destroy(collision.gameObject);
            UpdatePainValue(1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactive") || collision.gameObject.CompareTag("Grafiti"))
        {
            interactionObject = collision.gameObject;
            interactionAvlb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactionAvlb = collision.gameObject.CompareTag("Interactive") || collision.gameObject.CompareTag("Grafiti") ? false : interactionAvlb;
        if (collision.gameObject.CompareTag("Interactive")) ReleaseCalha();
        if (collision.gameObject.CompareTag("Grafiti")) uiReference.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = collision.gameObject.CompareTag("Ground") ? true : isGrounded;
        hasCoyoteTime = collision.gameObject.CompareTag("Ground") ? true : hasCoyoteTime;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            coyoteTime = hasCoyoteTime ? baseCoyoteTime : 0f;
            isGrounded = false;
        }
    }
}
