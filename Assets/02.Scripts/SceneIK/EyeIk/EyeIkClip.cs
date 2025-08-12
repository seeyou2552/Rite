using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CustomTimeline
{
    [Serializable]
    public class EyeIkClip : PlayableAsset, ITimelineClipAsset
    {
        public EyeIkBehaviour template = new EyeIkBehaviour();
        public ExposedReference<Transform> currentTarget;

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<EyeIkBehaviour>.Create(graph, template);
            EyeIkBehaviour clone = playable.GetBehaviour();
            clone.currentTarget = currentTarget.Resolve(graph.GetResolver());
            return playable;
        }
    }
}