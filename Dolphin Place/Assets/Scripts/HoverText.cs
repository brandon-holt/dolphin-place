using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Canvas canvas;
    private GameObject hoverText;
    private FollowMouse hoverTextFollowMouse;
    public GameObject hoverTextPrefab;
    public string[] displayText;
    private bool dontShowHoverText;

    public void SetVisibility(bool visible)
    {
        dontShowHoverText = !visible;

        if (hoverText != null && !visible) hoverText.SetActive(visible);
    }

    public void SetActive(bool active)
    {
        if (hoverText != null) hoverText.SetActive(active);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dontShowHoverText) return;

        if (hoverText == null)
        {
            hoverText = Instantiate(hoverTextPrefab, canvas.transform);

            string text = "";

            foreach (string s in displayText) text += s + "\n";

            hoverText.GetComponentInChildren<TextMeshProUGUI>().text = text;

            hoverTextFollowMouse = hoverText.GetComponent<FollowMouse>();

            hoverTextFollowMouse.canvas = canvas;

            hoverTextFollowMouse.SnapToMouse();

            hoverText.name = "Hover Text (" + gameObject.name + ")";
        }
        else
        {
            hoverTextFollowMouse.SnapToMouse();

            hoverText.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverText == null) return;

        hoverText.SetActive(false);
    }
}
