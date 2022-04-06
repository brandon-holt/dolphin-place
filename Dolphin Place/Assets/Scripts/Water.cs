using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dolphin"))
        {
            other.gameObject.GetComponentInParent<Dolphin>().WaterEntry();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Dolphin"))
        {
            other.gameObject.GetComponentInParent<Dolphin>().WaterExit();
        }
    }
}
