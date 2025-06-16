using UnityEngine;

public class HatCollideEnd : MonoBehaviour
{
    private bool _triggered = false;
    public GameObject currentPlayer;
    public GameObject newPlayer;
    public GameObject originalHat;
    public float liftSpeed = 1.0f;

    public InputName inputUIManager;

    private Transform newPlayerTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("collided");
            _triggered = true;
            currentPlayer.SetActive(false);
            //originalHat.SetActive(false);
            inputUIManager.Show();
            newPlayer.SetActive(true);
            originalHat.SetActive(false);
            //newPlayerTransform = newPlayer.transform;
            //inputUIManager.Show();
        }
    }

    private void Update()
    {
        if (_triggered && newPlayerTransform != null)
        {
            newPlayerTransform.position += Vector3.up * liftSpeed * Time.deltaTime;
        }
    }
}
