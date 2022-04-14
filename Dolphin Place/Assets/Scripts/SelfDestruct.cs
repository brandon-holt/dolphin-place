using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float destroyTime;

    void Start()
    {
        Invoke(nameof(SelfDestroy), destroyTime);
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
