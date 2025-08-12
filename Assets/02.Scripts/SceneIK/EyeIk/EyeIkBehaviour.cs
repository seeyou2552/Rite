using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CustomTimeline
{
    [Serializable]
    public class EyeIkBehaviour : PlayableBehaviour
    {
        public Transform currentTarget;

        public float lookSpeed = 1.0f;
        //public float lookAtWeight = 1.0f;
        //public float bodyWeight = 0.3f;
        //public float headWeight = 0.8f;
        //public float eyesWeight = 1.0f;
        //public float clampWeight = 0.5f;
    }
}