using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinetInteractable : MonoBehaviour, IInteractable
{
    [Header("���� ��ġ (ĳ��� ����)")]
    public Transform hidePoint;
    [Header("�ٽ� ���� ��ġ (ĳ��� ��)")]
    public Transform playerPoint;

    // ���� ���� ���� ����
    public bool isHidden = false;

    [HideInInspector]
    public Transform lastPoint;
    public void Interact()
    {
        if (!isHidden)
        {
            // ����
            lastPoint = hidePoint;
            isHidden = true;
            Player.Instance.controller.SitDown();
            Player.Instance.controller.currentState = PlayerState.Hiding;
            Player.Instance.controller.DisableMovementFor(0.1f);
            Player.Instance.transform.position = lastPoint.position;
            Player.Instance.transform.rotation = lastPoint.rotation;
            SoundManager.Instance.PlaySFX("Open_Drawer");
        }
        else
        {
            // ������
            lastPoint = playerPoint;
            isHidden = false;
            Player.Instance.controller.StartStandUp();
            Player.Instance.controller.currentState = PlayerState.Standing;
            Player.Instance.controller.DisableMovementFor(0.1f);
            Player.Instance.transform.position = lastPoint.position;
            SoundManager.Instance.PlaySFX("Close_Drawer");
        }
    }
}
