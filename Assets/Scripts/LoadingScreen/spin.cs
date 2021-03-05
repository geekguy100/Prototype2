using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{
    [Tooltip("the speed at which the object spin")]
    public float spinSpeed = 2;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, spinSpeed * Time.deltaTime * 100, Space.World);
    }
}
