using UnityEngine;
using Photon.Pun;

public class MultiplayerCallbacks : MonoBehaviourPunCallbacks
{
    public MultiplayerData multiplayerData;

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        if (UIMenu.instance != null)
        {
            UIMenu.instance.loading.SetActive(false);

            UIMenu.instance.joinButton.SetActive(true);

            UIMenu.instance.roomNameInput.gameObject.SetActive(true);

            UIMenu.instance.playerNameInput.interactable = true;
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (UIMenu.instance != null) UIMenu.instance.menuCanvas.SetActive(false);

        string dolphinName = UIMenu.RandomDolphinName();

        if (UIMenu.instance != null) dolphinName = UIMenu.instance.playerNameInput.text;

        if (SpawnDolphins.instance != null && UIMenu.instance != null)
        {
            SpawnDolphins.instance.SpawnDolphinOnline(dolphinName);

            UIMenu.instance.LoadGameCanvas();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        multiplayerData.Disconnect();

        if (UIMenu.instance != null) UIMenu.instance.ResetMenu();

        Debug.Log(message);
    }
}