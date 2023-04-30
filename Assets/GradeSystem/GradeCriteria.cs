using System;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public abstract class GradeCriteria
{
    /// <summary>
    /// The name of the criteria. If the criteria name matches with this one, it will be checked. Otherwise, ignored.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// Compare a criteria to another. The criteria passed into this method is the same type as the class.
    /// </summary>
    /// <param name="criteria">The criteria to compare. Should be casted to the containing class type.</param>
    /// <returns>A bool, true if the comparison is successful.</returns>
    public abstract bool CompareTo(GradeCriteria criteria);

#if UNITY_EDITOR
    /// <summary>
    /// Display this grade data to the editor.
    /// <br/> This method can be overridden, but should be only compiled in the editor under a preprocessor flag.
    /// </summary>
    /// <param name="gradeData">The grade data that this criteria belongs to.</param>
    public virtual void Display(GradeData gradeData) { DisplayEditorGUIFromType(gradeData); }

    private void DisplayEditorGUIFromType(GradeData gradeData)
    {
        Type gcType = GetType();
        FieldInfo[] fields = gcType.GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            object obj = field.GetValue(this);

            Type type = field.FieldType;

            GUIContent gui = new GUIContent(field.Name);

            object newObject = GetObjectFromType(gui, obj, type);

            if (newObject == null)
                continue;

            OverwriteValue(field, obj, newObject, gradeData);
        }
    }

    private void OverwriteValue(FieldInfo field, object oldObj, object newObj, GradeData dirty)
    {
        if (!oldObj.Equals(newObj)) // == doesn't work here
        {
            field.SetValue(this, newObj);
            EditorUtility.SetDirty(dirty);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }
    }

    private object GetObjectFromType(GUIContent gui, object obj, Type type)
    {
        if (type.IsEnum)
            return EditorGUILayout.EnumPopup(gui, (Enum)obj);
        else if (type == typeof(string))
            return EditorGUILayout.DelayedTextField(gui, obj.ToString());
        else if (type == typeof(int))
            return EditorGUILayout.DelayedIntField(gui, (int)obj);
        else if (type == typeof(float))
            return EditorGUILayout.DelayedFloatField(gui, (float)obj);
        else if (type == typeof(double))
            return EditorGUILayout.DelayedDoubleField(gui, (double)obj);
        else if (type == typeof(bool))
            return EditorGUILayout.Toggle(gui, (bool)obj);
        else
        {
            string objString = obj.ToString();
            string s = EditorGUILayout.DelayedTextField(gui, objString);
            if (s != objString)
            {
                try
                {
                    return Convert.ChangeType(s, type);
                }
                catch (Exception)
                {
                }
            }
        }

        return null;
    }
#endif
}
