using UnityEngine;
using TMPro;

public class ShowDolphinInfo : MonoBehaviour
{
    public string[] info;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

        info = null;
    }

    public void UpdateInfo()
    {
        if (info == null)
        {
            text.text = "New Dolphin";

            return;
        }

        text.text = "Name: " + info[1] + "\n";

        text.text += "Birthday: " + info[3] + "\n";

        text.text += "Last Swim: " + info[4] + "\n";

        text.text += "Swims: " + int.Parse(info[5]).ToString("N0") + "\n";

        text.text += "Swim Time: " + info[6] + " mins" + "\n";

        text.text += "Totalscore: " + int.Parse(info[7]).ToString("N0") + "\n";

        text.text += "Superscore: " + int.Parse(info[8]).ToString("N0") + "\n";

        text.text += "Score: " + int.Parse(info[9]).ToString("N0");
    }

    public int GetTotalScoreAtLoad()
    {
        if (info == null) return 0;

        return int.Parse(info[7]);
    }
}
