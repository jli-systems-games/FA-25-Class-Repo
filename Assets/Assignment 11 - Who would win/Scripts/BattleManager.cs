using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public float waitTime;

    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;
    private Vector3 firstTextPos = new Vector3(0, 131, 0);
    private Vector3 lastTextPos = new Vector3(0, -131, 0);

    //Player 1 Stats
    private string player1Name = "Player 1";
    private float player1AttackProbability;
    private float player1DodgeProbability;
    private float player1StunProbability;
    private float player1GoodChoiceProbability;
    private float player1ArmStrength;
    private float player1LegStrength;

    //Player 2 Stats
    private string player2Name = "Player 2";
    private float player2AttackProbability;
    private float player2DodgeProbability;
    private float player2StunProbability;
    private float player2GoodChoiceProbability;
    private float player2ArmStrength;
    private float player2LegStrength;

    private bool isPlayer1Start = true;
    private bool isDefeated = false;

    void Start()
    {
        player1Text.text = "";
        player2Text.text = "";

        //Set players stats depending on their levels
        player1DodgeProbability = Data.player1SpeedLevel / 100f;
        player1StunProbability = Data.player1BeautyLevel / 100f;
        player1GoodChoiceProbability = Data.player1SmartLevel / 100f;

        player2DodgeProbability = Data.player2SpeedLevel / 100f;
        player2StunProbability = Data.player2BeautyLevel / 100f;
        player2GoodChoiceProbability = Data.player2SmartLevel / 100f;

        StartCoroutine(PlayerOffense(player1Text, player2Text, Data.player1ArmLevel, Data.player1LegLevel, player2DodgeProbability, player2StunProbability, player1GoodChoiceProbability));
    }

    private IEnumerator PlayerOffense(TextMeshProUGUI offensePlayerText, TextMeshProUGUI defensePlayerText, int offensePlayerArmLevel, int offensePlayerLegLevel, float defensePlayerDodgeProbability, float defensePlayerStunProbability, float offensePlayerGoodChoiceProbability)
    {
        yield return StartCoroutine(Attack(offensePlayerText, defensePlayerText, offensePlayerArmLevel, offensePlayerLegLevel, defensePlayerDodgeProbability, defensePlayerStunProbability, offensePlayerGoodChoiceProbability));
    }

    private IEnumerator Attack(TextMeshProUGUI offensePlayerText, TextMeshProUGUI defensePlayerText, int offensePlayerArmLevel, int offensePlayerLegLevel, float defensePlayerDodgeProbability, float defensePlayerStunProbability, float offensePlayerGoodChoiceProbability)
    {
        if (isPlayer1Start)
        {
            player2Text.text = "";
            player1Text.rectTransform.localPosition = firstTextPos;
            player2Text.rectTransform.localPosition = lastTextPos;
        }
        else
        {
            player1Text.text = "";
            player1Text.rectTransform.localPosition = lastTextPos;
            player2Text.rectTransform.localPosition = firstTextPos;
        }

        float randomStun = Random.Range(0f, 1f);
        if (randomStun < defensePlayerStunProbability)
        {
            offensePlayerText.text = "Player got stunned by player's beauty!";
            yield return new WaitForSeconds(waitTime);
            defensePlayerText.text = "Player feels smug about their face";

            yield return StartCoroutine(Ending());
        }
        else
        {
            bool isArmStronger;

            if (offensePlayerArmLevel > offensePlayerLegLevel)
            {
                isArmStronger = true;
            }
            else
            {
                isArmStronger = false;
            }

            float attackRandom = Random.Range(0f, 1f);

            if (attackRandom < offensePlayerGoodChoiceProbability)
            {
                if (isArmStronger)
                {
                    yield return StartCoroutine(ArmAttack(offensePlayerText, defensePlayerText, offensePlayerArmLevel, defensePlayerDodgeProbability));
                }
                else
                {
                    yield return StartCoroutine(LegAttack(offensePlayerText, defensePlayerText, offensePlayerLegLevel, defensePlayerDodgeProbability));
                }
            }
            else
            {
                if (!isArmStronger)
                {
                    yield return StartCoroutine(ArmAttack(offensePlayerText, defensePlayerText, offensePlayerArmLevel, defensePlayerDodgeProbability));
                }
                else
                {
                    yield return StartCoroutine(LegAttack(offensePlayerText, defensePlayerText, offensePlayerLegLevel, defensePlayerDodgeProbability));
                }
            }
        }
    }

    private IEnumerator ArmAttack(TextMeshProUGUI offensePlayerText, TextMeshProUGUI defensePlayerText, int offensePlayerArmLevel, float defensePlayerDodgeProbability)
    {
        offensePlayerText.text = "Player punched player!";

        yield return StartCoroutine(CheckResponse(defensePlayerText, offensePlayerArmLevel, defensePlayerDodgeProbability));
    }

    private IEnumerator LegAttack(TextMeshProUGUI offensePlayerText, TextMeshProUGUI defensePlayerText, int offensePlayerLegLevel, float defensePlayerDodgeProbability)
    {
        offensePlayerText.text = "Player kicked player!";

        yield return StartCoroutine(CheckResponse(defensePlayerText, offensePlayerLegLevel, defensePlayerDodgeProbability));
    }

    private IEnumerator CheckResponse(TextMeshProUGUI defensePlayerText, int strengthLevel, float defensePlayerDodgeProbability)
    {
        yield return new WaitForSeconds(waitTime);

        float randomDodge = Random.Range(0f, 1f);
        if (randomDodge < defensePlayerDodgeProbability)
        {
            defensePlayerText.text = "Player successfully dodged!";

            StartCoroutine(Ending());
        }
        else
        {
            int defensePlayerHealth;

            if (!isPlayer1Start)
            {
                defensePlayerHealth = Data.player1HealthLevel;
            }
            else
            {
                defensePlayerHealth = Data.player2HealthLevel;
            }

            defensePlayerHealth -= strengthLevel;

            if (defensePlayerHealth <= 0)
            {
                defensePlayerText.text = "Player is dead";
                isDefeated = true;
            }
            else
            {
                defensePlayerText.text = "Player got hit!";
            }

            if (!isPlayer1Start)
            {
                Data.player1HealthLevel = defensePlayerHealth;
            }
            else
            {
                Data.player2HealthLevel = defensePlayerHealth;
            }

            StartCoroutine(Ending());
        }
    }

    private IEnumerator Ending()
    {
        yield return new WaitForSeconds(waitTime);

        if (!isDefeated)
        {
            if (isPlayer1Start)
            {
                isPlayer1Start = false;
                StartCoroutine(PlayerOffense(player2Text, player1Text, Data.player2ArmLevel, Data.player2LegLevel, player1DodgeProbability, player1StunProbability, player2GoodChoiceProbability));
            }
            else
            {
                isPlayer1Start = true;
                StartCoroutine(PlayerOffense(player1Text, player2Text, Data.player1ArmLevel, Data.player1LegLevel, player2DodgeProbability, player2StunProbability, player1GoodChoiceProbability));
            }
        }
        else
        {
            yield break;
        }

        Debug.Log("player 1 health is " + Data.player1HealthLevel + "player 2 health is " + Data.player2HealthLevel);
    }
}
