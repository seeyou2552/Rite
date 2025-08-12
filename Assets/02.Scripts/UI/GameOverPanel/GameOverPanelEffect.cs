using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelEffect : MonoBehaviour
{
    public Image bloodImage;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI retryText;
    public Button retryButton;
    
    private bool isGameOver = false;

    private void Awake()
    {
        ActiveGameOverPanel();
    }

    [ContextMenu("게임오버패널 실행")]
    public void ActiveGameOverPanel()
    {
        if (isGameOver) return;
        isGameOver = true;
        this.gameObject.SetActive(true);
        StartCoroutine(EffectCoroutine());
        Cursor.lockState = CursorLockMode.None;
        
    }

    IEnumerator FadeInText(TextMeshProUGUI text)
    {
        Color c = text.color;
        while (c.a < 0.98f)
        {
            c.a += Time.deltaTime;
            text.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator SpreadBloodEffect()
    {
        bloodImage.gameObject.SetActive(true);
        SoundManager.Instance.PlaySFX(Const.BloodSquish);
        bloodImage.fillAmount = 0.3f;
        yield return new WaitForSeconds(0.1f);
        bloodImage.fillAmount = 0.6f;
        yield return new WaitForSeconds(0.1f);
        bloodImage.fillAmount = 0.9f;
        yield return new WaitForSeconds(0.1f);
        bloodImage.fillAmount = 1f;
        yield return new WaitForSeconds(1f);
    }

    IEnumerator EffectCoroutine()
    {
        StartCoroutine(SpreadBloodEffect());
        yield return new WaitForSeconds(1.6f);
        gameOverText.gameObject.SetActive(true);
        StartCoroutine(FadeInText(gameOverText));
        yield return new WaitForSeconds(2f);
        retryButton.gameObject.SetActive(true);
        retryText.gameObject.SetActive(true);
        StartCoroutine(FadeInText(retryText));
        
        
    }
    
    
    

}
