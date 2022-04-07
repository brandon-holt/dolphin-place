using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Window : MonoBehaviour
{
    public Color color;
    public string title;
    public Sprite sprite;

#if UNITY_EDITOR
    private void OnValidate()
    {
        transform.Find("Title Text").GetComponent<TextMeshProUGUI>().text = title;

        GetComponent<Image>().color = color;

        transform.Find("Icon").GetComponent<Image>().sprite = sprite;
    }
#endif
}
