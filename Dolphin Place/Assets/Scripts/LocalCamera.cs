using UnityEngine;

public class LocalCamera : MonoBehaviour
{
    public LocalParameters localParameters;
    private Vector3 behindVector, relativeTo, refVel;
    private float heighOffset, refFloat;

    private void FixedUpdate()
    {
        if (localParameters.menuDolphin != null)
        {
            transform.position = localParameters.menuDolphinCamPosition;

            transform.rotation = Quaternion.identity;
        }
        else
        {
            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition()
    {
        Vector3 newPosition = Vector3.zero; Quaternion newRotation = Quaternion.identity;

        if (localParameters.localDolphin == null || localParameters.localDolphin.planet == null) return;

        if (localParameters.controlModes == LocalParameters.ControlModes.Side2D) Side2D(out newPosition, out newRotation);
        else if (localParameters.controlModes == LocalParameters.ControlModes.Behind3D) Behind3D(out newPosition, out newRotation);

        transform.position = Vector3.SmoothDamp(transform.position, newPosition,
        ref refVel, localParameters.cameraPositionSmoothTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation,
        Mathf.SmoothDamp(0f, 1f, ref refFloat, localParameters.cameraAngleSmoothTime));
    }

    private void Side2D(out Vector3 newPosition, out Quaternion newRotation)
    {
        Vector3 forward = localParameters.localDolphin.transform.position - transform.position;

        Vector3 up = (localParameters.localDolphin.transform.position - localParameters.localDolphin.planet.transform.position).normalized;

        Vector3 side = localParameters.localDolphin.transform.rotation * localParameters.localDolphin.sideVector;

        newPosition = localParameters.localDolphin.transform.position + localParameters.cameraOffset.x * side
        + localParameters.cameraOffset.y * up;

        float distanceToPlanet = (newPosition - localParameters.localDolphin.planet.transform.position).magnitude;

        if (distanceToPlanet < localParameters.localDolphin.planet.waterRadius)
        {
            newPosition = 1.05f * localParameters.localDolphin.planet.waterRadius * (newPosition - localParameters.localDolphin.planet.transform.position).normalized
            + localParameters.localDolphin.planet.transform.position;
        }

        newRotation = Quaternion.LookRotation(forward, up);
    }

    private void Behind3D(out Vector3 newPosition, out Quaternion newRotation)
    {
        Vector3 forward = localParameters.localDolphin.transform.position - transform.position;

        Vector3 up = (localParameters.localDolphin.transform.position - localParameters.localDolphin.planet.transform.position).normalized;

        Vector3 behind = Vector3.Cross(up, localParameters.localDolphin.transform.rotation * localParameters.localDolphin.sideVector).normalized;

        newPosition = localParameters.localDolphin.transform.position + localParameters.cameraOffset.x * behind
        + localParameters.cameraOffset.y * up;

        float distanceToPlanet = (newPosition - localParameters.localDolphin.planet.transform.position).magnitude;

        if (distanceToPlanet < localParameters.localDolphin.planet.waterRadius)
        {
            newPosition = 1.05f * localParameters.localDolphin.planet.waterRadius * (newPosition - localParameters.localDolphin.planet.transform.position).normalized
            + localParameters.localDolphin.planet.transform.position;
        }

        newPosition += localParameters.cameraOffset.y * (newPosition - localParameters.localDolphin.planet.transform.position).normalized;

        newRotation = Quaternion.LookRotation(forward, up);
    }
}