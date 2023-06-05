using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquilibrioTrigger : MonoBehaviour
{
    private bool isOverlappingGround = false;

    public bool GetOverlappingFlag()
    {
        return isOverlappingGround;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.StartsWith("Ground"))
        {
            isOverlappingGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.StartsWith("Ground"))
        {
            isOverlappingGround = false;
        }
    }
}
