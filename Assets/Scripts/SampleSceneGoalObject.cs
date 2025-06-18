using UnityEngine;

namespace SampleScene
{
    public class SampleSceneGoalObject : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            GameManager.Instance.playManager.FinishGame();
        }
    }
}
