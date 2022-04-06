using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dolphin"))
        {
            other.gameObject.GetComponentInParent<Dolphin>().planet = this.gameObject.GetComponentInParent<Planet>();
        }
    }
}
