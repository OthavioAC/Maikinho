using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuvemController : MonoBehaviour
{
    [SerializeField] private GameObject nuvemPrefab;
    [SerializeField] private Transform nuvemContainer;

    private float nuvemCooldown;

    private void Start()
    {
        Vector3 bufferPosition = this.transform.position;
        bufferPosition.x = Camera.main.ViewportToWorldPoint(Vector3.left).x;
        this.transform.position = bufferPosition;
        nuvemCooldown = 1f;
    }

    private void Update()
    {
        nuvemCooldown -= Time.deltaTime;
        if(nuvemCooldown <= 0f)
        {
            nuvemCooldown = .1f;
            GameObject novaNuvem = Instantiate(nuvemPrefab, nuvemContainer, false);
            novaNuvem.transform.position = new Vector3(Camera.main.ViewportToWorldPoint(Vector3.right * 2f).x, Random.Range(20f, 40f), 0f);
            novaNuvem.GetComponent<Rigidbody2D>().velocity = Vector2.left * Random.Range(10f, 35f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Nuvem"))
        {
            Destroy(collision.gameObject);
        }
    }
}
