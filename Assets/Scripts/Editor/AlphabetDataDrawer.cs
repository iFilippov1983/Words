using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

[CustomEditor(typeof(AlphabetData))]
[CanEditMultipleObjects]
[Serializable]
public class AlphabetDataDrawer : Editor
{
    private ReorderableList AlphabetPlainList;
    private ReorderableList AlphabetNormalList;
    private ReorderableList AlphabetHighlightedList;
    private ReorderableList AlphabetWrongList;

    private void OnEnable()
    {
        InitializeReordableList(ref AlphabetPlainList, "AlphabetPlane", "Alphabet Plane");                      //name of AlphabetData.AlphabetPlane public List
        InitializeReordableList(ref AlphabetNormalList, "AlphabetNormal", "Alphabet Normal");                   //name of AlphabetData.AlphabetNormal public List
        InitializeReordableList(ref AlphabetHighlightedList, "AlphabetHighlighted", "Alphabet Highlighted");    //name of AlphabetData.AlphabetHighlighted public List
        InitializeReordableList(ref AlphabetWrongList, "AlphabetWrong", "Alphabet Wrong");                      //name of AlphabetData.AlphabetWrong public List
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        AlphabetPlainList.DoLayoutList();
        AlphabetNormalList.DoLayoutList();
        AlphabetHighlightedList.DoLayoutList();
        AlphabetWrongList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void InitializeReordableList(ref ReorderableList list, string propertyName, string listLabel)
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty(propertyName), true, true, true, true);

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, listLabel);
        };

        var l = list;

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField
                (
                new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Letter"), //name of AlphabetData.Letter public field
                GUIContent.none
                );

            EditorGUI.PropertyField
                (
                new Rect(rect.x + 70, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Sprite"), //name of AlphabetData.Sprite public field
                GUIContent.none
                );
        };
    }
}
