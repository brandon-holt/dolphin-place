using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class TrickCamera : MonoBehaviour
{
    public LocalParameters lp;
    public TextMeshProUGUI nameText;
    private Transform currentDolphin;
    private Vector3 refVel;

    public void StartFollowingLocalDolphin()
    {
        currentDolphin = lp.localDolphin.transform;

        nameText.text = lp.localDolphin.dolphinName;
    }

    public void SwitchDolphins()
    {
        if (SpawnDolphins.instance == null) return;

        List<Dolphin> dolphins = SpawnDolphins.instance.GetComponentsInChildren<Dolphin>().ToList();

        if (dolphins.Contains(currentDolphin.GetComponent<Dolphin>()))
        {
            int index = dolphins.IndexOf(currentDolphin.GetComponent<Dolphin>());

            if (index == dolphins.Count - 1) index = 0;
            else index++;

            currentDolphin = dolphins[index].transform;

            nameText.text = currentDolphin.GetComponent<Dolphin>().dolphinName;
        }
    }

    private void FixedUpdate()
    {
        if (currentDolphin == null) return;

        Vector3 targetPosition = currentDolphin.position + lp.trickCameraOffset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref refVel, .1f);

        transform.LookAt(currentDolphin, Vector3.up);
    }
}
