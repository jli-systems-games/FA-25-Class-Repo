using UnityEngine;
using UnityEngine.UI;

public class SetCharacterParts : MonoBehaviour
{
    public Image player1HeadDisplay;
    public Image player1BodyDisplay;
    public Image player1LegDisplay;

    public Image player2HeadDisplay;
    public Image player2BodyDisplay;
    public Image player2LegDisplay;

    void Start()
    {
        player1HeadDisplay.sprite = Data.player1Head;
        player1BodyDisplay.sprite = Data.player1Body;
        player1LegDisplay.sprite = Data.player1Leg;

        player2HeadDisplay.sprite = Data.player2Head;
        player2BodyDisplay.sprite = Data.player2Body;
        player2LegDisplay.sprite = Data.player2Leg;
    }
}
