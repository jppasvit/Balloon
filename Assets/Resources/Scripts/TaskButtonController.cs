using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class TaskButtonController : MonoBehaviour
{
    [SerializeField]
    private CloudAnchorManager cloudAnchorManager;
    [SerializeField]
    private MessageArea messageArea;
    private static string message = "Please tap the place where you want the balloon to be created."; 

    private static List<GameObject> buttons = new List<GameObject>();

    void Start()
    {
        FillTaskButtonsList();
        TaskForTaskButton(TaskButton.TaskButtonType.Clear);
        TaskForTaskButton(TaskButton.TaskButtonType.Host);
        TaskForTaskButton(TaskButton.TaskButtonType.Resolve);
    }

    private GameObject SearchTaskButtonByTaskButtonType(TaskButton.TaskButtonType buttonType)
    {
        foreach (GameObject button in buttons)
        {
            TaskButton taskButton = button.GetComponent<TaskButton>();
            if (taskButton != null && taskButton.GetTaskButtonType() == buttonType)
            {
                return button;
            }
        }
        return null;
    }

    private void TaskForTaskButton(TaskButton.TaskButtonType buttonType)
    {
        GameObject taskButton = SearchTaskButtonByTaskButtonType(buttonType);
        if (taskButton != null)
        {
            var button = taskButton.transform.GetChild(0).GetComponent<Button>();
            if ( buttonType == TaskButton.TaskButtonType.Clear )
            {
                button.onClick.AddListener(Clear);
            }
            else if ( buttonType == TaskButton.TaskButtonType.Host )
            {
                button.onClick.AddListener(SetTrueEmulateHost);
            }
            else if ( buttonType == TaskButton.TaskButtonType.Resolve )
            {
                button.onClick.AddListener(SetTrueEmulateResolve);
            }
        }
    }

    private void Clear()
    {
        messageArea.InfoMessage(message);
        cloudAnchorManager.Clear();
    }

    private void SetTrueEmulateResolve() 
    {
        messageArea.InfoMessage(message);
        cloudAnchorManager.emulateResolve = true;
    }

    private void SetTrueEmulateHost()
    {
        messageArea.InfoMessage(message);
        cloudAnchorManager.emulateHost = true;
    }

    public void SetActiveTaskButton(TaskButton.TaskButtonType buttonType, bool active)
    {
        GameObject taskButton = SearchTaskButtonByTaskButtonType(buttonType);
        if (taskButton != null)
        {
            taskButton.SetActive(active);
            taskButton.transform.GetChild(0).gameObject.SetActive(active);
        }
    }

    public void SetActiveButton(TaskButton.TaskButtonType buttonType, bool active)
    {
        GameObject taskButton = SearchTaskButtonByTaskButtonType(buttonType);
        if (taskButton != null)
        {
            GameObject button = taskButton.transform.GetChild(0).gameObject;
            button.SetActive(active);
        }
    }

    private void FillTaskButtonsList()
    {
        for(int i = 0; i < transform.childCount ; i++)
        {
            var button = transform.GetChild(i);
            if (button.GetComponent<TaskButton>() != null )
            {
                buttons.Add(button.gameObject);
            }
        }
    }
}
