using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CustomTimeline
{
    public class EyeIkMixerBehaviour : PlayableBehaviour
    {
        EyeIk trackBinding;

        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as EyeIk;

            if (!trackBinding || !trackBinding.gameObject.activeSelf)
                return;

            int inputCount = playable.GetInputCount();
            int currentInputCount = 0;
            float positionTotalWeight = 0f;

            Vector3 defaultPosition = trackBinding.lookAtObj.position;
            Vector3 targetPosition = Vector3.zero;
            Vector3 blendedPosition = Vector3.zero;
            float targetLookAt = 0f;
            float blendedLookAt = 0f;

            // update
            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<EyeIkBehaviour> inputPlayable = (ScriptPlayable<EyeIkBehaviour>)playable.GetInput(i);
                EyeIkBehaviour input = inputPlayable.GetBehaviour();

                if (inputWeight > 0f)
                    currentInputCount++;

                targetLookAt += inputWeight * (inputWeight > 0 ? 1 : 0);
                targetPosition += inputWeight * (inputWeight > 0 ? input.currentTarget.position : (trackBinding.animator.bodyPosition + trackBinding.animator.bodyRotation * new Vector3(0, 0.5f, 1)));

                positionTotalWeight += inputWeight;
            }

            // onClip == false
            if (currentInputCount == 0)
            {
                blendedLookAt = 0;
                blendedPosition = targetPosition + (trackBinding.animator.bodyPosition + trackBinding.animator.bodyRotation * new Vector3(0, 0.5f, 1)) * (1f - positionTotalWeight);
            }
            // onClip == true
            else
            {
                trackBinding.ikActivate = true;
                blendedLookAt = targetLookAt;
                blendedPosition = targetPosition + defaultPosition * (1f - positionTotalWeight);

                //trackBinding.lookAtWeight = input.lookAtWeight;
                //trackBinding.bodyWeight = input.bodyWeight;
                //trackBinding.headWeight = input.headWeight;
                //trackBinding.eyesWeight = input.eyesWeight;
                //trackBinding.clampWeight = input.clampWeight;
            }

            trackBinding.lookAtObj.position = blendedPosition;
            trackBinding.lookAtWeight = blendedLookAt;
        }
    }
}