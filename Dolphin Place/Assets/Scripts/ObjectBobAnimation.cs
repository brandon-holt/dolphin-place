using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBobAnimation : MonoBehaviour
{
    public bool playOnAwake;
    public float bobSpeed, heightRange;

    private void Awake()
    {
        if (playOnAwake) Play();
    }

    public void Play(Vector3 direction = default(Vector3))
    {
        if (direction == default(Vector3)) direction = Vector3.up;

        StartCoroutine(Bob(direction));
    }

    private IEnumerator Bob(Vector3 direction)
    {
        float offset = Random.Range(0f, 1f);

        Vector3 originalPosition = transform.position;

        direction = transform.rotation * direction.normalized;

        while (true)
        {
            transform.position = originalPosition + direction * heightRange * Mathf.Sin((Time.time - offset) * bobSpeed);

            yield return null;
        }
    }
}
