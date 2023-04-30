[System.Serializable]
public class FloatGradeCriteria : GradeCriteria
{
    public float Value;
    public CriteriaOperation Operation;

    public FloatGradeCriteria(string name, float value)
    {
        Name = name;
        Value = value;
    }

    public FloatGradeCriteria(float value)
    {
        Value = value;
    }

    public FloatGradeCriteria() { }

    public override bool CompareTo(GradeCriteria criteria)
    {
        FloatGradeCriteria number = (FloatGradeCriteria)criteria;

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
