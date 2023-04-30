#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(GradeData))]
public class GradeDataEditor : Editor
{
    /// <summary>
    /// An array of types, all of which are inheriting from GradeCriteria.
    /// </summary>
    private static Type[] Types;

    private static List<GradeData> Grades;
    private static bool ShowPriorityOrder = false;

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void DidReloadScripts()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Types = FindDerivedTypes(assembly, typeof(GradeCriteria)).ToArray();

        Grades = Resources.LoadAll<GradeData>("").ToList();
        OrderGrades();
    }

    private static void AddGradeData(GradeData gradeData)
    {
        RemoveNullGrades();
        Grades.Add(gradeData);
        OrderGrades();
    }

    private static void RemoveGradeData(GradeData gradeData)
    {
        RemoveNullGrades();
        Grades.Remove(gradeData);
        OrderGrades();
    }

    private static void OrderGrades()
    {
        Grades = Grades.OrderByDescending(g => g.Priority).ToList();
    }

    private static void RemoveNullGrades()
    {
        for (int i = 0; i < Grades.Count; i++)
        {
            if (Grades[i] == null)
            {
                Grades.RemoveAt(i);
                i--;
            }
        }
    }

    private static List<GradeData> GetGrades(string group)
    {
        RemoveNullGrades();
        OrderGrades();
        List<GradeData> data = Grades;

        data = data.Where(t => t.Group == group).ToList();

        return data;
    }

    private static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
    {
        return assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && t != baseType);
    }

    public override void OnInspectorGUI()
    {
        GradeData data = (GradeData)target;

        if (!Grades.Contains(data))
        {
            Grades = Resources.LoadAll<GradeData>("").ToList();
            OrderGrades();
        }

        bool dirty = EditorUtility.IsDirty(target);

        if (dirty)
        {
            EditorGUILayout.HelpBox("Unsaved", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox("Saved", MessageType.Info);
        }

        if (GUILayout.Button("Save") && dirty)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            dirty = false;
        }

        GUIStyle boldStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            fontSize = 16
        };
        boldStyle.normal.textColor = new Color(.85f, .85f, .85f);

        GUIStyle textStyle = new GUIStyle
        {
            fontStyle = FontStyle.Normal,
            fontSize = 12
        };
        textStyle.normal.textColor = new Color(.7f, .7f, .7f);

        GUIStyle smallTextStyle = new GUIStyle
        {
            fontStyle = FontStyle.Normal,
            fontSize = 12
        };
        smallTextStyle.normal.textColor = new Color(.8f, .8f, .8f);

        GUIStyle smallWarningTextStyle = new GUIStyle
        {
            fontStyle = FontStyle.Normal,
            fontSize = 12
        };
        smallWarningTextStyle.normal.textColor = new Color(1f, 0.87f, 0.29f);

        GUIStyle warningStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            fontSize = 13
        };
        warningStyle.normal.textColor = new Color(1f, 0.87f, 0.29f);

        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("Grade Data Settings", boldStyle);
        EditorGUILayout.Space(10f);

        EditorGUI.BeginChangeCheck();

        string name = EditorGUILayout.TextField("Grade Name", data.Name);
        EditorGUILayout.LabelField("The name of the grade.", textStyle);
        EditorGUILayout.LabelField("Can be read and used by scripts.", textStyle);
        if (name != data.Name)
        {
            data.Name = name;
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Space(10f);

        string group = EditorGUILayout.TextField("Group Name", data.Group);
        EditorGUILayout.LabelField("The group the grade data belongs to.", textStyle);
        EditorGUILayout.LabelField("Groups are useful you want a certain set of grade data.", textStyle);
        if (group != data.Group)
        {
            data.Group = group;
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Space(10f);

        int priority = EditorGUILayout.IntField("Priority", data.Priority);
        EditorGUILayout.LabelField("The priority of the grade.", textStyle);
        EditorGUILayout.LabelField("Higher priority grades will be processed first.", textStyle);
        if (priority != data.Priority)
        {
            data.Priority = priority;
            EditorUtility.SetDirty(target);
        }

        ShowPriorityOrder = EditorGUILayout.Toggle("Show Priority Order", ShowPriorityOrder);
        if (ShowPriorityOrder)
        {
            List<GradeData> gradesInGroup = GetGrades(group);

            List<int> samePriorityIndex = new List<int>();

            int[] priorities = new int[gradesInGroup.Count];
            for (int i = 0; i < priorities.Length; i++)
            {
                priorities[i] = gradesInGroup[i].Priority;
            }

            for (int i = 0; i < priorities.Length; i++)
            {
                for (int j = 0; j < priorities.Length; j++)
                {
                    if (priorities[i] == priorities[j] && i != j)
                        samePriorityIndex.Add(i);
                }
            }

            EditorGUILayout.Space(5f);

            EditorGUILayout.LabelField($"Order of processing for group '{group}':", textStyle);

            for (int i = 0; i < gradesInGroup.Count; i++)
            {
                GradeData grade = gradesInGroup[i];

                bool conflict = samePriorityIndex.Contains(i);
                GUIStyle style = conflict ? smallWarningTextStyle : smallTextStyle;

                EditorGUILayout.LabelField($"{grade.Priority} | {grade.name} {(conflict ? "(duplicate priority)" : "")}", style);
            }
        }

        EditorGUILayout.Space(10f);
        GuiLine();
        EditorGUILayout.Space(10f);

        EditorGUILayout.LabelField("Criteria Constructors", boldStyle);
        EditorGUILayout.LabelField("To create your own criteria, inherit from GradeCriteria.", textStyle);
        EditorGUILayout.LabelField("Check the default criteria classes for examples.", textStyle);
        for (int i = 0; i < Types.Length; i++)
        {
            Type type = Types[i];

            ConstructorInfo blankConstructor = type.GetConstructor(Type.EmptyTypes);
            bool hasBlankConstructor = blankConstructor != null;

            if (!hasBlankConstructor)
                EditorGUILayout.LabelField($"> {type.Name} requires a blank constructor!", warningStyle);

            GUI.enabled = blankConstructor != null;

            if (GUILayout.Button($"New {type.Name} Criteria"))
            {
                object obj = Activator.CreateInstance(type);

                data.AddCriteria((GradeCriteria)obj);
                EditorUtility.SetDirty(target);
            }

            GUI.enabled = true;
        }

        EditorGUILayout.Space(10f);
        GuiLine();
        EditorGUILayout.Space(10f);

        EditorGUILayout.LabelField("Grade Criteria", boldStyle);
        EditorGUILayout.Space(10f);


        GradeCriteria[] gradeCriteria = data.Criteria;

        for (int i = 0; i < gradeCriteria.Length; i++)
        {
            GradeCriteria gc = gradeCriteria[i];
            if (gc == null)
                continue;

            Type gcType = gc.GetType();
            EditorGUILayout.LabelField($"Criteria #{i + 1}, {gcType.Name}", boldStyle);
            EditorGUILayout.Space(5f);

            gc.Display(data);

            EditorGUILayout.Space(10f);
            if (GUILayout.Button($"Remove Criteria"))
            {
                data.RemoveCriteria(i);
                i--;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space(10f);
            GuiLine();
            EditorGUILayout.Space(10f);
        }
    }

    private void GuiLine(int height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

}

#endif
