using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
   작성일자 : 2025.06.16
   작성자 : 오수호
   타이틀 씬에서 사용되는 Candle을 조절하는 스크립트
   CandleLightController를 가져와서 사용
   
 
 */
public class CandleAdjust : MonoBehaviour
{
    [Header("����, �Ҳ�")]
    [SerializeField] private Light candleLight;
    public GameObject flame;


    [Header("���ڰŸ� �ֱ�")]
    public float flickerIntervalMin = 0.2f;
    public float flickerIntervalMax = 1f;

    public Image logoImage;

    private Coroutine flickerCoroutine;
    private float baseIntensity= 10f;
    private readonly List<Transform> monsters = new List<Transform>();


    #region �ڷ�ƾ
    public void StartFlicker()
    {
        if (flickerCoroutine == null)
        {
            candleLight.enabled = true;
            flame.SetActive(true);
            flickerCoroutine = StartCoroutine(Flicker());
        }
    }

    public void TurnOffLight()
    {
        candleLight.enabled = false;
        flame.SetActive(false);
        Color color = logoImage.color;
        color.a = 0f;
        logoImage.color = color;
    }

    public void TurnOnLight()
    {
        candleLight.enabled = true;
        flame.SetActive(true);
        Color color = logoImage.color;
        color.a = 1f;
        logoImage.color = color;
    }

    public void StopFlicker()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
            candleLight.intensity = baseIntensity;
        }
    }

    public IEnumerator Flicker()
    {
        while (true)
        {
            float duration = Random.Range(flickerIntervalMin, flickerIntervalMax);
            // ���̵� �ƿ�
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                Color color = logoImage.color;
                color.a = Mathf.Lerp(1f, 0f, t / duration);
                logoImage.color = color;
                candleLight.intensity = Mathf.Lerp(baseIntensity, 0f, t / duration);
                yield return null;
            }
            // ������ ������
            candleLight.intensity = 0f;

            // ���̵� �� 
            duration = Random.Range(flickerIntervalMin, flickerIntervalMax);
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                Color color = logoImage.color;
                color.a = Mathf.Lerp(0f, 1f, t / duration);
                logoImage.color = color;
                candleLight.intensity = Mathf.Lerp(0f, baseIntensity, t / duration);
                yield return null;
            }
            // ������ ������
            candleLight.intensity = baseIntensity;
        }
    }
    #endregion 

    #region OnTrigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && !monsters.Contains(other.transform))
            monsters.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
            monsters.Remove(other.transform);
    }
    #endregion
}
