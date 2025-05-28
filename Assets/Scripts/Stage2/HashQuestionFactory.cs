using System.Collections.Generic;

using UnityEngine;

namespace Stage2
{
    public class HashQuestionFactory
    {
        private List<IHashFunction> hashFunctions;
        private System.Random rng = new System.Random();

        public HashQuestionFactory()
        {
            hashFunctions = new List<IHashFunction>
            {
                new ModHash(5),
                new ModHash(7),
                new ModHash(11)
            };
        }

        public HashQuestion GenerateRandomQuestion()
        {
            int key = rng.Next(10, 100); // Random key in range 10–99
            int index = rng.Next(hashFunctions.Count);
            IHashFunction chosenFunction = hashFunctions[index];

            return new HashQuestion(key, chosenFunction);
        }
    }
}
