using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private GameObject target;
    private Vector3 spawnTarget;
    private Vector3 spawnTargetPosition;
    private float spawnRadius = 3f;
    private float coinSpeed = 25f;
    private bool moveToTarget = false;
    private float moveDelay = 1f;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    private void Start()
    {
        spawnTarget = Camera.main.WorldToViewportPoint(this.transform.position + new Vector3(UnityEngine.Random.Range(-spawnRadius, spawnRadius), UnityEngine.Random.Range(-spawnRadius, spawnRadius), 0f));
    }

    private void Update()
    {
        spawnTargetPosition = Camera.main.ViewportToWorldPoint(spawnTarget);
        if ((spawnTargetPosition - this.transform.position).magnitude < .2f) moveToTarget = true;
        Vector3 direction = (moveToTarget ? target.transform.position : spawnTargetPosition) - this.transform.position;
        if (!moveToTarget || moveDelay <= 0) this.transform.position += direction.normalized * coinSpeed * Time.deltaTime;
        if (moveToTarget)
        {
            moveDelay -= Time.deltaTime;
            if (direction.magnitude < 2f)
            {
                Core.IncrementaQuantidadeMoeda(1);
                Destroy(this.gameObject);
            }
        }
    }
}
