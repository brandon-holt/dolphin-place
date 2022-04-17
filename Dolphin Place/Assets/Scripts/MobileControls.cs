using UnityEngine;
using UnityEngine.EventSystems;

public class MobileControls : MonoBehaviour
{
    public LocalParameters lp;
    public EventTrigger turnleft, turnright, swim, frontflip, backflip, twist;
    public RectTransform[] rectTransformsAdjustForSafeArea;
    private bool swimming;

#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR_OSX

    private void Start()
    {
        AddEntry(turnleft, EventTriggerType.PointerDown, (a) => { Turnleft(true); });
        AddEntry(turnleft, EventTriggerType.PointerUp, (a) => { Turnleft(false); });

        AddEntry(turnright, EventTriggerType.PointerDown, (a) => { Turnright(true); });
        AddEntry(turnright, EventTriggerType.PointerUp, (a) => { Turnright(false); });

        AddEntry(swim, EventTriggerType.PointerDown, (a) => { Swim(); });

        AddEntry(frontflip, EventTriggerType.PointerDown, (a) => { Frontflip(true); });
        AddEntry(frontflip, EventTriggerType.PointerUp, (a) => { Frontflip(false); });

        AddEntry(backflip, EventTriggerType.PointerDown, (a) => { Backflip(true); });
        AddEntry(backflip, EventTriggerType.PointerUp, (a) => { Backflip(false); });

        AddEntry(twist, EventTriggerType.PointerDown, (a) => { Twist(true); });
        AddEntry(twist, EventTriggerType.PointerUp, (a) => { Twist(false); });
    }

    private void AddEntry(EventTrigger trigger, EventTriggerType eventTriggerType,
    UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = eventTriggerType;

        entry.callback.AddListener(action);

        trigger.triggers.Add(entry);
    }

    private void Update()
    {
        if (lp.localDolphin != null) lp.localDolphin.SetInputs(input2D, input3D, twistInput);

        foreach (RectTransform rt in rectTransformsAdjustForSafeArea) AdjustForSafeArea(rt);

        input2D.y = swimming ? 1 : 0;
    }

    private Vector2 input2D, input3D;
    private float twistInput;

    private void Turnleft(bool active)
    {
        input3D.x = active ? -1 : 0;
    }

    private void Turnright(bool active)
    {
        input3D.x = active ? 1 : 0;
    }

    private void Swim()
    {
        swimming = !swimming;
    }

    private void Frontflip(bool active)
    {
        input2D.x = active ? 1 : 0;
    }

    private void Backflip(bool active)
    {
        input2D.x = active ? -1 : 0;
    }

    private void Twist(bool active)
    {
        twistInput = active ? 1 : 0;
    }

    private void AdjustForSafeArea(RectTransform rt)
    {
        float adjustSize = (Screen.width - Screen.safeArea.width) / 2;

        if (Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            rt.offsetMin = new Vector2(adjustSize, rt.offsetMin.y);

            rt.offsetMax = new Vector2(0, rt.offsetMax.y);
        }
        else if (Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            rt.offsetMin = new Vector2(0, rt.offsetMin.y);

            rt.offsetMax = new Vector2(-adjustSize, rt.offsetMax.y);
        }
    }

#endif
}