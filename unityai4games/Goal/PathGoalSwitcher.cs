using GoalNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGoalSwitcher : MonoBehaviour {

    private PathGoal goal_script;
    private int goal_index = 0;
    public List<Node> ordered_goals;
    // Use this for initialization

    void Start()
    {
        goal_script = GetComponent<PathGoal>();
        goal_script.setGoal(ordered_goals[goal_index]);
    }

    public void SetNextGoal()
    {
        if (goal_index >= ordered_goals.Count - 1)
        {
            goal_index = 0;
        }
        else
        {
            goal_index += 1;
        }
        goal_script.setGoal(ordered_goals[goal_index]);
        Debug.Log(ordered_goals[goal_index].ToString());
    }
}
