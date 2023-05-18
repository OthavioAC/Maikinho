using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxController : MonoBehaviour
{
    [SerializeField] private float fixedHeight = 0;
    [SerializeField] private float paralaxSpeed = 0;

    Vector2 newPosition;
    Vector2 newLocal;

    Vector3 positionBuffer;

    private void Start()
    {
        positionBuffer = Camera.main.transform.position;
    }

    private void Update()
    {
        newPosition = this.transform.position;
        newPosition.x += (positionBuffer.x - Camera.main.transform.position.x) * paralaxSpeed;
        newPosition.y = fixedHeight;
        this.transform.position = newPosition;
        
        if (this.transform.localPosition.x > 30)
        {
            this.transform.localPosition -= Vector3.right * 51;
        }
        if (this.transform.localPosition.x < -30)
        {
            this.transform.localPosition += Vector3.right * 51;
        }

        positionBuffer = Camera.main.transform.position;
    }
}
