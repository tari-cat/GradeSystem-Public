#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;

public class CriteriaDeletionProcessor : UnityEditor.AssetModificationProcessor
{
    static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt)
    {
        Type assetType = AssetDatabase.GetMainAssetTypeAtPath(path);

        if (assetType != typeof(MonoScript))
            return AssetDeleteResult.DidNotDelete;

        MonoScript script = (MonoScript)AssetDatabase.LoadAssetAtPath(path, assetType);
        Type scriptClass = script.GetClass();

        Type gradeCriteriaType = typeof(GradeCriteria);
        if (scriptClass.BaseType == gradeCriteriaType)
        {
            if (AnyGradeCriteriaContainsType(scriptClass))
            {
                bool option = EditorUtility.DisplayDialog(
                    "hey man",
                    "You're trying to delete a GradeCriteria script while some GradeData depends on it. If you delete this, I'll remove the criteria for you.",
                    "Delete",
                    "Do Not Delete"
                );

                if (option)
                {
                    RemoveAllGradeCriteriaOfType(scriptClass);
                    return AssetDeleteResult.DidNotDelete;
                }
                else
                {
                    return AssetDeleteResult.FailedDelete;
                }
            }
        }
        return AssetDeleteResult.DidNotDelete;
    }

    private static bool AnyGradeCriteriaContainsType(Type type)
    {
        GradeData[] data = Resources.LoadAll<GradeData>("");

        for (int i = 0; i < data.Length; i++)
        {
            GradeData gradeData = data[i];

            for (int j = 0; j < gradeData.Criteria.Length; j++)
            {
                if (gradeData.Criteria[j].GetType() == type)
                    return true;
            }
        }

        return false;
    }

    private static void RemoveAllGradeCriteriaOfType(Type type)
    {
        GradeData[] data = Resources.LoadAll<GradeData>("");
        bool dirty = false;
        for (int i = 0; i < data.Length; i++)
        {
            GradeData gradeData = data[i];

            for (int j = 0; j < gradeData.Criteria.Length; j++)
            {
                if (gradeData.Criteria[j].GetType() == type)
                {
                    gradeData.RemoveCriteria(j);
                    j--;
                    EditorUtility.SetDirty(gradeData);
                    dirty = true;
                }
            }
        }

        if (dirty)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

#endif