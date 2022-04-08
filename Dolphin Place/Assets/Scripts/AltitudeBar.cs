using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AltitudeBar : MonoBehaviour
{
    public LocalParameters lp;
    public Transform planetIcon, dolphinIcon;
    public RectTransform bar;
    private Planet currentPlanet;
    private float currentCalibrationDistance;

    private void Update()
    {
        if (lp.localDolphin == null) return;

        if (currentPlanet == null || currentPlanet != lp.localDolphin.planet) GetCalibrationPlanet();

        SetHeight();
    }

    public void SetHeight()
    {
        float distanceFromPlanet = (lp.localDolphin.transform.position
        - currentPlanet.transform.position).magnitude;

        float height = bar.rect.height * distanceFromPlanet / currentCalibrationDistance;

        Vector3 position = planetIcon.localPosition;

        position.y += height;

        dolphinIcon.localPosition = position;
    }

    private void GetCalibrationPlanet()
    {
        currentPlanet = lp.localDolphin.planet;

        List<Planet> planets = currentPlanet.transform.parent.GetComponentsInChildren<Planet>().ToList();

        float minDistance = float.MaxValue;

        planets.Remove(currentPlanet);

        foreach (Planet p in planets)
        {
            float distance = (currentPlanet.transform.position - p.transform.position).magnitude;

            if (distance < minDistance) minDistance = distance;
        }

        currentCalibrationDistance = minDistance;
    }
}
