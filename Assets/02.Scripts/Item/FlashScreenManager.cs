using UnityEngine;
using System.Collections;

public class FlashScreenManager : MonoBehaviour
{
    public CanvasGroup flashScreen; // 인스펙터에서 연결

    public void Flash()
    {
        Debug.Log("Flash called!");
        StartCoroutine(FlashScreenEffect());
    }

    private IEnumerator FlashScreenEffect()
    {
        flashScreen.alpha = 1f;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            flashScreen.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        flashScreen.alpha = 0f;
    }
}
