using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[CreateAssetMenu(fileName = "MultiplayerData", menuName = "MultiplayerData")]
public class MultiplayerData : ScriptableObject
{
    public LocalParameters localParameters;

    public void InstantiateMultiplayerDolphin(string prefabName, Vector3 position, Quaternion rotation, object[] customData)
    {
        PhotonNetwork.Instantiate(prefabName, position, rotation, 0, customData);
    }

    public void InitializeMultiplayerDolphin(Dolphin dolphin)
    {
        PhotonView photonView = dolphin.GetComponent<PhotonView>();

        if (photonView != null && photonView.IsMine) localParameters.localDolphin = dolphin;
        else dolphin.rb.isKinematic = true;

        dolphin.dolphinName = (string)photonView.InstantiationData[0];

        dolphin.SetSkin((int)photonView.InstantiationData[1]);
    }

    public void ConnectToMaster()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinRandomOrCreateRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public void JoinOrCreateRoom(string roomName)
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 8 }, TypedLobby.Default);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
}