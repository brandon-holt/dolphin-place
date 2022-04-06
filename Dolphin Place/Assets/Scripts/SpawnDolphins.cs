using UnityEngine;

public class SpawnDolphins : MonoBehaviour
{
    public static SpawnDolphins instance;
    public LocalParameters localParameters;
    public DolphinParameters dolphinParameters;
    public MultiplayerData multiplayerData;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnDolphinOnline(string dolphinName)
    {
        localParameters.SetGameMode(LocalParameters.GameModes.Multiplayer);

        int skin = 0;

        if (localParameters.menuDolphin != null) skin = localParameters.menuDolphin.GetSkin();

        object[] customData = new object[] { dolphinName, skin };

        multiplayerData.InstantiateMultiplayerDolphin(dolphinParameters.dolphinPreafab.name,
        transform.position, Quaternion.identity, customData);

        if (localParameters.menuDolphin != null) Destroy(localParameters.menuDolphin.gameObject);
    }

    public void SpawnDolphinOffline(string dolphinName)
    {
        localParameters.SetGameMode(LocalParameters.GameModes.Singleplayer);

        Dolphin dolphin = Instantiate(dolphinParameters.dolphinPreafab, transform).GetComponent<Dolphin>();

        dolphin.transform.localPosition = Vector3.zero;

        dolphin.dolphinName = dolphinName;

        if (localParameters.menuDolphin != null)
        {
            dolphin.SetSkin(localParameters.menuDolphin.GetSkin());

            Destroy(localParameters.menuDolphin.gameObject);
        }

    }
}