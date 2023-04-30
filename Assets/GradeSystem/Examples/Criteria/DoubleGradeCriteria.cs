[System.Serializable]
public class DoubleGradeCriteria : GradeCriteria
{
    public double Value;
    public CriteriaOperation Operation;

    public DoubleGradeCriteria(string name, double value)
    {
        Name = name;
        Value = value;
    }

    public DoubleGradeCriteria(double value)
    {
        Value = value;
    }

    public DoubleGradeCriteria() { }

    public override bool CompareTo(GradeCriteria criteria)
    {
        DoubleGradeCriteria number = (DoubleGradeCriteria)criteria;

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
