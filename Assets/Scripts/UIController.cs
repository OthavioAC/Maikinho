using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public void UpdatePaintMeter(int amount)
    {
        this.GetComponentInChildren<TextMeshProUGUI>().text = amount.ToString();
    }
}
