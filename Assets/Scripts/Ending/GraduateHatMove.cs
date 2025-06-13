using UnityEngine;

public class GraduateHatMove : MonoBehaviour
{
    public float amplitude = 0.05f; 
    public float frequency = 1f;  

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float yOffset = amplitude * Mathf.Sin(Time.time * frequency * 2 * Mathf.PI);
        transform.localPosition = startPos + new Vector3(0, yOffset, 0);
    }
}
