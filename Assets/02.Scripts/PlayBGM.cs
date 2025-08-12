using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBGM : MonoBehaviour
{
    [SerializeField] private string clipName;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(clipName, true);
    }

    private void OnDestroy()
    {
        SoundManager.Instance.StopBGM();
    }
}
