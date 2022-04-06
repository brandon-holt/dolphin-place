using UnityEngine;
using System.Collections.Generic;

public class RingManager : MonoBehaviour
{
    public LocalParameters lp;
    public GameObject ringPrefab;
    public int maxRings;
    public List<Ring> rings = new List<Ring>();

    public void GameModeChanged()
    {
        if (lp.gameMode == LocalParameters.GameModes.Menu)
        {
            foreach (Ring r in rings) Destroy(r.gameObject);

            rings.Clear();
        }
        else
        {
            //Invoke(nameof(SpawnRings), 1f);

            SpawnRings();
        }
    }

    public void SpawnRings()
    {
        if (lp.localDolphin == null || lp.localDolphin.planet == null) return;

        int ringsToAdd = maxRings - rings.Count;

        for (int i = 0; i < ringsToAdd; i++)
        {
            Ring r = Instantiate(ringPrefab, transform).GetComponent<Ring>();

            Vector3 pos = Vector3.zero;

            float radius = lp.localDolphin.planet.planetRadius;

            pos = RandomPositionOnSphere(Random.Range(radius, radius * 2),
            Random.Range(0f, 2f * Mathf.PI), Random.Range(0f, 2f * Mathf.PI));

            r.transform.position = lp.localDolphin.planet.transform.position + pos;

            r.transform.LookAt(lp.localDolphin.planet.transform.position + 2 * pos,
            transform.position - lp.localDolphin.planet.transform.position);

            r.Animate(Vector3.forward);

            rings.Add(r);
        }
    }

    private int RandomSign()
    {
        return Random.Range(0, 2) == 0 ? -1 : 1;
    }

    // calculate random position on surface of sphere of radius r with angles phi and theta
    private Vector3 RandomPositionOnSphere(float r, float phi, float theta)
    {
        Vector3 pos = new Vector3();

        pos.x = r * Mathf.Sin(phi) * Mathf.Cos(theta);
        pos.y = r * Mathf.Sin(phi) * Mathf.Sin(theta);
        pos.z = r * Mathf.Cos(phi);

        return pos;
    }

}
