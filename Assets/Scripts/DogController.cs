using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField] private Transform maicon;
    [SerializeField] private float dogSpeed = 0f;
    [SerializeField] private float comfortableDistance = 0f;

    private Rigidbody2D corpoDog;

    private float rotationFrame = 1f;
    private float rotationTime = .01f;

    private void Start()
    {
        Physics2D.IgnoreCollision(maicon.GetChild(0).GetComponent<BoxCollider2D>(), this.GetComponent<BoxCollider2D>(), true);
        corpoDog = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float currentDistance = maicon.position.x - this.transform.position.x;
        this.GetComponent<SpriteRenderer>().flipX = currentDistance < 0 ? true : (currentDistance > 0 ? false : this.GetComponent<SpriteRenderer>().flipX);
        if (Math.Abs(currentDistance) > comfortableDistance && this.GetComponent<Rigidbody2D>().velocity.magnitude < dogSpeed)
        {
            corpoDog.velocity += new Vector2(currentDistance, 0f).normalized * dogSpeed * Time.deltaTime;
        }

        rotationTime -= Time.deltaTime;

        if(corpoDog.velocity.magnitude != 0f && rotationTime <= 0f)
        {
            rotationFrame *= -1f;
            rotationTime = .5f;
            this.transform.rotation = Quaternion.Euler(0f,0f,15f * rotationFrame);
        }

        if (corpoDog.velocity.magnitude == 0f)
        {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
