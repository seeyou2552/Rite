using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerRandomSpawn : MonoBehaviour
{
    [Header("��Ȱ ����Ʈ ���")]
    public Transform[] revivalPoints;

    [Header("�÷��̾�")]
    [Tooltip("�������� �÷��̾� Transform (�밳 ĳ����) ")]
    public Transform playerTransform;

    public void RespawnPlayer()
    {
        if (revivalPoints == null || revivalPoints.Length == 0)
        {
            return;
        }

        // ���� ���� ����
        Transform chosen = revivalPoints[Random.Range(0, revivalPoints.Length)];

        // CharacterController�� ������ �浹 ��Ȱ��ȭ
        if (playerTransform == null)
        {
            playerTransform = Player.Instance.transform;
        }
        var cc = playerTransform.GetComponent<CharacterController>();
        
        if (cc != null) cc.enabled = false;

        // ��ġ �� ȸ�� ����
        playerTransform.position = chosen.position;
        playerTransform.rotation = chosen.rotation;

        // CharacterController �ٽ� Ȱ��ȭ
        if (cc != null) cc.enabled = true;
    }
}
