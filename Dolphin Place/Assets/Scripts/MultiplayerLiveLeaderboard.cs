using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MultiplayerLiveLeaderboard : MonoBehaviour
{
    public LocalParameters lp;
    public Table table;
    public Transform dolphinsParent;
    public float updateEverySeconds;

    public void SetLiveLeaderboardState()
    {
        bool multiplayer = lp.gameMode == LocalParameters.GameModes.Multiplayer;

        table.gameObject.SetActive(multiplayer);

        if (multiplayer) InvokeRepeating(nameof(UpdateTable), 0f, updateEverySeconds);
        else CancelInvoke();
    }

    private void UpdateTable()
    {
        Dolphin[] dolphins = dolphinsParent.GetComponentsInChildren<Dolphin>();

        Dictionary<string, int> dolphinScores = new Dictionary<string, int>();

        foreach (Dolphin d in dolphins)
        {
            if (d.menuDolphin) continue;

            string key = d.dolphinName;

            while (dolphinScores.ContainsKey(key)) key += Random.Range(0, 10);

            dolphinScores.Add(key, d.GetScores(out _, out _, out _, out _, out _));
        }

        dolphinScores = dolphinScores.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        string tableData = "rank/dolphin/score" + '\n';

        string[] names = dolphinScores.Keys.ToArray();

        int[] scores = dolphinScores.Values.ToArray();

        for (int i = names.Length - 1; i > -1; i--)
        {
            tableData += (names.Length - i).ToString() + "/" + names[i] + "/" + scores[i];

            if (i > 0) tableData += "\n";
        }

        table.SetTableInfo("Live Scores", tableData, null, 20f, 30f);
    }
}
