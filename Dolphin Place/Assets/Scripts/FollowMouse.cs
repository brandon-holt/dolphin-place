using UnityEngine;
using UnityEngine.InputSystem;
public class FollowMouse : MonoBehaviour
{
    public Canvas canvas;
    private RectTransform rt;
    private Vector2 refVel2; private Vector3 refVel3;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void SnapToMouse()
    {
        if (canvas == null) return;

        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
        Mouse.current.position.ReadValue(), canvas.worldCamera, out pos);

        transform.position = canvas.transform.TransformPoint(pos);
    }

    void Update()
    {
        if (canvas == null) return;

        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
        Mouse.current.position.ReadValue(), canvas.worldCamera, out pos);

        Vector2 viewPortPos = canvas.worldCamera.ScreenToViewportPoint(Mouse.current.position.ReadValue());

        Vector2 pivots = .1f * Vector2.one;

        pivots.x += .8f * Mathf.Floor(viewPortPos.x / .51f);

        pivots.y += .8f * Mathf.Floor(viewPortPos.y / .51f);

        rt.pivot = Vector2.SmoothDamp(rt.pivot, pivots, ref refVel2, .2f);

        Vector3 newPosition = canvas.transform.TransformPoint(pos);

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref refVel3, .2f);
    }

}
