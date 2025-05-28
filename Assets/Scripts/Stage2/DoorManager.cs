using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Stage2
{
    public class DoorManager : MonoBehaviour
    {
        public Door[] doors; 
        private HashQuestionFactory _factory = new HashQuestionFactory();

        private void Start()
        {
            SetupQuestion();
        }

        private void SetupQuestion()
        {
            var question = _factory.GenerateRandomQuestion();
            int correctAnswer = question.correctAnswer;
            int divisor = question.divisorHash;

            // Generate two wrong but unique answers
            HashSet<int> answers = new HashSet<int> { correctAnswer };
            while (answers.Count < 3)
            {
                int wrong = Random.Range(0, divisor); // tweak range if needed
                if (wrong != correctAnswer)
                    answers.Add(wrong);
            }

            List<int> answerList = answers.ToList();
            Shuffle(answerList);

            for (int i = 0; i < doors.Length; i++)
            {
                bool isCorrect = (answerList[i] == correctAnswer);
                doors[i].Setup(answerList[i], isCorrect);
            }
        }

        private void Shuffle(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = Random.Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
