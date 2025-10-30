using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    public GameObject panel;
    private bool isVisible = false;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            isVisible = !isVisible;
            if (panel != null)
                panel.SetActive(isVisible);
        }
    }
}