using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tinta : MonoBehaviour
{
    private void Update()
    {
        this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles + (new Vector3(0f, 90f, 0f) * Time.deltaTime));
    }
}
