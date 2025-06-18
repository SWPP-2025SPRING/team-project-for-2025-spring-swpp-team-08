using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NewPlayerMove : MonoBehaviour
{
    public float liftSpeed = 0.5f;

    void Update()
    {
        transform.position += Vector3.up * liftSpeed * Time.deltaTime;
    }
}
