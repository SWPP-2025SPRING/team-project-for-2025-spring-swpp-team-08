using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2 : MonoBehaviour
{
    public Transform target; // The target to follow
    private Vector3 offset; // Offset from the target
    void Start()
    {
        offset = transform.position - target.position; // Calculate the initial offset
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            target.position.z + offset.z
        ); // Update camera position based on target position and offset
        
    }
}
