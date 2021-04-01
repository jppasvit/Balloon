using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [SerializeField]
    private GameObject optionsPanel;

    [SerializeField]
    private CloudAnchorManager cloudAnchorManager;

    [SerializeField]
    private Button verticalForwardTapButton;

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
        cloudAnchorManager.BalloonManualPositioning();
    }

    public void RestartGame()
    {
        GameController.instance.gameObject.GetPhotonView().RPC("RPC_Restart", RpcTarget.All);
    }
    public void VerticalForwardTap()
    {
        gameObject.GetPhotonView().RPC("RPC_VerticalForwardTap", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_VerticalForwardTap()
    {
        Text buttonText = verticalForwardTapButton.GetComponentInChildren<Text>();
        if (GameController.instance.tapBalloonForceVertical)
        {
            buttonText.text = "Enable vertical force to tap";
        }
        else
        {
            buttonText.text = "Enable vertical and forward force to tap";
        }
        GameController.instance.tapBalloonForceVertical = !GameController.instance.tapBalloonForceVertical;
    }

    public void GoodOrSufficientFeatureMapQuality(Button button)
    {
        Text buttonText = button.GetComponentInChildren<Text>();
        if (cloudAnchorManager.onlyGoodOrSufficientFeatureMapQuality)
        {
            buttonText.text = "Hosting with sufficient FeatureMapQuality";
        }
        else
        {
            buttonText.text = "Hosting without sufficient FeatureMapQuality";
        }
        cloudAnchorManager.onlyGoodOrSufficientFeatureMapQuality = !cloudAnchorManager.onlyGoodOrSufficientFeatureMapQuality;
    }

    public void EnableDisablePlaneClassification(Button button)
    {
        Text buttonText = button.GetComponentInChildren<Text>();
        if (GameController.instance.planeClassificationEnabled)
        {
            buttonText.text = "The game is over when balloon touch floor plane";
        }
        else
        {
            buttonText.text = "The game is over when balloon touch any plane";
        }
        GameController.instance.planeClassificationEnabled = !GameController.instance.planeClassificationEnabled;
    }

}
