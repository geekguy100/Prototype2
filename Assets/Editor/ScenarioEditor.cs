using System;
using System.Collections.Generic;
using FileLoading;
using UnityEditor;
using UnityEngine;

public class ScenarioEditor : EditorWindow
{
    private static Scenarios scenarios = new Scenarios();

    private string fileName;
    private string setup;
    private string iconName;
    private int id;
    
    public List<Choices> choiceList = new List<Choices>();
    
    [MenuItem("Scenarios/Edit")] 
    public static void ShowWindow()
    {
        TextAsset text = Resources.Load<TextAsset>("Scenarios/Scenarios");
        scenarios = JsonUtility.FromJson<Scenarios>(text.text);
        EditorWindow.GetWindow(typeof(ScenarioEditor));
    }

    private void OnGUI()
    {
        ScriptableObject target = this;
        SerializedObject so;
        GUILayout.Label("Setups", EditorStyles.boldLabel);
        so = new SerializedObject(target);
        SerializedProperty property = so.FindProperty("scenarios");
        EditorGUILayout.PropertyField(property, true);
        so.ApplyModifiedProperties();
    }
}
