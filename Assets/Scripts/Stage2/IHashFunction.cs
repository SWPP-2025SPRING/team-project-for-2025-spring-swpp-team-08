using UnityEngine;

namespace Stage2
{
    public interface IHashFunction
    {
        public int Compute(int key);
        public string GetFormula();
        public int GetDivisor();
    }
}
