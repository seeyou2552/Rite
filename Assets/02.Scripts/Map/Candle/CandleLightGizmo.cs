using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleLightGizmo : MonoBehaviour
{
    private CandleLightController controller;

    void OnDrawGizmos()
    {
        if (controller == null)
            controller = GetComponent<CandleLightController>();
        if (controller == null)
            return;

        Vector3 pos = transform.position;
        // �����Ÿ��� ���� (Ȳ��)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, controller.flickerDistance);
        // ���� Off ���� (������)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, controller.offDistance);
    }
}
