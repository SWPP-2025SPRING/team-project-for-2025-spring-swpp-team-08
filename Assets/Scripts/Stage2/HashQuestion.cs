using UnityEngine;

namespace Stage2
{
    public class HashQuestion
    {
        public int key;
        public IHashFunction hashFunction;
        public int correctAnswer;

        public HashQuestion(int key, IHashFunction hashFunction)
        {
            this.key = key;
            this.hashFunction = hashFunction;
            this.correctAnswer = hashFunction.Compute(key);
        }
    }
}
