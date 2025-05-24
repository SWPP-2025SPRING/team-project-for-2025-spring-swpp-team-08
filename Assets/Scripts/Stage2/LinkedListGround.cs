using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stage2
{
    public class LinkedListGround : MonoBehaviour
    {
        public GameObject player;
        private PlayerController playerController;
        public int padNumber;
        private static int LAST_PAD_NUM = 0;

        private void Start()
        {
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
                if (playerController == null)
                {
                    Debug.Log("PlayerController 컴포넌트를 찾을 수 없습니다!");
                }
            }
            else
            {
                Debug.Log("Player 오브젝트가 연결되지 않았습니다!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            int expected = LAST_PAD_NUM + 1;

            if (padNumber == expected)
            {
                Debug.Log($"순서대로 밟음! 현재: {padNumber}");
                LAST_PAD_NUM = padNumber;
            }
            else
            {
                Debug.Log($"순서 틀림! 밟은 번호: {padNumber}, 기대한 번호: {expected}");
                LAST_PAD_NUM = 0;

                GameManager.Instance.playManager.DisplayCheckpointReturn();
                Vector3 currentCheckPoint = GameManager.Instance.playManager.GetCurrentCheckpoint();

                if (playerController != null)
                {
                    playerController.MoveTo(currentCheckPoint);
                }
            }
        }
    }
}
