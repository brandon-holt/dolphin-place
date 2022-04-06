using UnityEngine;
using System.Collections.Generic;

public class Ring : MonoBehaviour
{
    public LocalParameters lp;
    public ParticleSystem p;
    private RingManager ringManager;

    private void Awake()
    {
        ringManager = GetComponentInParent<RingManager>();
    }

    public void Animate(Vector3 direction = default(Vector3))
    {
        GetComponent<ObjectBobAnimation>().Play(direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dolphin"))
        {
            Dolphin d = other.GetComponentInParent<Dolphin>();

            d.rb.AddForce(d.rb.velocity.normalized * lp.ringForce, ForceMode.Impulse);

            GetComponent<Renderer>().enabled = false;

            p.Play();

            Invoke(nameof(DestroySelf), 3f);
        }
    }

    private void DestroySelf()
    {
        if (ringManager != null && ringManager.rings.Contains(this))
        {
            ringManager.rings.Remove(this);

            ringManager.SpawnRings();
        }

        Destroy(gameObject);
    }
}