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
        // 깜빡거리기 범위 (황색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, controller.flickerDistance);
        // 완전 Off 범위 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, controller.offDistance);
    }
}
