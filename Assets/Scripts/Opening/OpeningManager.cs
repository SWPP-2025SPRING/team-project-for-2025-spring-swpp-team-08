using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opening
{

    public class OpeningManager : MonoBehaviour
    {
        public OpeningSequence openingSequence;

        void Start()
        {
            StartCoroutine(PlayOpening());
        }

        IEnumerator PlayOpening()
        {
            yield return openingSequence.PlaySequence();
            StartTutorial();
        }

        void StartTutorial()
        {
            GetPlayManager().StartGame();
        }

        private PlayManager GetPlayManager()
        {
            return GameManager.Instance.playManager;
        }
    }
}
