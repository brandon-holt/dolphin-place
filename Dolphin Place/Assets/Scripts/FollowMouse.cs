using UnityEngine;
using UnityEngine.InputSystem;
public class FollowMouse : MonoBehaviour
{
    public Canvas canvas;

    void Update()
    {
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
        Mouse.current.position.ReadValue(), canvas.worldCamera, out pos);

        transform.position = canvas.transform.TransformPoint(pos);
    }

}
