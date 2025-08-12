using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleInteract : MonoBehaviour, IInteractable
{
    [Header("점등 상태")]
    [SerializeField] private bool isLit = false;

    // 외부에서 상태를 확인할 수 있도록 프로퍼티 추가
    public bool IsLit => isLit;

    [Header("불 켜지는 효과")]
    public GameObject candleEffect;
    public void Interact()
    {
        // �̹� ���� ���¸� �� �̻� ó������ ����
        if (isLit)
            return;

        // ��ȭ ȿ�� Ȱ��ȭ
        if (candleEffect != null)
        {
            SoundManager.Instance.PlaySFX("CandleTurnOn");
            candleEffect.SetActive(true);
        }

        // ���� ��ȣ�ۿ� ����
        isLit = true;
    }
}
