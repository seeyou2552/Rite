using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleLightController : MonoBehaviour
{
    [Header("����, �Ҳ�")]
    [SerializeField] private Light candleLight;
    public GameObject flame;

    [Header("�� �Ÿ� ���ϸ� ���ڰŸ���")]
    public float flickerDistance = 10f;

    [Header("�� �Ÿ� ���ϸ� �� ������")]
    public float offDistance = 2f;

    [Header("���ڰŸ� �ֱ�")]
    public float flickerIntervalMin = 0.2f;
    public float flickerIntervalMax = 1f;


    private Coroutine flickerCoroutine;
    private float baseIntensity= 5f;
    private readonly List<Transform> monsters = new List<Transform>();

    void Update()
    {
        LightState();
    }

    void LightState()
    {
        // �ֺ��� ���Ͱ� ����
        if (monsters.Count == 0)
        {
            StopFlicker();
            candleLight.enabled = true;
            flame.SetActive(true);
            candleLight.intensity = baseIntensity;
            return;
        }

        // ���� ����� ���ͱ��� �Ÿ� ���
        float nearest = float.MaxValue;
        Vector3 pos = transform.position;
        for (int i = monsters.Count - 1; i >= 0; i--)
        {
            if (monsters[i] == null)
            {
                monsters.RemoveAt(i);
                continue;
            }

            float dist = Vector3.Distance(pos, monsters[i].position);
            if (dist < nearest) nearest = dist;
        }

        if (nearest <= offDistance)
        {
            // ������ Off
            StopFlicker();
            candleLight.enabled = false;
            flame.SetActive(false);
        }
        else if (nearest <= flickerDistance)
        {
            // ������
            StartFlicker();
        }
        else
        {
            // ���� 
            StopFlicker();
            candleLight.enabled = true;
            flame.SetActive(true);
            candleLight.intensity = baseIntensity;
        }
    }

    #region �ڷ�ƾ
    private void StartFlicker()
    {
        if (flickerCoroutine == null)
        {
            candleLight.enabled = true;
            flame.SetActive(true);
            flickerCoroutine = StartCoroutine(Flicker());
        }
    }

    private void StopFlicker()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
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
                candleLight.intensity = Mathf.Lerp(baseIntensity, 0f, t / duration);
                yield return null;
            }
            // ������ ������
            candleLight.intensity = 0f;

            // ���̵� �� 
            duration = Random.Range(flickerIntervalMin, flickerIntervalMax);
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
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
