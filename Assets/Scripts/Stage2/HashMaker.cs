using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stage2
{
    //Design pattern: Strategy Pattern
    public interface IHashFunction
    {
        int Compute(int key);          
        string GetFormula();          
    }

    public class Mod5Hash : IHashFunction
    {
        public int Compute(int key)
        {
            return key % 5;
        }

        public string GetFormula()
        {
            return "h(k) = k % 5";
        }
    }

    public class Mod7Hash : IHashFunction
    {
        public int Compute(int key)
        {
            return key % 7;
        }

        public string GetFormula()
        {
            return "h(k) = k % 7";
        }
    }

    public class Mod11Hash : IHashFunction
    {
        public int Compute(int key)
        {
            return key % 11;
        }

        public string GetFormula()
        {
            return "h(k) = k % 11";
        }
    }

    //key, ftn,answer
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

    // question with random hash ftn
    public class HashQuestionFactory
    {
        private List<IHashFunction> hashFunctions;
        private System.Random rng = new System.Random();

        public HashQuestionFactory()
        {
            hashFunctions = new List<IHashFunction>
            {
                new Mod5Hash(),
                new Mod7Hash(),
                new Mod11Hash()
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
