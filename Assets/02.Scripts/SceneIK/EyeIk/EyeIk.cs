using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTimeline
{
    [ExecuteInEditMode]
    public class EyeIk : MonoBehaviour
    {
        public Animator animator;
        public Transform lookAtObj = null;
        public bool ikActivate = false;

        public float lookAtWeight = 1.0f;
        public float bodyWeight = 0.3f;
        public float headWeight = 0.8f;
        public float eyesWeight = 0.5f;
        public float clampWeight = 0.5f;

        private void Start()
        {
            if (animator == null)
            {
                if (GetComponent<Animator>())
                    animator = GetComponent<Animator>();
                else
                    animator = gameObject.AddComponent<Animator>();
            }
                
        }

        void OnAnimatorIK(int layorIndex)
        {
            if (animator && animator.gameObject.activeSelf)
            {
                if (ikActivate)
                {
                    animator.SetLookAtWeight(lookAtWeight, bodyWeight, headWeight, eyesWeight, clampWeight);
                    if (lookAtObj != null)
                    {
                        animator.SetLookAtPosition(lookAtObj.position);
                    }
                    else
                    {
                        animator.SetLookAtWeight(0.0f);
                    }
                }
                else
                {
                    animator.SetLookAtWeight(0.0f);
                }
            }
        }
    }
}