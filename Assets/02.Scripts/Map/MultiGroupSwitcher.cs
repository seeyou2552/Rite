using System;
using UnityEngine;
public class MultiGroupSwitcher : MonoBehaviour
{
    [Serializable]
    public class ToggleGroup
    {
        [Header("�׷� �̸�")]
        public string groupName;
        [Header("������Ʈ �ֱ�")]
        public GameObject[] objects;
    }

    [Header("���� �׷�")]
    public ToggleGroup[] groups;
}
