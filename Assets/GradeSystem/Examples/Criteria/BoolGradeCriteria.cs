[System.Serializable]
public class BoolGradeCriteria : GradeCriteria
{
    public bool Value;
    public BoolCriteriaOperation Operation;

    public BoolGradeCriteria(string name, bool value)
    {
        Name = name;
        Value = value;
    }

    public BoolGradeCriteria(bool value)
    {
        Value = value;
    }

    public BoolGradeCriteria() { }

    public override bool CompareTo(GradeCriteria criteria)
    {
        BoolGradeCriteria boolean = (BoolGradeCriteria)criteria;

        return Operation switch
        {
            BoolCriteriaOperation.EqualTo => Value == boolean.Value,
            BoolCriteriaOperation.NotEqualTo => Value != boolean.Value,
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
