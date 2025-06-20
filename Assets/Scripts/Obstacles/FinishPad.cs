using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPad : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("FinishPad");
            GameManager.Instance.playManager.FinishGame();
        }
    }
}
