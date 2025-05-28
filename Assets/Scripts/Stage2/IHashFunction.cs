using UnityEngine;

namespace Stage2
{
    public interface IHashFunction
    {
        int Compute(int key);
        string GetFormula();
    }
}
