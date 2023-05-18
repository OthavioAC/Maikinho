using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquilibrioController : MonoBehaviour
{
    [SerializeField] private MaiconController maicon;

    EquilibrioTrigger maiconFrontTrigger;
    EquilibrioTrigger maiconBackTrigger;

    private void Start()
    {
        maiconFrontTrigger = this.transform.GetChild(0).GetComponent<EquilibrioTrigger>();
        maiconBackTrigger = this.transform.GetChild(1).GetComponent<EquilibrioTrigger>();
    }

    private void Update()
    {
        this.transform.rotation = maicon.GetSpriteFlipX() ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.Euler(Vector3.zero);
        maicon.SetEstaEquilibrando(!(maiconFrontTrigger.GetOverlappingFlag() && maiconBackTrigger.GetOverlappingFlag()));
    }
}
