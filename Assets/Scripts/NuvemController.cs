using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuvemController : MonoBehaviour
{
    [SerializeField] private GameObject nuvemPrefab;
    [SerializeField] private Transform nuvemContainer;
    private Sprite[] nuvemSprite;

    private float nuvemCooldown;
    private float bufferAltura = 20f;

    private float alturaInicial = 20f;
    private float rangeAltura = 200f;

    private void Start()
    {
        Vector3 bufferPosition = this.transform.position;
        bufferPosition.x = Camera.main.ViewportToWorldPoint(Vector3.left).x;
        this.transform.position = bufferPosition;
        nuvemSprite = Resources.LoadAll<Sprite>("NUVEM");
        nuvemCooldown = 1f;
    }

    private void Update()
    {
        nuvemCooldown -= Time.deltaTime;
        if(nuvemCooldown <= 0f)
        {
            float novaAltura = Random.Range(alturaInicial, alturaInicial + rangeAltura);
            nuvemCooldown = Random.Range(.1f, 1f) * (1 - Mathf.Abs((bufferAltura - novaAltura) / rangeAltura));
            bufferAltura = novaAltura;
            GameObject novaNuvem = Instantiate(nuvemPrefab, nuvemContainer, false);
            novaNuvem.transform.position = new Vector3(Camera.main.ViewportToWorldPoint(Vector3.right * 2f).x, novaAltura, 0f);
            novaNuvem.GetComponent<SpriteRenderer>().sprite = nuvemSprite[Random.Range(0, nuvemSprite.Length)];
            novaNuvem.GetComponent<Rigidbody2D>().velocity = Vector2.left * Random.Range(5f, 20f);
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
