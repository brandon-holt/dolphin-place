using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelBar : MonoBehaviour
{
    public Image levelBarFill;
    public TextMeshProUGUI levelNumberText, levelXPText;
    public int testXP;

    public void UpdateLevelBar(int xp)
    {
        int level = GetCurrentLevel(xp);

        int xpToNextLevel = GetXpRequiredForLevel(level);

        int xpAtLastLevel = GetXpRequiredForLevel(level - 1);

        levelBarFill.fillAmount = (float)(xp - xpAtLastLevel) / (xpToNextLevel - xpAtLastLevel);

        levelNumberText.text = level.ToString();

        levelXPText.text = xp.ToString("N0") + " / " + xpToNextLevel.ToString("N0");
    }

    public float x = .1f, y = 2f;

    private int GetCurrentLevel(int xp)
    {
        return Mathf.CeilToInt(x * Mathf.Pow(xp, 1f / y));
    }

    private int GetXpRequiredForLevel(int level)
    {
        return Mathf.FloorToInt(Mathf.Pow(level / x, y));
    }
}
