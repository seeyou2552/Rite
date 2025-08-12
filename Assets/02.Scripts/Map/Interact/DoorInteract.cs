using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [Header("잠금 상태")]
    public bool isLocked = false;

    private AnimProp animProp;

    void Awake()
    {
        animProp = GetComponent<AnimProp>();
    }

    public void UnLock()
    {
        if (!isLocked) return;

        isLocked = false;
        // 잠금 해제 사운드 추가시 주석해제
        /*if (unlockSound != null)
        { 
            audioSource.clip = unlockSound;
            audioSource.Play();
        }*/
        Debug.Log("문이 열렸습니다!");
    }

    public void Interact()
    {
        if (!isLocked)
        {
            // 기존 문 열기/닫기 로직 호출
            animProp.Interact();
        }
        else
        {
            SoundManager.Instance.PlaySFX("key_twist");
            Debug.Log("문이 잠겨있습니다. 열쇠가 필요합니다.");
        }
    }
}