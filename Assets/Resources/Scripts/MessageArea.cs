using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageArea : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI messageArea;
    public static MessageArea instance { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InfoMessage(string message)
    {
        ClearFormat();
        messageArea.SetText(message);
    }

    public void SuccessMessage(string message)
    {
        SuccessFormat();
        messageArea.SetText(message);
    }

    public void WarningMessage(string message)
    {
        WarningFormat();
        messageArea.SetText(message);
    }

    public void ErrorMessage(string message)
    {
        ErrorFormat();
        messageArea.SetText(message);
    }

    public void InstructionMessage(string message)
    {
        InstructionFormat();
        messageArea.SetText(message);
    }

    public void Clear()
    {
        messageArea.SetText("");
    }

    private void ClearFormat()
    {
        messageArea.color = Color.white;
        messageArea.fontSize = 40;
        messageArea.fontStyle = FontStyles.Normal;
    }

    private void WarningFormat()
    {
        messageArea.color = Color.yellow;
        messageArea.fontSize = 35;
        messageArea.fontStyle = FontStyles.Normal;
    }

    private void ErrorFormat()
    {
        messageArea.color = Color.red;
        messageArea.fontSize = 35;
        messageArea.fontStyle = FontStyles.Normal;
    }

    private void InstructionFormat()
    {
        messageArea.color = Color.white;
        messageArea.fontSize = 40;
        messageArea.fontStyle = FontStyles.Underline;
    }

    private void SuccessFormat()
    {
        messageArea.color = Color.green;
        messageArea.fontSize = 35;
        messageArea.fontStyle = FontStyles.Normal;
    }
}
