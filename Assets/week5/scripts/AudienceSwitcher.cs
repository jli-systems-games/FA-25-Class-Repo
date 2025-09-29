using UnityEngine;

public class AudienceSwitcher : MonoBehaviour
{
    public GameObject audience1Root; // 관중 화면 A
    public GameObject audience2Root; // 관중 화면 B

    bool showingA1 = true;

    void Start() { Apply(); }

    public void ToggleAudience()
    {
        showingA1 = !showingA1;
        Apply();
    }

    void Apply()
    {
        if (audience1Root) audience1Root.SetActive(showingA1);
        if (audience2Root) audience2Root.SetActive(!showingA1);
    }
}
