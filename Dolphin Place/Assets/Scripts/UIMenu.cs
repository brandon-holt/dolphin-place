using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class UIMenu : MonoBehaviour
{
    public static UIMenu instance;
    public MultiplayerData multiplayerData;
    public LocalParameters localParameters;
    public DolphinDatabase dolphinDatabase;
    public DolphinParameters dolphinParameters;
    public GameObject menuCanvas, gameCanvas, loading, multiplayerOptions;
    public TMP_InputField playerNameInput, roomNameInput;
    public TextMeshProUGUI populationText, versionText;
    public TextMeshProUGUI lastSplitsText, lastScoreText, maxSplitsText, maxScoreText, comboText;
    public LevelBar levelBar;
    public Button multiplayerButton;
    public Table leaderboard;
    public ShowDolphinInfo dolphinInfo;
    public Volume volume;

    private void Awake()
    {
        instance = this;

        localParameters.controlModes = LocalParameters.ControlModes.Side2D;

        versionText.text = "Version " + Application.version;

        ResetMenu();
    }

    public void GameModeSwitched()
    {
        if (localParameters.gameMode == LocalParameters.GameModes.Singleplayer
        || localParameters.gameMode == LocalParameters.GameModes.Multiplayer)
            UpdateScoresAndLevel();
    }

    public void ResetMenu()
    {
        menuCanvas.SetActive(true);

        gameCanvas.SetActive(false);

        loading.SetActive(false);

        multiplayerOptions.SetActive(false);

        playerNameInput.text = "";

        roomNameInput.text = "";

        multiplayerButton.interactable = true;

        roomNameInput.interactable = true;

        playerNameInput.interactable = true;

        localParameters.SetGameMode(LocalParameters.GameModes.Menu);

        dolphinDatabase.GetNumberOfDolphins();
    }

    private void Update()
    {
        if (localParameters.gameMode == LocalParameters.GameModes.Menu)
            populationText.text = "Population: " + dolphinDatabase.numberOfDolphinsInDatabase.ToString("N0");

        if (localParameters.localDolphin != null
        && localParameters.gameMode != LocalParameters.GameModes.Menu)
            comboText.text = localParameters.localDolphin.combo.GetComboString();
    }

    public void StartSingleplayer()
    {
        if (string.IsNullOrEmpty(playerNameInput.text)) playerNameInput.text = RandomDolphinName();

        if (SpawnDolphins.instance != null) SpawnDolphins.instance.SpawnDolphinOffline(playerNameInput.text);

        multiplayerData.Disconnect();

        LoadGameCanvas();
    }

    public void SelectMultiplayer()
    {
        multiplayerButton.interactable = false;

        multiplayerData.ConnectToMaster();

        loading.SetActive(true);
    }

    public void Join()
    {
        multiplayerOptions.SetActive(false);

        playerNameInput.interactable = false;

        loading.SetActive(true);

        if (string.IsNullOrEmpty(playerNameInput.text)) playerNameInput.text = RandomDolphinName();

        if (string.IsNullOrEmpty(roomNameInput.text)) multiplayerData.JoinRandomOrCreateRoom();
        else multiplayerData.JoinOrCreateRoom(roomNameInput.text);
    }

    public static string RandomDolphinName()
    {
        return "Dolphin" + Random.Range(0, 9999).ToString();
    }

    public void LoadGameCanvas(bool transitionFinished = false)
    {
        if (!transitionFinished)
        {
            StartCoroutine(Transition(LoadGameCanvas));

            return;
        }

        menuCanvas.SetActive(false);

        gameCanvas.SetActive(true);
    }

    public void BackToMenu(bool transitionFinished = false)
    {
        if (!transitionFinished)
        {
            StartCoroutine(Transition(BackToMenu));

            return;
        }

        if (localParameters.localDolphin != null && dolphinDatabase != null)
            dolphinDatabase.UploadDolphinScore(localParameters.localDolphin);

        if (localParameters.gameMode == LocalParameters.GameModes.Singleplayer)
            Destroy(localParameters.localDolphin.gameObject);
        else if (localParameters.gameMode == LocalParameters.GameModes.Multiplayer)
            multiplayerData.Disconnect();

        ResetMenu();

        if (localParameters.menuDolphin == null) Instantiate(dolphinParameters.dolphinPreafab);
    }

    private IEnumerator Transition(System.Action<bool> callback)
    {
        float speed = 0f; float value = 0f;
#if LIMITLESS_EXISTS
        if (volume.profile.TryGet<LimitlessGlitch17>(out LimitlessGlitch17 g17))
        {
            g17.enable.Override(true);

            value = g17.strength.min;

            speed = 2 * (g17.strength.max - value);

            while (value < g17.strength.max)
            {
                value += Time.deltaTime * speed;

                g17.strength.Override(value);

                yield return null;
            }
        }
#endif

        yield return null;

        callback(true);

#if LIMITLESS_EXISTS
        if (g17 != null)
        {
            while (value > g17.strength.min)
            {
                value -= Time.deltaTime * speed;

                g17.strength.Override(value);

                yield return null;
            }

            g17.enable.Override(false);
        }
#endif
    }

    public void DownloadMacPlayer()
    {
        Application.OpenURL("https://www.dolphin.place/downloads/mac.zip");
    }

    public void DownloadWindowsPlayer()
    {
        Application.OpenURL("https://www.dolphin.place/downloads/windows.zip");
    }

    public void OpenGitHub()
    {
        Application.OpenURL("https://www.github.com/brandon-holt/dolphin-place");
    }

    public void IncrementSkin(int direction)
    {
        int currentSkin = localParameters.menuDolphin.GetSkin() + direction;

        if (currentSkin >= localParameters.menuDolphin.skins.materials.Count) currentSkin = 0;
        else if (currentSkin < 0) currentSkin = localParameters.menuDolphin.skins.materials.Count - 1;

        localParameters.menuDolphin.SetSkin(currentSkin);
    }

    public void SearchForDolphin(System.String dolphinName)
    {
        dolphinDatabase.DownloadDolphin(dolphinName, LoadToMenuDolphin);
    }

    private void LoadToMenuDolphin(string downloadData)
    {
        if (localParameters.menuDolphin == null) return;

        if (string.IsNullOrEmpty(downloadData))
        {
            dolphinInfo.info = null;

            totalScoreAtLoad = 0;

            return;
        }

        string[] data = downloadData.Split('/');

        dolphinInfo.info = data;

        totalScoreAtLoad = dolphinInfo.GetTotalScoreAtLoad();

        localParameters.menuDolphin.dolphinName = data[1];

        localParameters.menuDolphin.SetSkin(int.Parse(data[2]));

        playerNameInput.text = localParameters.menuDolphin.dolphinName;
    }

    private int totalScoreAtLoad;
    public void UpdateScoresAndLevel()
    {
        localParameters.localDolphin.GetScores(out int score, out List<int> scoreSplits,
        out int superscore, out List<int> superscoreSplits, out int totalscore);

        lastSplitsText.text = "";
        lastScoreText.text = Table.FormatNumber(score);
        for (int i = 0; i < scoreSplits.Count; i++)
        {
            lastSplitsText.text += scoreSplits[i].ToString("N0");
            if (i < scoreSplits.Count - 1) lastSplitsText.text += " / ";
        }

        maxSplitsText.text = "";
        maxScoreText.text = Table.FormatNumber(superscore);
        for (int i = 0; i < superscoreSplits.Count; i++)
        {
            maxSplitsText.text += superscoreSplits[i].ToString("N0");
            if (i < superscoreSplits.Count - 1) maxSplitsText.text += " / ";
        }

        levelBar?.UpdateLevelBar(totalScoreAtLoad + totalscore);
    }

    public void OpenLeaderboards()
    {
        dolphinDatabase.GetLeaderboardScores(DisplayLeaderboardData);
    }

    private void DisplayLeaderboardData(string data)
    {
        leaderboard.SetTableInfo("Dolphins", data, new List<int>() { 1, 3, 7, 8, 9 });

        leaderboard.gameObject.SetActive(true);
    }
}