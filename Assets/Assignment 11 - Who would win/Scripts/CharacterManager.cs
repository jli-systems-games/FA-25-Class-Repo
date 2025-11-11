using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    [Header("Player 1")]
    public Image player1HeadDisplay;
    public Image player1BodyDisplay;
    public Image player1LegDisplay;
    private int player1HeadIndex = 0;
    private int player1BodyIndex = 0;
    private int player1LegIndex = 0;

    [Header("Player 2")]
    public Image player2HeadDisplay;
    public Image player2BodyDisplay;
    public Image player2LegDisplay;
    private int player2HeadIndex = 0;
    private int player2BodyIndex = 0;
    private int player2LegIndex = 0;

    [Header("Scriptable Parts")]
    public List<PartBase> headParts;
    public List<PartBase> bodyParts;
    public List<PartBase> legParts;

    void Start()
    {
        player1HeadDisplay.sprite = headParts[player1HeadIndex].image;
        player1BodyDisplay.sprite = bodyParts[player1BodyIndex].image;
        player1LegDisplay.sprite = legParts[player1LegIndex].image;

        player2HeadDisplay.sprite = headParts[player2HeadIndex].image;
        player2BodyDisplay.sprite = bodyParts[player2BodyIndex].image;
        player2LegDisplay.sprite = legParts[player2LegIndex].image;
    }

    #region Player Change Parts
    public void Player1HeadClickRight()
    {
        RightClick(ref player1HeadIndex, headParts, player1HeadDisplay);
    }

    public void Player1HeadClickLeft()
    {
        LeftClick(ref player1HeadIndex, headParts, player1HeadDisplay);
    }
    public void Player1BodyClickRight()
    {
        RightClick(ref player1BodyIndex, bodyParts, player1BodyDisplay);
    }

    public void Player1BodyClickLeft()
    {
        LeftClick(ref player1BodyIndex, bodyParts, player1BodyDisplay);
    }
    public void Player1LegClickRight()
    {
        RightClick(ref player1LegIndex, legParts, player1LegDisplay);
    }

    public void Player1LegClickLeft()
    {
        LeftClick(ref player1LegIndex, legParts, player1LegDisplay);
    }
    public void Player2HeadClickRight()
    {
        RightClick(ref player2HeadIndex, headParts, player2HeadDisplay);
    }

    public void Player2HeadClickLeft()
    {
        LeftClick(ref player2HeadIndex, headParts, player2HeadDisplay);
    }
    public void Player2BodyClickRight()
    {
        RightClick(ref player2BodyIndex, bodyParts, player2BodyDisplay);
    }

    public void Player2BodyClickLeft()
    {
        LeftClick(ref player2BodyIndex, bodyParts, player2BodyDisplay);
    }
    public void Player2LegClickRight()
    {
        RightClick(ref player2LegIndex, legParts, player2LegDisplay);
    }

    public void Player2LegClickLeft()
    {
        LeftClick(ref player2LegIndex, legParts, player2LegDisplay);
    }

    public void RightClick(ref int playerIndex, List<PartBase> parts, Image displayImage)
    {
        if (playerIndex < parts.Count - 1)
        {
            playerIndex++;
        }
        else
        {
            playerIndex = 0;
        }

        displayImage.sprite = parts[playerIndex].image;
    }

    public void LeftClick(ref int playerIndex, List<PartBase> parts, Image displayImage)
    {
        if (playerIndex > 0)
        {
            playerIndex--;
        }
        else
        {
            playerIndex = parts.Count - 1;
        }

        displayImage.sprite = parts[playerIndex].image;
    }

    #endregion

    public void BattleStart()
    {
        SetHeadDataValues(headParts, player1HeadIndex, ref Data.player1BeautyLevel, ref Data.player1SmartLevel, ref Data.player1Head);
        SetHeadDataValues(headParts, player2HeadIndex, ref Data.player2BeautyLevel, ref Data.player2SmartLevel, ref Data.player2Head);
        SetBodyDataValues(bodyParts, player1BodyIndex, ref Data.player1HealthLevel, ref Data.player1ArmLevel, ref Data.player1Body);
        SetBodyDataValues(bodyParts, player2BodyIndex, ref Data.player2HealthLevel, ref Data.player2ArmLevel, ref Data.player2Body);
        SetLegDataValues(legParts, player1LegIndex, ref Data.player1SpeedLevel, ref Data.player1LegLevel, ref Data.player1Leg);
        SetLegDataValues(legParts, player2LegIndex, ref Data.player2SpeedLevel, ref Data.player2LegLevel, ref Data.player2Leg);

        SceneManager.LoadScene("Battle Scene");
    }

    public void SetHeadDataValues(List<PartBase> parts, int playerIndex, ref int beautyLevel, ref int smartLevel, ref Sprite partImage)
    {
        PartBase currentPlayerPart = parts[playerIndex];
        HeadPart currentPlayerHead = currentPlayerPart as HeadPart;
        beautyLevel = currentPlayerHead.beautyLevel;
        smartLevel = currentPlayerHead.smartLevel;
        partImage = currentPlayerHead.image;

        Debug.Log(beautyLevel);
        Debug.Log(smartLevel);
    }

    public void SetBodyDataValues(List<PartBase> parts, int playerIndex, ref int healthLevel, ref int armLevel, ref Sprite partImage)
    {
        PartBase currentPlayerPart = parts[playerIndex];
        BodyPart currentPlayerBody = currentPlayerPart as BodyPart;
        healthLevel = currentPlayerBody.healthLevel;
        armLevel = currentPlayerBody.armLevel;
        partImage = currentPlayerBody.image;

        Debug.Log(healthLevel);
        Debug.Log(armLevel);
    }

    public void SetLegDataValues(List<PartBase> parts, int playerIndex, ref int speedLevel, ref int legLevel, ref Sprite partImage)
    {
        PartBase currentPlayerPart = parts[playerIndex];
        LegPart currentPlayerLeg = currentPlayerPart as LegPart;
        speedLevel = currentPlayerLeg.speedLevel;
        legLevel = currentPlayerLeg.legLevel;
        partImage = currentPlayerLeg.image;

        Debug.Log(speedLevel);
        Debug.Log(legLevel);
    }
}
