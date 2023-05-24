using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxController : MonoBehaviour
{
    [SerializeField] private bool fixarAltura = false;
    [SerializeField] private bool acompanharCamera = false;
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
        novaPosicao.y = fixarAltura ? alturaFixa : novaPosicao.y;
        this.transform.position = novaPosicao;
        
        if (acompanharCamera && this.transform.localPosition.x > 30)
        {
            this.transform.localPosition -= Vector3.right * 51.2f;
        }
        if (acompanharCamera && this.transform.localPosition.x < -30)
        {
            this.transform.localPosition += Vector3.right * 51.2f;
        }

        bufferPosicao = Camera.main.transform.position;
    }
}
