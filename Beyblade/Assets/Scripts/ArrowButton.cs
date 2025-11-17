using UnityEngine;

public class ArrowButton : MonoBehaviour
{
    public BladeSelector selector;
    public bool rightArrow;

    public void Press()
    {
        if (rightArrow)
            selector.Next();
        else
            selector.Previous();
    }
}
