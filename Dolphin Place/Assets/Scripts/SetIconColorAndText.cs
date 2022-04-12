using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SetIconColorAndText : MonoBehaviour
{
    public LocalParameters lp;
    public Gradient gradient;
    public float min, max;
    public string textFormat;
    public enum Variable { Null, LocalDolphinSpeed, SwimMultiplier }
    public Variable variable;
    public float variableUpdateInterval;
    private Image image;
    private TextMeshProUGUI text;

    private void OnEnable()
    {
        image = GetComponent<Image>();

        text = GetComponentInChildren<TextMeshProUGUI>();

        if (variable != Variable.Null) StartCoroutine(UpdateVariable());
    }

    public void SetColor(float value)
    {
        float normalizedValue = (value - min) / (max - min);

        image.color = gradient.Evaluate(normalizedValue);

        if (variable == Variable.SwimMultiplier) text.text = Mathf.RoundToInt(value).ToString() + "x";
        else text.text = value.ToString(textFormat);
    }

    private IEnumerator UpdateVariable()
    {
        while (true)
        {
            switch (variable)
            {
                case Variable.LocalDolphinSpeed:
                    if (lp.localDolphin != null) SetColor(lp.localDolphin.rb.velocity.magnitude);
                    break;
                default:
                    break;

            }

            yield return new WaitForSeconds(variableUpdateInterval);
        }
    }
}
