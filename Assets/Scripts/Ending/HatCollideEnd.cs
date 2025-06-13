using UnityEngine;

public class HatCollideEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🎓 Graduation Hat triggered by Player!");
            GameManager.Instance.playManager.uiManager.UpdateSubtitle("Congratulations!", 3f);


        }
    }
}
