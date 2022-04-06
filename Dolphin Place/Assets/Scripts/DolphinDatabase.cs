using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;

[CreateAssetMenu(fileName = "DolphinDatabase", menuName = "Dolphin Database")]
public class DolphinDatabase : ScriptableObject
{
    public TextAsset databaseInfo;
    public int numberOfDolphinsToDownload, numberOfDolphinsInDatabase;
    public int uploadScoreEveryMinutes;


    public void DownloadDolphin(string dolphinName, Action<string> callback)
    {
        string uploadType = "dolphin";

        string URL = "type=" + uploadType + "&name=" + dolphinName;

        UIMenu.instance.StartCoroutine(CallDatabase(URL, uploadType + dolphinName, callback));
    }

    public void UploadDolphin(Dolphin dolphin, int visits = 0)
    {
        PlayerPrefs.SetString("dolphin", dolphin.dolphinName);

        string uploadType = "update";
        string dolphinName = dolphin.dolphinName;
        int minutes = dolphin.GetMinutes();
        int skin = dolphin.GetSkin();

        string URL = "name=" + dolphinName + "&type=" + uploadType + "&visits=" + visits
            + "&minutes=" + minutes + "&skin=" + skin;

        UIMenu.instance.StartCoroutine(CallDatabase(URL, uploadType + dolphinName));
    }

    public void UploadDolphinScore(Dolphin dolphin)
    {
        string uploadType = "score";
        string dolphinName = dolphin.dolphinName;
        int minutes = dolphin.GetMinutes();
        dolphin.GetScores(out int score, out _, out int superscore, out _, out int totalscore);

        string URL = "name="
            + dolphinName + "&type=" + uploadType + "&minutes=" + minutes + "&score=" + score
            + "&superscore=" + superscore + "&totalscore=" + totalscore;

        UIMenu.instance.StartCoroutine(CallDatabase(URL, uploadType + dolphinName));
    }

    public void GetLeaderboardScores(Action<string> callback)
    {
        string uploadType = "alldata";
        int n = numberOfDolphinsToDownload;

        string URL = "type=" + uploadType + "&n=" + n;

        UIMenu.instance.StartCoroutine(CallDatabase(URL, uploadType, callback));
    }

    public void GetNumberOfDolphins()
    {
        string uploadType = "population";

        string URL = "type=" + uploadType;

        UIMenu.instance.StartCoroutine(CallDatabase(URL, uploadType, UpdateNumberOfDolphins));
    }

    private void UpdateNumberOfDolphins(string downloadText)
    {
        int.TryParse(downloadText, out numberOfDolphinsInDatabase);
    }

    private IEnumerator CallDatabase(string URL, string info, Action<string> callback = null)
    {
        if (databaseInfo == null) yield break;

        URL = databaseInfo.text.Split('\n')[0] + URL;

        URL = AddHash(URL, info);

        UnityWebRequest w = UnityWebRequest.Get(URL);
        w.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store"); w.SetRequestHeader("Pragma", "no-cache");
        yield return w.SendWebRequest();

        if (w.result == UnityWebRequest.Result.ConnectionError) { Debug.Log("Connection error."); }
        else { Debug.Log("Connection successful."); }

        // might want to add something here to not send the callback if the request failed or the data is not correct

        callback?.Invoke(w.downloadHandler.text);
    }

    private string AddHash(string URL, string info)
    {
        string key = databaseInfo.text.Split('\n')[1];

        string hash = ComputeHash(info + key);

        URL += "&hash=" + hash;

        return URL;
    }

    static string ComputeHash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++) builder.AppendFormat(bytes[i].ToString("x2"));

            return builder.ToString();
        }
    }
}