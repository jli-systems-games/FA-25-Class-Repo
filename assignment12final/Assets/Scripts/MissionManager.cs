using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public MissionState currentState = MissionState.Intro;

    [Header("UI")]
    public TextMeshProUGUI objectiveText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetObjective("Go to the Family Bar.");
    }

    public void SetObjective(string text)
    {
        if (objectiveText != null)
        {
            objectiveText.text = text;
        }
    }

    public void OnEnterLocation(LocationType location)
    {
        if (currentState == MissionState.Ended) return;

        switch (currentState)
        {
            case MissionState.Intro:
                if (location == LocationType.Bar)
                {
                    StartIntroAtBar();
                }
                break;

            case MissionState.GoCollectDebt:
                if (location == LocationType.Market)
                {
                    StartDebtChoiceAtMarket();
                }
                break;

            case MissionState.ReportDebt:
                if (location == LocationType.Bar)
                {
                    ReportDebtAtBar();
                }
                break;

            case MissionState.GoAlley:
                if (location == LocationType.Alley)
                {
                    StartAlleyChoice();
                }
                break;

            case MissionState.ReportAlley:
                if (location == LocationType.Bar)
                {
                    ReportAlleyAtBar();
                }
                break;

            case MissionState.GoWarehouse:
                if (location == LocationType.Warehouse)
                {
                    StartWarehouseChoice();
                }
                break;

            case MissionState.FinalDecision:
                if (location == LocationType.Bar)
                {
                    StartFinalDecision();
                }
                break;
        }
    }

    #region 具体事件

    void StartIntroAtBar()
    {
        currentState = MissionState.GoCollectDebt;
        DialogueUI.Instance.ShowLine(
            "Boss: You’re new, kid. Let’s see what you can do. Go collect the money from that diner on Market Street.",
            () =>
            {
                SetObjective("Go to Market Street and talk to the diner owner.");
            }
        );
    }

    void StartDebtChoiceAtMarket()
    {
        DialogueUI.Instance.ShowChoices(
            "The owner looks nervous. He says business is bad and he needs more time.",
            "Threaten him and break something.",
            () =>
            {
                ReputationManager.Instance.AddReputation(10, 0);
                DialogueUI.Instance.ShowLine(
                    "You slam your fist on the table. He pays up with shaking hands.",
                    () =>
                    {
                        currentState = MissionState.ReportDebt;
                        SetObjective("Return to the Bar and report to the Boss.");
                    }
                );
            },
            "Give him more time and warn him gently.",
            () =>
            {
                ReputationManager.Instance.AddReputation(0, 10);
                DialogueUI.Instance.ShowLine(
                    "You lower your voice and give him a week. He nods gratefully.",
                    () =>
                    {
                        currentState = MissionState.ReportDebt;
                        SetObjective("Return to the Bar and report to the Boss.");
                    }
                );
            }
        );
    }

    void ReportDebtAtBar()
    {
        currentState = MissionState.GoAlley;
        DialogueUI.Instance.ShowLine(
            "Boss: Not bad. There’s another problem. One of our guys has been stealing. Deal with him in the back alley.",
            () =>
            {
                SetObjective("Go to the Back Alley.");
            }
        );
    }

    void StartAlleyChoice()
    {
        DialogueUI.Instance.ShowChoices(
            "You find the traitor trembling in the dark alley. He says he only took money to pay his family’s debt.",
            "Punish him hard. Make an example.",
            () =>
            {
                ReputationManager.Instance.AddReputation(15, -5);
                DialogueUI.Instance.ShowLine(
                    "Your blows echo in the alley. No one will dare cross you for a while.",
                    () =>
                    {
                        currentState = MissionState.ReportAlley;
                        SetObjective("Return to the Bar.");
                    }
                );
            },
            "Stage a beating, but secretly let him go.",
            () =>
            {
                ReputationManager.Instance.AddReputation(5, 10);
                DialogueUI.Instance.ShowLine(
                    "You make some noise for the walls to hear, then whisper: \"Run. Don’t come back.\"",
                    () =>
                    {
                        currentState = MissionState.ReportAlley;
                        SetObjective("Return to the Bar.");
                    }
                );
            }
        );
    }

    void ReportAlleyAtBar()
    {
        currentState = MissionState.GoWarehouse;
        DialogueUI.Instance.ShowLine(
            "Boss: Things are heating up. Tonight you watch the goods at the warehouse. Don’t lose anything.",
            () =>
            {
                SetObjective("Go to the Warehouse District.");
            }
        );
    }

    void StartWarehouseChoice()
    {
        DialogueUI.Instance.ShowChoices(
            "Night falls. Another gang attacks the warehouse. Fire, shouting, chaos. Your men look to you.",
            "Protect the goods, no matter the cost.",
            () =>
            {
                ReputationManager.Instance.AddReputation(20, -10);
                DialogueUI.Instance.ShowLine(
                    "You order everyone to hold the line. The goods are safe, but some of your men don’t make it out.",
                    () =>
                    {
                        currentState = MissionState.FinalDecision;
                        SetObjective("Return to the Bar for a final meeting.");
                    }
                );
            },
            "Save your men, abandon the goods.",
            () =>
            {
                ReputationManager.Instance.AddReputation(-5, 20);
                DialogueUI.Instance.ShowLine(
                    "You shout for a retreat. The warehouse burns, but your people live.",
                    () =>
                    {
                        currentState = MissionState.FinalDecision;
                        SetObjective("Return to the Bar for a final meeting.");
                    }
                );
            }
        );
    }

    void StartFinalDecision()
    {
        currentState = MissionState.Ended;

        int fear = ReputationManager.Instance.familyFear;
        int respect = ReputationManager.Instance.familyRespect;

        Debug.Log($"FINAL REPUTATION -> FEAR: {fear}, RESPECT: {respect}");

        int diff = fear - respect; // 正：Fear 多，负：Respect 多

        // 参数区：以后不满意可以自己改
        int highThreshold = 35;  // "非常高"
        int midThreshold = 30;  // "够高"
        int minOther = 15;  // 另一条的最低量，表示“不是零”
        int dominanceMargin = 25; // 超过这个差距就是明显偏向一边

        // 1) 好结局：Fear & Respect 都高，且差距不大
        if (fear >= midThreshold && respect >= midThreshold && Mathf.Abs(diff) <= dominanceMargin)
        {
            EndingUI.Instance.ShowEnding(
                "Harbor Boss",
                "People lower their voices when they say your name.\n" +
                "Not just out of fear, but out of habit and respect.\n\n" +
                "You kept this broken harbor standing a little longer,\n" +
                "and for now, that is enough."
            );
        }
        // 2) Bloody King：Fear 非常高，Respect 也有，但Fear 明显压过Respect
        else if (fear >= highThreshold && respect >= minOther && diff >= dominanceMargin)
        {
            EndingUI.Instance.ShowEnding(
                "Bloody King",
                "Everyone on the street knows your name.\n" +
                "They lower their eyes when you pass.\n\n" +
                "They obey you not because they trust you,\n" +
                "but because they are terrified."
            );
        }
        // 3) Gentleman Boss：Respect 非常高，Fear 也有，但明显偏向“被喜欢”
        else if (respect >= highThreshold && fear >= minOther && diff <= -dominanceMargin)
        {
            EndingUI.Instance.ShowEnding(
                "Gentleman Boss",
                "Your enemies speak your name with caution.\n" +
                "Your people speak it with something like affection.\n\n" +
                "You tried to hold power without drowning in blood.\n" +
                "In this city, that's almost a miracle."
            );
        }
        // 4) 其他情况：Burned Out
        else
        {
            EndingUI.Instance.ShowEnding(
                "Burned Out",
                "You tried to balance terror and mercy without choosing a side.\n" +
                "Or you never built enough of either.\n\n" +
                "When the storm hit, no one knew which version of you to follow.\n" +
                "The city moves on. Someone else will take your place."
            );
        }
    }


    #endregion
}
