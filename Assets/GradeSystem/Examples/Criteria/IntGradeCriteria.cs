[System.Serializable]
public class IntGradeCriteria : GradeCriteria
{
    public int Value;
    public CriteriaOperation Operation;

    public IntGradeCriteria(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public IntGradeCriteria(int value)
    {
        Value = value;
    }

    public IntGradeCriteria() { }

    public override bool CompareTo(GradeCriteria criteria)
    {
        IntGradeCriteria number = (IntGradeCriteria)criteria;

        return Operation switch
        {
            CriteriaOperation.EqualTo => number.Value == Value,
            CriteriaOperation.NotEqualTo => number.Value != Value,
            CriteriaOperation.GreaterThan => number.Value > Value,
            CriteriaOperation.LessThan => number.Value < Value,
            CriteriaOperation.GreaterThanOrEqualTo => number.Value >= Value,
            CriteriaOperation.LessThanOrEqualTo => number.Value <= Value,
            _ => false
        };
    }

#if UNITY_EDITOR
    public override void Display(GradeData gradeData)
    {
        base.Display(gradeData);
    }
#endif
}
