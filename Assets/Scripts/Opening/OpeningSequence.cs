using System.Collections;

using UnityEngine;

namespace Opening
{
    public class OpeningSequence : MonoBehaviour
    {
        public GameObject RoomObj;
        public GameObject PlayerBallObj;

        public IEnumerator PlaySequence()
        {
            PrintPlayerComment();
            PlayAnimation();

            yield return new WaitForSeconds(1f);
            RoomObj.SetActive(false);
            PlayerBallObj.SetActive(true);
            PlayerBallObj.transform.position = new Vector3(1, 1, 1);

            PrintPlayerComment();
            PrintSubtitle();
            PlayAnimation();

            yield return new WaitForSeconds(1f);
        }

        private void PrintPlayerComment()
        {
        }

        private void PrintSubtitle()
        {
        }

        private void PlayAnimation()
        {
        }

        private PlayManager GetPlayManager()
        {
            return GameManager.Instance.playManager;
        }
    }
}
