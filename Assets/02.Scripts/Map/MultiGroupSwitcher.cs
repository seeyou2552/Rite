using System;
using UnityEngine;
public class MultiGroupSwitcher : MonoBehaviour
{
    [Serializable]
    public class ToggleGroup
    {
        [Header("그룹 이름")]
        public string groupName;
        [Header("오브젝트 넣기")]
        public GameObject[] objects;
    }

    [Header("개별 그룹")]
    public ToggleGroup[] groups;
}
