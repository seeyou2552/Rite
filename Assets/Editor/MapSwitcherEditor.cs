using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiGroupSwitcher))]
public class MapSwitcherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // �⺻ �ν����� ������
        DrawDefaultInspector();
        // ��ư ����
        EditorGUILayout.Space();

        var manager = (MultiGroupSwitcher)target;

        // �� �׷� ��ư �׸���
        EditorGUILayout.LabelField("�׷캰 ��ư", EditorStyles.boldLabel);
        for (int i = 0; i < manager.groups.Length; i++)
        {
            var group = manager.groups[i];
            if (string.IsNullOrEmpty(group.groupName))
                group.groupName = "Group " + (i + 1);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(group.groupName, GUILayout.Width(60));
            if (GUILayout.Button("���� �ѱ�", GUILayout.Width(100)))
            {
                // ��� ����
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
