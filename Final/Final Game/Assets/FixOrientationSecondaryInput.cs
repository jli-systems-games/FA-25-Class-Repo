using UnityEngine;
using System.Reflection;
using MoreMountains.TopDownEngine;

public class FixOrientationSecondaryInput : MonoBehaviour
{
    void Start()
    {
        // 找场景里所有 Character
        var characters = FindObjectsByType<Character>(FindObjectsSortMode.None);

        foreach (var character in characters)
        {
            var inputManager = character.GetComponent<InputManager>();
            var orientation2D = character.GetComponent<CharacterOrientation2D>();
            var orientation3D = character.GetComponent<CharacterOrientation3D>();

            string playerID = inputManager != null ? inputManager.PlayerID : "None";
            string name = character.name;

            if (orientation2D != null)
            {
                HandleOrientation(name, playerID, orientation2D);
            }
            if (orientation3D != null)
            {
                HandleOrientation(name, playerID, orientation3D);
            }
        }
    }

    private void HandleOrientation(string name, string playerID, object orientation)
    {
        var type = orientation.GetType();
        var useSecondaryField = type.GetField("UseSecondaryInput",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (useSecondaryField == null)
        {
            Debug.Log($"[OrientationDebug] {name} ({playerID}) 没有 UseSecondaryInput 字段。");
            return;
        }

        bool currentValue = (bool)useSecondaryField.GetValue(orientation);
        Debug.Log($"[OrientationDebug] {name} ({playerID}) UseSecondaryInput = {currentValue}");

        // ★ 根据你 Inspector 里的 PlayerID 写：例如 "Player3" / "Player4"
        if (playerID == "Player3" || playerID == "Player4")
        {
            if (currentValue)
            {
                useSecondaryField.SetValue(orientation, false);
                Debug.Log($"[OrientationFix] 已把 {name} ({playerID}) 的 UseSecondaryInput 设为 false。");
            }
        }
    }
}
