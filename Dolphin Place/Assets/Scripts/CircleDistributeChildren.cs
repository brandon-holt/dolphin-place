using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CircleDistributeChildren : MonoBehaviour
{
    public float startAngle, endAngle;

#if UNITY_EDITOR
    private void OnValidate()
    {
        DistributeChildren();
    }
#endif

    private void DistributeChildren()
    {
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++) children.Add(transform.GetChild(i));

        float radius = GetComponent<RectTransform>().rect.width / 2;

        for (int i = 0; i < children.Count; i++)
        {
            float angle = startAngle + (endAngle - startAngle) * i / Mathf.Max(children.Count - 1, 1);

            angle *= Mathf.Deg2Rad;

            children[i].localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        }
    }
}
