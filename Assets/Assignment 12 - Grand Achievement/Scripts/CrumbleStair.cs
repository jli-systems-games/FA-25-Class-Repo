using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleStair : MonoBehaviour
{
    public Transform[] stairTransforms;

    public float buildDistance = 5f;
    public Transform player;
    public string breakTimeProperty = "_Break_Time";
    public float breakTimeMax = 6f;
    public float buildSpeed = 1f;

    private float[] currentBreakTimes;

    private bool[] isActiveStep;

    void Start()
    {
        currentBreakTimes = new float[stairTransforms.Length];
        isActiveStep = new bool[stairTransforms.Length];

        for (int i = 0; i < stairTransforms.Length; i++)
        {
            stairTransforms[i].gameObject.SetActive(false);

            Renderer[] renderers = stairTransforms[i].GetComponentsInChildren<Renderer>();

            foreach (Renderer r in renderers)
            {
                r.material = new Material(r.material);
                r.material.SetFloat(breakTimeProperty, breakTimeMax);
            }

            currentBreakTimes[i] = breakTimeMax;
            isActiveStep[i] = false;
        }
    }

    void Update()
    {
        if (player == null) return;

        for (int i = 0; i < stairTransforms.Length; i++)
        {
            float distance = Vector3.Distance(player.position, stairTransforms[i].position);

            if (distance < buildDistance)
            {
                if (!isActiveStep[i])
                {
                    stairTransforms[i].gameObject.SetActive(true);
                    isActiveStep[i] = true;
                }

                currentBreakTimes[i] -= Time.deltaTime * buildSpeed;
                currentBreakTimes[i] = Mathf.Max(currentBreakTimes[i], 0f);

                Renderer[] renderers = stairTransforms[i].GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    r.material.SetFloat(breakTimeProperty, currentBreakTimes[i]);
                }
            }
        }
    }
}
