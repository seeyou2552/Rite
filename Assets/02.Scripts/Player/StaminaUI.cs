using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Transform fill;
    public CanvasGroup staminaGroup;
    public float fadeSpeed = 2f;

    private bool isFadingOut = false;


    void Update()
    {
        float PlayerStamina = Player.Instance.condition.stamina;
        float PlayerMaxStamina = Player.Instance.condition.maxStamina;

        float ratio = PlayerStamina / PlayerMaxStamina;
        ratio = Mathf.Clamp01(ratio);
        fill.localScale = new Vector3(ratio, 1f, 1f);

        if (ratio < 1f)
        {
            isFadingOut = false;
            staminaGroup.alpha = Mathf.MoveTowards(staminaGroup.alpha, 1f, Time.deltaTime * fadeSpeed);
            staminaGroup.blocksRaycasts = true;
            staminaGroup.interactable = true;
        }
        else
        {
            if (!isFadingOut && staminaGroup.alpha >= 0.99f)
                isFadingOut = true;

            if (isFadingOut)
            {
                staminaGroup.alpha = Mathf.MoveTowards(staminaGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
                staminaGroup.blocksRaycasts = false;
                staminaGroup.interactable = false;
            }
        }
    }
}
