using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Grade
{
    private static GradeData[] Grades;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Grades = Resources.LoadAll<GradeData>("");

        Grades = Grades.OrderByDescending(g => g.Priority).ToArray();
    }

    /// <summary>
    /// Get a grade from the given folder of grades to search through, and criteria to evaluate.
    /// </summary>
    /// <param name="group">The group to search for GradeData.</param>
    /// <param name="criteria">An IEnumerable list of criteria to use to search for a grade. If an expected criteria isn't provided, it is ignored.</param>
    /// <returns>The Grade earned through the given group and criteria.</returns>
    public static GradeData Evaluate(string group, IEnumerable<GradeCriteria> criteria) => Evaluate(group, criteria.ToArray());

    /// <summary>
    /// Get a grade from the given folder of grades to search through, and criteria to evaluate.
    /// </summary>
    /// <param name="group">The group to search for GradeData.</param>
    /// <param name="criteria">An array of criteria to use to search for a grade. If an expected criteria isn't provided, it is ignored.</param>
    /// <returns>The Grade earned through the given group and criteria.</returns>
    public static GradeData Evaluate(string group, params GradeCriteria[] criteria)
    {
        GradeData[] data = GetGrades(group);
        if (data.Length <= 0)
            return null;

        for (int i = 0; i < data.Length; i++)
        {
            GradeData grade = data[i];

            if (grade.Evaluate(criteria))
            {
                return grade;
            }
        }

        return Grades[Grades.Length - 1];
    }

    /// <summary>
    /// Get all of the grades in a given folder.
    /// </summary>
    /// <param name="group">The group to search.</param>
    /// <returns>An array of the desired GradeData.</returns>
    public static GradeData[] GetGrades(string group)
    {
        GradeData[] data = Grades;

        data = data.Where(t => t.Group == group).OrderByDescending(g => g.Priority).ToArray();

        return data;
    }
}
