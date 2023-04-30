# GradeSystem
A small utility package which makes the process of adding grades or ranks in games easier.

## Installation
 - Grab the .unitypackage file from the Releases tab.
 - Open the .unitypackage file with the Unity Editor open.
  - Optionally, untick the checkbox on the `Examples` folder to exclude any non-core scripts.

## Creating a grade
 - To create a grade, create or open your 'Resources' folder, and anywhere inside of the folder:
	 -  Right Click -> 'Create' -> 'Create New Grade'.
 - Give the new `ScriptableObject` any criteria you want, a name, a group, and a priority. You're all set!

## Creating your own criteria
 - To create a criteria script, create a new C# Script anywhere in your project.
 - Once you have a new script, make it inherit from `GradeCriteria` instead of `MonoBehaviour`.
 - This script should be given the `[System.Serializable]` attribute at the top of the class so it can save in the `ScriptableObject`.
 - Create your `CompareTo(GradeCriteria criteria)` override method,  which will check if the criteria to compare against is valid. Returning `true` will mean the comparison is valid.
	 - Tip: The given criteria will always be the same type as your new `GradeCriteria` class. You should probably cast the criteria value to the `GradeCriteria` class you created to make it easier to use.
 - Optionally, create a `Display(GradeData data)` override method, but surround it with a `#if UNITY_EDITOR` preprocessor flag. This method is responsible for any Inspector GUI in the `GradeData` `ScriptableObject`.

## Actually comparing the grade data
- Somewhere in the script that you're using to compare the grade data to the requirement, you'll need to create a variable which uses the same type as the criteria you want to compare against.
- Create your variables, and then initialize them in `Awake()`. Initializing them in the script will NOT work.
- Example:
```cs
[HideInInspector, System.NonSerialized]
public IntGradeCriteria ScoreCriteria;

private void Awake() {
	ScoreCriteria =  new IntGradeCriteria("Score", 0);
}
```
- To evaluate the `GradeCriteria` against the `GradeData`, run `Grade.Evaluate`, which will take the `GradeData`'s group, and any criteria you provide. **Criteria not provided will not be checked for, and skipped.**
- Example:
```cs
GradeData one = Grade.Evaluate("ExampleGroup", ScoreCriteria);
GradeData two = Grade.Evaluate("OtherGroup", Criteria1, Criteria2); // params GradeCriteria[]
GradeData more = Grade.Evaluate("AnotherGroup", MultipleCriteria); // IEnumerable<GradeCriteria>
```
- You can also get all of the `GradeData` from any group, at any time with `Grade.GetGrades(string group)`.
