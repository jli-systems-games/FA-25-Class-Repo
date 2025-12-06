using UnityEngine;

public class MixButtonControl : MonoBehaviour
{
    
    public LabInventorySelector labSelector;

    void Start()
    {
        if (labSelector != null && labSelector.mixButton != null)
        {
            labSelector.mixButton.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (labSelector != null && labSelector.mixButton != null)
        {
            labSelector.mixButton.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (labSelector != null && labSelector.mixButton != null)
        {
            labSelector.mixButton.gameObject.SetActive(false);
        }

        if (labSelector != null)
        {
            labSelector.ClearSelection();
        }
    }
}