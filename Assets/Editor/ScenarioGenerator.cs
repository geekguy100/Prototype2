using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FileLoading;
using UnityEditor;
using UnityEngine;

public class ScenarioGenerator : EditorWindow
{
    
    private static Scenarios scenarios = new Scenarios();

    private string fileName;
    private string setup;
    
    public List<Choice> choiceList = new List<Choice>();
    
    [MenuItem("Secnarios/Create")] 
    public static void ShowWindow()
    {
        Array.Resize(ref scenarios.Setups, 1);
        scenarios.Setups[0] = new Scenario();
        EditorWindow.GetWindow(typeof(ScenarioGenerator));
    }
    
    
    private void OnGUI()
    {
        ScriptableObject target = this;
        SerializedObject so;
        GUILayout.Label("File name: " , EditorStyles.boldLabel);
        fileName = EditorGUILayout.TextField(fileName);
        GUILayout.Label("Setup: ", EditorStyles.boldLabel);
        setup = EditorGUILayout.TextArea(setup);
        
        GUILayout.Label("Choices", EditorStyles.boldLabel);
        so = new SerializedObject(target);
        SerializedProperty property = so.FindProperty("choiceList");
        EditorGUILayout.PropertyField(property, true);
        so.ApplyModifiedProperties();


        if (GUILayout.Button("Save Setup"))
        {
            scenarios.Setups[0].Setup = setup;
            Array.Resize(ref scenarios.Setups[0].Decisions, choiceList.Count);
            for (int i = 0; i < choiceList.Count; ++i)
            {
                scenarios.Setups[0].Decisions[i] = new Choices();
                scenarios.Setups[0].Decisions[i].Approval = choiceList[i].approval;
                scenarios.Setups[0].Decisions[i].Efficiency = choiceList[i].efficiency;
                scenarios.Setups[0].Decisions[i].Environment = choiceList[i].environment;
                scenarios.Setups[0].Decisions[i].Cost = choiceList[i].cost;
            }

            string json = JsonUtility.ToJson(scenarios);
            Debug.Log(json);
            string path = "Assets/Resources/Scenarios/" + fileName + ".json";
            if (File.Exists(path))
            {
                json = json.Substring(12);
                string currentFile = File.ReadAllText(path);
                currentFile = currentFile.Substring(0, (currentFile.Length - 4));
                currentFile += ",{" + json;
                File.WriteAllText(path, currentFile);
                
            }
            else
            {
                FileStream file = File.Create(path);
                file.Close();
                File.WriteAllText(path, json);
            }
            
            Debug.Log("Save successful!");
        }
        
    }
}
