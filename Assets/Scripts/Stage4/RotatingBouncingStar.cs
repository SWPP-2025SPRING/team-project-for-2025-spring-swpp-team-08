using UnityEngine;

public class RotatingBouncingStar : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // Degrees per second
    
    [Header("Bouncing Settings")]
    public float bounceHeight = 0.5f;     // How high the star bounces
    public float bounceSpeed = 2f;        // How fast the bouncing animation is
    
    private Vector3 startPosition;
    private float bounceTimer;
    
    void Start()
    {
        // Store the initial position
        startPosition = transform.position;
        bounceTimer = 0f;
    }
    
    void Update()
    {
        // Handle rotation
        transform.Rotate(rotationSpeed * Time.deltaTime);
        
        // Handle bouncing
        bounceTimer += Time.deltaTime * bounceSpeed;
        
        // Calculate the bounce offset using sine wave for smooth up/down motion
        float bounceOffset = Mathf.Sin(bounceTimer) * bounceHeight;
        
        // Apply the bounce to the Y position
        Vector3 newPosition = startPosition;
        newPosition.y += bounceOffset;
        transform.position = newPosition;
    }
}