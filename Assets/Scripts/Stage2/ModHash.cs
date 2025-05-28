using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stage2
{
    public class ModHash : IHashFunction
    {
        private int _divisor;

        public ModHash(int divisor)
        {
            _divisor = divisor;
        }

        public int Compute(int key)
        {
            return key % _divisor;
        }

        public string GetFormula()
        {
            return "h(k) = k % " + _divisor;
        }

    }
}
