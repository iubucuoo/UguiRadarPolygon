using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI.Extensions;

[CustomEditor(typeof(UIPolygon), true)]
[CanEditMultipleObjects]
public class UIPolygonEditor : GraphicEditor
{
    SerializedProperty m_Texture;
    SerializedProperty fill;
    SerializedProperty thickness;
    SerializedProperty sides;
    SerializedProperty rotation;
    SerializedProperty VerticesDistances;
    SerializedProperty pointSprite;
    SerializedProperty lineSprite;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_Texture = serializedObject.FindProperty("m_Texture");
        fill = serializedObject.FindProperty("fill");
        thickness = serializedObject.FindProperty("thickness");
        sides = serializedObject.FindProperty("sides");
        rotation = serializedObject.FindProperty("rotation");
        VerticesDistances = serializedObject.FindProperty("VerticesDistances");
        pointSprite = serializedObject.FindProperty("pointSprite");
        lineSprite = serializedObject.FindProperty("lineSprite");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_Texture);
        EditorGUILayout.PropertyField(fill);
        EditorGUILayout.PropertyField(thickness);
        EditorGUILayout.PropertyField(sides);
        EditorGUILayout.PropertyField(rotation);
        EditorGUILayout.PropertyField(VerticesDistances);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(pointSprite);
        EditorGUILayout.PropertyField(lineSprite);

        UIPolygon radar = target as UIPolygon;
        if (radar != null)
        {
            if (GUILayout.Button("生成顶点"))
            {
                radar.InitPoints();
            }
            if (GUILayout.Button("生成顶点连线"))
            {
                radar.InitLines();
            }
        }
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

    }
}