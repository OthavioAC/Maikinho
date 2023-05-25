using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerTintas : MonoBehaviour
{
    [SerializeField] private GameObject tintaPrefab;
    [SerializeField] private GameObject[] tintas = { null, null, null, null, null, null, null };
    [SerializeField] private float[] respawnTime = { 10, 10, 10, 10, 10, 10, 0};

    private Vector3[] posicoes = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};

    private void Start()
    {
        for (int index = 0; index < (int)CorTinta.TODAS; index++)
        {
            if(tintas[index] != null)
            {
                posicoes[index] = tintas[index].transform.position;
            }
        }
    }

    private void Update()
    {
        for (int index = 0; index < (int)CorTinta.TODAS; index++)
        {
            if (tintas[index] != null) continue;
            if (respawnTime[index] <= 0)
            {
                tintas[index] = Instantiate(tintaPrefab, posicoes[index], Quaternion.identity, this.transform.parent);
                tintas[index].GetComponent<Interagivel>().SetCorTinta((CorTinta)index);
                respawnTime[index] = 5f;
                continue;
            }
            respawnTime[index] -= Time.deltaTime;
        }
    }
}
