using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SplitClock : MonoBehaviour
{
    public LocalParameters lp;
    public Gradient rimGradient;
    public Image rim, scoresRim;
    public TextMeshProUGUI splitText;

    private void FixedUpdate()
    {
        float endTime = lp.localDolphin.timeLastSplitStart + lp.secondsPerSplit;

        float secondsLeft = endTime - Time.time;

        splitText.text = secondsLeft.ToString("N0");

        float fractionTimeLeft = secondsLeft / lp.secondsPerSplit;

        rim.fillAmount = fractionTimeLeft;

        rim.color = rimGradient.Evaluate(fractionTimeLeft);

        scoresRim.color = rim.color;
    }
}
