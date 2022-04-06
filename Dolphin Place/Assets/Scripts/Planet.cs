using UnityEngine;

public class Planet : MonoBehaviour
{
    public float gravityForce, waterDensity;
    public float planetRadius, waterRadius;

    private void Awake()
    {
        planetRadius = transform.lossyScale.x * GetComponent<SphereCollider>().radius;

        Transform water = transform.Find("Water");

        waterRadius = water.lossyScale.x * water.GetComponent<SphereCollider>().radius;
    }

    public void ApplyForces(Dolphin dolphin)
    {
        Vector3 vector = transform.position - dolphin.transform.position;

        Vector3 gravity = gravityForce * vector.normalized / vector.sqrMagnitude;

        Vector3 buoyant = Vector3.zero;

        if (vector.magnitude < 1.05f * waterRadius && dolphin.inWater) buoyant = -(waterDensity / dolphin.dolphinParameters.density) * gravity;

        dolphin.rb.AddForce(700000f * (gravity + buoyant));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dolphin"))
        {
            Dolphin dolphin = collision.gameObject.GetComponentInParent<Dolphin>();

            dolphin.ResetMutliplier();

            dolphin.ResetCombo();
        }
    }
}
