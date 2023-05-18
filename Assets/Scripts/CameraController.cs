using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform maicon;
    [SerializeField] private float horizontalDistance = 0;
    [SerializeField] private float verticalDistance = 0;

    [SerializeField] private float maxWorldHeight = 0;
    [SerializeField] private float minWorldHeight = 0; // 3

    private (float, float)[] streetBounds =
    {
        (-123, -35), // "rua 1"
        (-33.5f, 31), // "rua 2"
        (33, 105), // "rua 3"
    };

    private Vector3 newCameraPosition;

    void Update()
    {
        newCameraPosition = this.transform.position;
        // x update
        float difX = maicon.position.x - this.transform.position.x;
        if (Math.Abs(difX) > horizontalDistance)
        {
            float directionX = difX / Math.Abs(difX);
            newCameraPosition.x = KeepXInBounds(maicon.position.x - (horizontalDistance * directionX));
        }
        // y update
        float difY = maicon.position.y - this.transform.position.y;
        if (Math.Abs(difY) > verticalDistance)
        {
            float directionY = difY / Math.Abs(difY);
            newCameraPosition.y = KeepYInBounds(maicon.position.y - (verticalDistance * directionY));
        }
        this.transform.position = newCameraPosition;
    }

    private float KeepXInBounds(float newXPosition)
    {
        float diff = newXPosition - this.transform.position.x;
        float offset = (float)Math.Sqrt(Math.Pow((this.transform.position.x - Camera.main.ViewportToWorldPoint(Vector3.one).x), 2));
        (float, int) positionBuffer = (newXPosition, 0);
        foreach((float, float) rua in streetBounds)
        { 
            if (Camera.main.ViewportToWorldPoint(Vector3.zero).x + diff > rua.Item1 && Camera.main.ViewportToWorldPoint(Vector3.one).x + diff < rua.Item2) return newXPosition;
            float dist0 = (float)Math.Sqrt(Math.Pow((newXPosition - positionBuffer.Item1), 2));
            float dist1 = (float)Math.Sqrt(Math.Pow((newXPosition - rua.Item1), 2));
            float dist2 = (float)Math.Sqrt(Math.Pow((newXPosition - rua.Item2), 2));
            positionBuffer = dist0 != 0 ? (dist0 < dist1 ? (dist0 < dist2 ? positionBuffer : (rua.Item2, -1)) : (dist1 < dist2 ? (rua.Item1, 1) : (rua.Item2, -1))) : (dist1 < dist2 ? (rua.Item1, 1) : (rua.Item2, -1));
        }
        return positionBuffer.Item1 + (offset * positionBuffer.Item2);
    }

    private float KeepYInBounds(float newYPosition)
    {
        if (newYPosition > maxWorldHeight)
        {
            return maxWorldHeight;
        }
        if (newYPosition < minWorldHeight)
        {
            return minWorldHeight;
        }
        return newYPosition;
    }

}
