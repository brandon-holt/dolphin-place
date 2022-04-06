using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;

public class MultiplayerSyncDolphin : MonoBehaviour
{
    private Dolphin dolphin;
    private PhotonView photonView;

    private void Awake()
    {
        dolphin = GetComponent<Dolphin>();

        photonView = GetComponent<PhotonView>();

        if (dolphin.localParameters.gameMode == LocalParameters.GameModes.Multiplayer)
            InvokeRepeating(nameof(SyncAll), 1f, 1f);
    }

    private void SyncAll()
    {
        if (dolphin.localParameters.localDolphin != dolphin) CancelInvoke();

        if (dolphin.splits.Count == 0) return;

        photonView.RPC(nameof(SyncSplits), RpcTarget.Others, dolphin.splits.ToArray());
    }

    [PunRPC]
    public void SyncSplits(int[] splits)
    {
        if (splits.Length == 0) return;

        dolphin.splits = splits.ToList();
    }
}
