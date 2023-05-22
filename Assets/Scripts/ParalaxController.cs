using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxController : MonoBehaviour
{
    [SerializeField] private float alturaFixa = 0;
    [SerializeField] private float velocidadeParalax = 0;

    Vector2 novaPosicao;
    Vector3 bufferPosicao;

    private void Start()
    {
        bufferPosicao = Camera.main.transform.position;
    }

    private void Update()
    {
        novaPosicao = this.transform.position;
        novaPosicao.x += (bufferPosicao.x - Camera.main.transform.position.x) * velocidadeParalax;
        novaPosicao.y = alturaFixa;
        this.transform.position = novaPosicao;
        
        if (this.transform.localPosition.x > 30)
        {
            this.transform.localPosition -= Vector3.right * 51;
        }
        if (this.transform.localPosition.x < -30)
        {
            this.transform.localPosition += Vector3.right * 51;
        }

        bufferPosicao = Camera.main.transform.position;
    }
}
