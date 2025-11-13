using UnityEngine;

public class AvatarSelection : MonoBehaviour
{
    public int avatarID;
    public AvatarSelectionManager manager;

    void OnMouseDown()
    {
        if (manager != null)
            manager.OnAvatarClicked(this);
    }

    public void Highlight()
    {
        transform.localScale = Vector3.one * 1.1f;
    }

    public void UnHighlight()
    {
        transform.localScale = Vector3.one;
    }
}
