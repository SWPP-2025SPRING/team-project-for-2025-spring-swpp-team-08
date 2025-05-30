using UnityEngine;

namespace Stage2
{
    public class Door : MonoBehaviour
    {
        public int displayedAnswer;
        public bool isCorrect;

        public void Setup(int answer, bool correct)
        {
            displayedAnswer = answer;
            isCorrect = correct;

            public TextMesh text;
            if (text != null)
                text.text = answer.ToString();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (isCorrect)
                {
                    Debug.Log("Correct door!");
                    // todo
                }
                else
                {
                    Debug.Log("Wrong door. Game Over.");
                    // todo
                }
            }
        }
    }
}
