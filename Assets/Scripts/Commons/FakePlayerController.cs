using UnityEngine;

public class FakePlayerController : NewPlayerControl
{
    protected new void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected new void Update()
    {
        return;
    }

    protected new void FixedUpdate()
    {
        return;
    }

    public new void Jump(float magnitude)
    {
        rb.AddForce(Vector3.up * magnitude, ForceMode.Impulse);
    }
}
