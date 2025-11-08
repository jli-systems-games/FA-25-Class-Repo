using UnityEngine;

public class NameTarget : MonoBehaviour
{

    public string objectName;

    public bool CheckAnswer(string playerInput)
    {
        if (playerInput == null) return false;


        return string.Equals(
            objectName.Trim(),
            playerInput.Trim(),
            System.StringComparison.OrdinalIgnoreCase
        );
    }
}