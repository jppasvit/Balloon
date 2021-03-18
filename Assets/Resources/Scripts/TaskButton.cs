using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskButton : MonoBehaviour
{
    public enum TaskButtonType
    {
        Host = 0,
        Resolve = 1,
        Clear
    }

    [SerializeField]
    private TaskButtonType taskButtonType;

    public TaskButtonType GetTaskButtonType()
    {
        return taskButtonType;
    }

}
