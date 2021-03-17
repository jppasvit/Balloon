using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageArea : MonoBehaviour
{
  
    public GameObject gameObjectMessageArea;
    private TextMeshProUGUI messageArea;

    void Awake()
    {
        messageArea = gameObjectMessageArea.GetComponent<TextMeshProUGUI>();
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

    public void ClearFormat()
    {
        messageArea.color = Color.white;
        messageArea.fontSize = 40;
        messageArea.fontStyle = FontStyles.Normal;
    }

    public void WarningFormat()
    {
        messageArea.color = Color.yellow;
        messageArea.fontSize = 35;
        messageArea.fontStyle = FontStyles.Normal;
    }

    public void ErrorFormat()
    {
        messageArea.color = Color.red;
        messageArea.fontSize = 35;
        messageArea.fontStyle = FontStyles.Normal;
    }

    public void InstructionFormat()
    {
        messageArea.color = Color.white;
        messageArea.fontSize = 40;
        messageArea.fontStyle = FontStyles.Underline;
    }

    public void SuccessFormat()
    {
        messageArea.color = Color.green;
        messageArea.fontSize = 35;
        messageArea.fontStyle = FontStyles.Normal;
    }
}
