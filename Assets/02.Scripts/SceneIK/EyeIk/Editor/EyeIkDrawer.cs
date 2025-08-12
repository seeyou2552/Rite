using UnityEditor;
using UnityEngine;

namespace CustomTimeline
{
    [CustomPropertyDrawer(typeof(EyeIkBehaviour))]
    public class EyeIkDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 1;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //SerializedProperty lookSpeedProp = property.FindPropertyRelative("lookSpeed");
            //SerializedProperty lookAtWeightProp = property.FindPropertyRelative("lookAtWeight");
            //SerializedProperty bodyWeightProp = property.FindPropertyRelative("bodyWeight");
            //SerializedProperty headWeightProp = property.FindPropertyRelative("headWeight");
            //SerializedProperty eyesWeightProp = property.FindPropertyRelative("eyesWeight");
            //SerializedProperty clampWeightProp = property.FindPropertyRelative("clampWeight");

            Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            //singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            //EditorGUI.PropertyField(singleFieldRect, lookSpeedProp);

            //singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            //EditorGUI.PropertyField(singleFieldRect, lookAtWeightProp);

            //singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            //EditorGUI.PropertyField(singleFieldRect, bodyWeightProp);

            //singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            //EditorGUI.PropertyField(singleFieldRect, headWeightProp);

            //singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            //EditorGUI.PropertyField(singleFieldRect, eyesWeightProp);

            //singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            //EditorGUI.PropertyField(singleFieldRect, clampWeightProp);
        }
    }
}