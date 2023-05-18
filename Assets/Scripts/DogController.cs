using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField] private Transform maicon;
    [SerializeField] private float dogSpeed = 0f;
    [SerializeField] private float comfortableDistance = 0f;

    private void Start()
    {
        Physics2D.IgnoreCollision(maicon.GetComponent<BoxCollider2D>(), this.GetComponent<BoxCollider2D>(), true);
    }

    private void Update()
    {
        float currentDistance = maicon.position.x - this.transform.position.x;
        this.GetComponent<SpriteRenderer>().flipX = currentDistance < 0 ? false : (currentDistance > 0 ? true : this.GetComponent<SpriteRenderer>().flipX);
        if (Math.Abs(currentDistance) > comfortableDistance && this.GetComponent<Rigidbody2D>().velocity.magnitude < dogSpeed)
        {
            this.GetComponent<Rigidbody2D>().velocity += new Vector2(currentDistance, 0f).normalized * dogSpeed * Time.deltaTime;
        }
    }
}
