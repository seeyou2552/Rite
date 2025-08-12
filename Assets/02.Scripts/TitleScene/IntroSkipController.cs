using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IntroSkipController : MonoBehaviour
{
    public Image skipProgressBar;
    public TextMeshProUGUI skipText;
    public IntroManager introManager;
    /* 전체 인트로 스킵 변수 */
    private float skipSpeed = 0.5f;
    private float skipAmount = 0f;
    private bool isSkip = false;
    private bool isHolding = false;

    public Action<float> skipDelayAction;

    private void Awake()
    {
        skipDelayAction += UpdateSkipUI;
        UpdateSkipUI(skipAmount);
    }

    public void SkipIntro(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {   
            skipText.text = Const.introSkipIndicatiorIsPressed;
            isHolding = true;

        }

        if (context.canceled)
        {
            skipAmount = 0f;
            skipText.text = Const.introSkipIndicatiorIsCancled;
            isHolding = false;
        }
        skipDelayAction?.Invoke(skipAmount);
        
    }

    public void UpdateSkipUI(float amount)
    {
        skipProgressBar.fillAmount = amount;
        if (amount == 0f)
        {
            skipText.alpha = 255f;
        }
        else
        {
            skipText.alpha = amount * skipSpeed ;
        }
        
        if (amount > 1f)
        {
            amount = 1f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isHolding)
        {
            skipAmount += Time.deltaTime * skipSpeed;
            skipDelayAction?.Invoke(skipAmount);
            if (skipAmount > 1f)
            {
                introManager.LoadMainGameScene();
            }
        }
    }
}
