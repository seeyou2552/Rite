using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClipType
{
    None,
    Horror,
    Found
}

public class MonsterBGM : MonoBehaviour
{
    public ClipType clip;

    void Start()
    {
        SoundManager.Instance.SetVolume(SoundType.BGM, 1f);
        clip = ClipType.None;
    }

    void Update()
    {
        if ((clip == ClipType.None || clip == ClipType.Horror) && Player.Instance.condition.chaseCount > 0)
        {
            SoundManager.Instance.PlayBGM("found_bgm");
            clip = ClipType.Found;
        }
        else if (clip != ClipType.None && Player.Instance.condition.chaseCount == 0)
        {
            SoundManager.Instance.StopBGM();
            clip = ClipType.None;
        }
    }
}
