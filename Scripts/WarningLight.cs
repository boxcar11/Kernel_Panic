using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLight : MonoBehaviour
{

    public List<GameObject> lights = new List<GameObject>();
    public bool hazard = false;

    public void StartLights()
    {
        StartCoroutine(CycleLights());
    }

    IEnumerator CycleLights()
    {
        while (hazard == false)
        {
            for (int i = 0; i < lights.Count - 1; i++)
            {
                lights[i].SetActive(true);
                yield return new WaitForSeconds(.1f);
                lights[i].SetActive(false);
            }
        }
        yield return null;
    }
}
