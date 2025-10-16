using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Image icon;
    public TMP_Text label;
    public HandType handType;
}

public enum HandType { Rock, Paper, Scissor }