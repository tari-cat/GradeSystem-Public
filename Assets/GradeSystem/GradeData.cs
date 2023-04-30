using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "New Grade", menuName = "Create New Grade")]
public class GradeData : ScriptableObject
{
    /// <summary>
    /// The group the grade belongs to. This could be a level, system name, etc.
    /// </summary>
    public string Group = string.Empty;

    /// <summary>
    /// The name of the grade data. This is usually a letter grade, or rank.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// The priority of the grade data. Higher priority means it will be processed first.
    /// </summary>
    public int Priority;

#if UNITY_EDITOR
    [HideInInspector, NonSerialized]
    private List<GradeCriteria> _criteria;
#endif

    /// <summary>
    /// The criteria the grade data contains. This is serialized by reference to contain inheriting types.
    /// </summary>
    [SerializeReference]
    public GradeCriteria[] Criteria = new GradeCriteria[0];

    /// <summary>
    /// Evaluate an array of grade criteria. Criteria will only be compared if they have the same type and name.
    /// </summary>
    /// <param name="criteriaToCheck">The array of criteria to compare against.</param>
    /// <returns>A bool, true if every comparison is successful.</returns>
    public bool Evaluate(params GradeCriteria[] criteriaToCheck)
    {
        for (int i = 0; i < criteriaToCheck.Length; i++)
        {
            GradeCriteria checkCriteria = criteriaToCheck[i];
            Type checkType = checkCriteria.GetType();

            for (int j = 0; j < Criteria.Length; j++)
            {
                GradeCriteria gradeCriteria = Criteria[j];
                Type gradeCriteriaType = gradeCriteria.GetType();
                if (gradeCriteriaType == checkType && gradeCriteria.Name == checkCriteria.Name)
                {
                    if (!gradeCriteria.CompareTo(checkCriteria))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Add a grade criteria to the data's serialized object.
    /// </summary>
    /// <param name="criteria">The grade criteria to add.</param>
    public void AddCriteria(GradeCriteria criteria)
    {
        _criteria = Criteria.ToList();
        _criteria.Add(criteria);
        Criteria = _criteria.ToArray();
    }

    /// <summary>
    /// Remove a grade criteria from the data's serialized object.
    /// </summary>
    /// <param name="criteria">The grade criteria index to remove.</param>
    public void RemoveCriteria(int index)
    {
        _criteria = Criteria.ToList();
        _criteria.RemoveAt(index);
        Criteria = _criteria.ToArray();
    }
#endif
}
