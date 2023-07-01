using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConditionHandler : MonoBehaviour
{
    [SerializeField]
    private string[] conditionNames;
    private bool[] conditionValues;
    [SerializeField]
    private UnityEvent onAllConditionsTrue;
    private Dictionary<string, int> conditions;

    private void Start()
    {
        conditions = new Dictionary<string, int>();
        conditionValues = new bool[conditionNames.Length];
        for (int i = 0; i < conditionNames.Length; i++)
        {
            conditions.Add(conditionNames[i], i);
            conditionValues[i] = false;
        }
    }

    public void UpdateCondition(string name, bool value)
    {
        if (conditions.ContainsKey(name))
        {
            conditionValues[conditions[name]] = value;

            bool allTrue = true;
            foreach (bool i in conditionValues)
            {
                if (i == false)
                {
                    allTrue = false;
                    break;
                }
            }

            if (allTrue)
                onAllConditionsTrue?.Invoke();
        }
    }

}
