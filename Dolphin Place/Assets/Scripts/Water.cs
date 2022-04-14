using UnityEngine;

public class Water : MonoBehaviour
{
    public GameObject splashPrefab;
    private SphereCollider sphereCollider;
    private MonoBehaviourDataContainer monoBehaviourDataContainer;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();

        monoBehaviourDataContainer = GetComponent<MonoBehaviourDataContainer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dolphin"))
        {
            float entryAngle = other.gameObject.GetComponentInParent<Dolphin>().WaterEntry();

            SplashEffect(other, entryAngle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Dolphin"))
        {
            other.gameObject.GetComponentInParent<Dolphin>().WaterExit();
        }
    }

    private void SplashEffect(Collider other, float entryAngle)
    {
        Vector3 splashPosition = sphereCollider.ClosestPointOnBounds(other.transform.position);

        Vector3 upward = splashPosition - transform.position;

        GameObject splash = Instantiate(splashPrefab, splashPosition, Quaternion.LookRotation(upward));

        float normalizationConstant = monoBehaviourDataContainer.localParameters.splashNormalizationConstant;

        float scale = other.attachedRigidbody.velocity.magnitude * entryAngle / normalizationConstant;

        for (int i = 0; i < 3; i++) splash.transform.GetChild(i).localScale = scale * Vector3.one;
    }
}
