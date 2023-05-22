using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PolicialAIState
{
    Idle,
    Busca,
    Apreensao,
}

public class PolicialAI : MonoBehaviour
{
    [SerializeField] private bool flipFlag = false;
    private SpriteRenderer copSprite;
    private PolicialAIState currentAIState;

    private void Start()
    {
        copSprite = this.GetComponent<SpriteRenderer>();
        currentAIState = PolicialAIState.Idle;
    }

    private void Update()
    {
        copSprite.flipX = flipFlag;
    }
}
