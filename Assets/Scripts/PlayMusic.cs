using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public AudioClip bgmClip;

    void Start()
    {
        if (bgmClip != null)
        {
            GameManager.Instance.PlayBgm(bgmClip);
        }
    }
}
