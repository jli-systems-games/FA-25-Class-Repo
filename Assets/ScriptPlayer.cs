using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScriptPlayer : MonoBehaviour
{
    public List<string> dialogue = new List<string>()
    {
        "While walking, I asked him to take me home.",
        "I haven't ridden a motorcycle in a long time.",
        "I also haven't been this close to someone in a long time.",
        "Even though I know this road isn't very far,",
        "I know I will get off soon." ,
        "But in this one minute,",
        "I feel so warm." 
    };

    private int currentIndex = 0;

    public TextMeshProUGUI dialogueText;

    public void GetNextLine()
    {
        dialogueText.text = dialogue[currentIndex];
        currentIndex++;
    }
}
