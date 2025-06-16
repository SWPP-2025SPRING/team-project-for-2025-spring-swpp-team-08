using UnityEngine;

public class HatCollideEnd : MonoBehaviour
{
    private bool _triggered = false;

    public InputName inputUIManager;

    private void Start()
    {
        if (inputUIManager == null)
        {
            Debug.LogError("NameInputUIManager가 씬에 없습니다!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return; 
        if (other.CompareTag("Player"))
        {
            _triggered = true;
            inputUIManager.Show();
        }
    }
}
