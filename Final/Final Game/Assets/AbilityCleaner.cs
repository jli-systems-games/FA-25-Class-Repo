using UnityEngine;
using MoreMountains.TopDownEngine;

public class AbilityCleaner : MonoBehaviour
{
    void Start()
    {
        Destroy(GetComponent<CharacterCrouch>());
        Destroy(GetComponent<CharacterDash3D>());
        Destroy(GetComponent<CharacterDash2D>());
    }
}