using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    TextMeshProUGUI tutorialText;
    private bool isFolding = false;
    private void Awake()
    {
        tutorialText = GameObject.Find("TutorialText").GetComponent<TextMeshProUGUI>();
        if (tutorialText != null)
        {
            tutorialText.text = Const.TutorialText;
        }
    }

    public void ToggleTutorial()
    {
        if (tutorialText == null) return;
        isFolding = !isFolding;
        if (isFolding)
        {
            tutorialText.text = Const.TutorialFoldText;
        }
        else
        {
            tutorialText.text = Const.TutorialText;
        }
    }
    
    
    
}
