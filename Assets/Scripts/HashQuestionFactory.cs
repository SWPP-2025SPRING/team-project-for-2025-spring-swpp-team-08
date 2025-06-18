using System.Collections.Generic;

using UnityEngine;

namespace Stage2
{
    public class HashQuestionFactory
    {
        private List<IHashFunction> _hashFunctions;
        private System.Random _rng = new System.Random();

        public HashQuestionFactory()
        {
            _hashFunctions = new List<IHashFunction>
            {
                new ModHash(5),
                new ModHash(7),
                new ModHash(11)
            };
        }

        public HashQuestion GenerateRandomQuestion()
        {
            int key = _rng.Next(50, 1000); 
            int index = _rng.Next(_hashFunctions.Count);
            IHashFunction chosenFunction = _hashFunctions[index];

            return new HashQuestion(key, chosenFunction);
        }
    }
}
