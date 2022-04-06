using UnityEngine;

[CreateAssetMenu(fileName = "DolphinParameters", menuName = "Dolphin Parameters")]
public class DolphinParameters : ScriptableObject
{
    public GameObject dolphinPreafab;
    public float swimSpeed;
    public float rollSpeed;
    public float waterExitForce, waterEntryForce;
    public float density;
    public float mass;
    public float drag;
    public float angularDrag;
    public float tailSlideDecceleration;
    public float tailSlideLaunchForce;
}