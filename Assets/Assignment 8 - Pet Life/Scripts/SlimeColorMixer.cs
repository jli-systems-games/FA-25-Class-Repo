using System.Collections.Generic;
using UnityEngine;

public class SlimeColorMixer : MonoBehaviour
{
    private List<Color> consumedColors = new List<Color>();

    private MeshRenderer slimeRenderer;

    void Start()
    {
        slimeRenderer = GetComponent<MeshRenderer>();
        slimeRenderer.material.color = Color.clear;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Orb"))
        {
            MeshRenderer orbRenderer = other.GetComponent<MeshRenderer>();

            Color orbColor = orbRenderer.material.color;

            MixColor(orbColor);
        }
    }

    void MixColor(Color newOrbColor)
    {

    }
}
