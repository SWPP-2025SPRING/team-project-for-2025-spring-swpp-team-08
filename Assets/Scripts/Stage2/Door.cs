using UnityEngine;

namespace Stage2
{
    public class Door : MonoBehaviour
    {
        private int _displayedAnswer;
        private bool _isCorrect;
        public TextMesh text;

        public void Setup(int answer, bool correct)
        {
            _displayedAnswer = answer;
            _isCorrect = correct;

            if (text != null)
            {
                text.text = answer.ToString();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (_isCorrect)
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
