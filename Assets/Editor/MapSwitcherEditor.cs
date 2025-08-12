using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiGroupSwitcher))]
public class MapSwitcherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 렌더링
        DrawDefaultInspector();
        // 버튼 영역
        EditorGUILayout.Space();

        var manager = (MultiGroupSwitcher)target;

        // 각 그룹 버튼 그리기
        EditorGUILayout.LabelField("그룹별 버튼", EditorStyles.boldLabel);
        for (int i = 0; i < manager.groups.Length; i++)
        {
            var group = manager.groups[i];
            if (string.IsNullOrEmpty(group.groupName))
                group.groupName = "Group " + (i + 1);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(group.groupName, GUILayout.Width(60));
            if (GUILayout.Button("끄기 켜기", GUILayout.Width(100)))
            {
                // 토글 로직
                foreach (var go in group.objects)
                {
                    if (go != null)
                        go.SetActive(!go.activeSelf);
                }
                EditorUtility.SetDirty(manager);
            }
            GUILayout.EndHorizontal();
        }
    }
}
