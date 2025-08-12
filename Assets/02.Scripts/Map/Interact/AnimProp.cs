using System.Collections;
using UnityEngine;
using static UnityEditor.MaterialProperty;

public interface IInteractable
{
    void Interact();
}
public enum PropType { Table, Drawer, Door, Shelf }
public class AnimProp : MonoBehaviour, IInteractable
{
    //플레이어가 상호작용 가능한 곳을 바라보면 상호작용 가능
    public Animator animator;

    [Header("Animation Durations (초)")]
    public float aniDuration = 1f;
    [Tooltip("오브젝트 타입 설정")]
    public PropType propType;


    public bool isOpen = false;
    private bool isAnimating = false;

    public void Interact()
    {
        if (isAnimating)
            return;
        if (!isOpen)
        {
            animator.Play("A");
            PlayOpenSFX();
        }
        else
        {
            animator.Play("B");
            PlayCloseSFX();
        }

        StartCoroutine(ResetAnim(aniDuration));
        isOpen = !isOpen;
    }

    private void PlayOpenSFX()
    {
        switch (propType)
        {
            case PropType.Table:
                SoundManager.Instance.PlaySFX("Tabletop_Open");
                break;
            case PropType.Shelf:
            case PropType.Drawer:
                SoundManager.Instance.PlaySFX("Closet_Open");
                break;
            case PropType.Door:
                SoundManager.Instance.PlaySFX("MetalDoor_Open");
                break;
        }
    }

    private void PlayCloseSFX()
    {
        switch (propType)
        {
            case PropType.Table:
                SoundManager.Instance.PlaySFX("Tabletop_Close");
                break;
            case PropType.Shelf:
            case PropType.Drawer:
                SoundManager.Instance.PlaySFX("Closet_Close");
                break;
            case PropType.Door:
                SoundManager.Instance.PlaySFX("MetalDoor_Close");
                break;
        }
    }
    private IEnumerator ResetAnim(float duration)
    {
        isAnimating = true;
        yield return new WaitForSeconds(duration);
        isAnimating = false;
    }
}
