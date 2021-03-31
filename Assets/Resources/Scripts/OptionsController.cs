using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsController : MonoBehaviour
{
    [SerializeField]
    private GameObject optionsPanel;

    [SerializeField]
    private CloudAnchorManager cloudAnchorManager;

    public void TurnOnOffOptionsPanel()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BalloonManualPositioning()
    {
        cloudAnchorManager.emulateHost = true;
    }

    public void RestartGame()
    {

    }

}
