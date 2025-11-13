using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public GameObject restartGameText;

    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;

    public float waitTime;
    public float shakeTime;

    public float shakeAmount;

    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;
    private Vector3 firstTextPos = new Vector3(0, 131, 0);
    private Vector3 lastTextPos = new Vector3(0, -131, 0);

    public Slider player1HealthBar;
    public Slider player2HealthBar;

    public GameObject player1;
    public GameObject player2;

    private GameObject defensePlayer;

    //Player 1 Stats
    private string player1Name;
    private float player1DodgeProbability;
    private float player1StunProbability;
    private float player1GoodChoiceProbability;

    //Player 2 Stats
    private string player2Name;
    private float player2DodgeProbability;
    private float player2StunProbability;
    private float player2GoodChoiceProbability;

    private bool isPlayer1Start;
    private bool isDefeated = false;

    void Start()
    {
        player1NameText.text = Data.player1Name;
        player2NameText.text = Data.player2Name;

        player1Text.text = "";
        player2Text.text = "";

        if (Data.player1Name != "")
        {
            player1Name = Data.player1Name;
        }
        else
        {
            player1Name = "Player 1";
        }

        if (Data.player2Name != "")
        {
            player2Name = Data.player2Name;
        }
        else
        {
            player2Name = "Player 2";
        }

        //Set players stats depending on their levels
        player1DodgeProbability = Data.player1SpeedLevel / 100f;
        player1StunProbability = Data.player1BeautyLevel / 100f;
        player1GoodChoiceProbability = Data.player1SmartLevel / 100f;

        player2DodgeProbability = Data.player2SpeedLevel / 100f;
        player2StunProbability = Data.player2BeautyLevel / 100f;
        player2GoodChoiceProbability = Data.player2SmartLevel / 100f;

        player1HealthBar.value = Data.player1HealthLevel;
        player2HealthBar.value = Data.player2HealthLevel;

        int randomStart = Random.Range(0, 3);

        if (randomStart > 1)
        {
            isPlayer1Start = true;
            StartCoroutine(PlayerOffense(player1Name, player2Name, player1Text, player2Text, Data.player1ArmLevel, Data.player1LegLevel, player2DodgeProbability, player2StunProbability, player1GoodChoiceProbability));
        }
        else
        {
            isPlayer1Start = false;
            StartCoroutine(PlayerOffense(player2Name, player1Name, player2Text, player1Text, Data.player2ArmLevel, Data.player2LegLevel, player1DodgeProbability, player1StunProbability, player2GoodChoiceProbability));
        }
    }

    private void Update()
    {
        if (isDefeated)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("Start Scene");
            }
        }
    }

    private IEnumerator PlayerOffense(string offensePlayerName, string defensePlayerName, TextMeshProUGUI offensePlayerText, TextMeshProUGUI defensePlayerText, int offensePlayerArmLevel, int offensePlayerLegLevel, float defensePlayerDodgeProbability, float defensePlayerStunProbability, float offensePlayerGoodChoiceProbability)
    {
        yield return StartCoroutine(Attack(offensePlayerName, defensePlayerName, offensePlayerText, defensePlayerText, offensePlayerArmLevel, offensePlayerLegLevel, defensePlayerDodgeProbability, defensePlayerStunProbability, offensePlayerGoodChoiceProbability));
    }

    private IEnumerator Attack(string offensePlayerName, string defensePlayerName, TextMeshProUGUI offensePlayerText, TextMeshProUGUI defensePlayerText, int offensePlayerArmLevel, int offensePlayerLegLevel, float defensePlayerDodgeProbability, float defensePlayerStunProbability, float offensePlayerGoodChoiceProbability)
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
            offensePlayerText.text = $"{offensePlayerName} got stunned by {defensePlayerName}'s beauty!";
            yield return new WaitForSeconds(waitTime);
            defensePlayerText.text = $"{defensePlayerName} feels smug about their face";

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
                    yield return StartCoroutine(ArmAttack(offensePlayerName, defensePlayerName, offensePlayerText, defensePlayerText, offensePlayerArmLevel, defensePlayerDodgeProbability));
                }
                else
                {
                    yield return StartCoroutine(LegAttack(offensePlayerName, defensePlayerName, offensePlayerText, defensePlayerText, offensePlayerLegLevel, defensePlayerDodgeProbability));
                }
            }
            else
            {
                if (!isArmStronger)
                {
                    yield return StartCoroutine(ArmAttack(offensePlayerName, defensePlayerName, offensePlayerText, defensePlayerText, offensePlayerArmLevel, defensePlayerDodgeProbability));
                }
                else
                {
                    yield return StartCoroutine(LegAttack(offensePlayerName, defensePlayerName, offensePlayerText, defensePlayerText, offensePlayerLegLevel, defensePlayerDodgeProbability));
                }
            }
        }
    }

    private IEnumerator ArmAttack(string offensePlayerName, string defensePlayerName, TextMeshProUGUI offensePlayerText, TextMeshProUGUI defensePlayerText, int offensePlayerArmLevel, float defensePlayerDodgeProbability)
    {
        offensePlayerText.text = $"{offensePlayerName} punched {defensePlayerName}!";

        yield return StartCoroutine(CheckResponse(defensePlayerName, defensePlayerText, offensePlayerArmLevel, defensePlayerDodgeProbability));
    }

    private IEnumerator LegAttack(string offensePlayerName, string defensePlayerName, TextMeshProUGUI offensePlayerText, TextMeshProUGUI defensePlayerText, int offensePlayerLegLevel, float defensePlayerDodgeProbability)
    {
        offensePlayerText.text = $"{offensePlayerName} kicked {defensePlayerName}!";

        yield return StartCoroutine(CheckResponse(defensePlayerName, defensePlayerText, offensePlayerLegLevel, defensePlayerDodgeProbability));
    }

    private IEnumerator CheckResponse(string defensePlayerName, TextMeshProUGUI defensePlayerText, int strengthLevel, float defensePlayerDodgeProbability)
    {
        yield return new WaitForSeconds(waitTime);

        float randomDodge = Random.Range(0f, 1f);
        if (randomDodge < defensePlayerDodgeProbability)
        {
            defensePlayerText.text = $"{defensePlayerName} successfully dodged!";

            StartCoroutine(Ending());
        }
        else
        {
            int defensePlayerHealth;
            Slider defensePlayerHealthBar;

            if (!isPlayer1Start)
            {
                defensePlayerHealth = Data.player1HealthLevel;
                defensePlayerHealthBar = player1HealthBar;
                defensePlayer = player1;
                
            }
            else
            {
                defensePlayerHealth = Data.player2HealthLevel;
                defensePlayerHealthBar = player2HealthBar;
                defensePlayer = player2;
            }

            defensePlayerHealth -= strengthLevel;
            defensePlayerHealthBar.value = defensePlayerHealth;

            if (defensePlayerHealth <= 0)
            {
                defensePlayerText.text = $"{defensePlayerName} died!";
                isDefeated = true;

                restartGameText.SetActive(true);
            }
            else
            {
                defensePlayerText.text = $"{defensePlayerName} got hit!";
            }

            StartCoroutine(ShakePlayer(defensePlayer, shakeTime));

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
                StartCoroutine(PlayerOffense(player2Name, player1Name, player2Text, player1Text, Data.player2ArmLevel, Data.player2LegLevel, player1DodgeProbability, player1StunProbability, player2GoodChoiceProbability));
            }
            else
            {
                isPlayer1Start = true;
                StartCoroutine(PlayerOffense(player1Name, player2Name, player1Text, player2Text, Data.player1ArmLevel, Data.player1LegLevel, player2DodgeProbability, player2StunProbability, player1GoodChoiceProbability));
            }
        }
        else
        {
            yield break;
        }

        Debug.Log("player 1 health is " + Data.player1HealthLevel + "player 2 health is " + Data.player2HealthLevel);
    }

    private IEnumerator ShakePlayer(GameObject player, float shakeTime)
    {
        RectTransform rect = player.GetComponent<RectTransform>();
        Vector3 OGPos = rect.anchoredPosition;

        float elapsed = 0f;
        while (elapsed < shakeTime)
        {
            elapsed += Time.deltaTime;
            Vector3 randomOffset = Random.insideUnitSphere * shakeAmount;
            //Got code from https://www.youtube.com/watch?v=-MZD_dZ41rI
            rect.anchoredPosition = OGPos + randomOffset;
            yield return null;
        }

        rect.anchoredPosition = OGPos;
    }
}
