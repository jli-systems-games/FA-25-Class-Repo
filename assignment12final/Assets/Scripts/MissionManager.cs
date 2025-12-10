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
        SetObjective("Head to the family bar to start your last night on the street.");
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

    #region Main story beats

    void StartIntroAtBar()
    {
        currentState = MissionState.GoCollectDebt;

        DialogueUI.Instance.ShowLine(
            "Boss: \"Tomorrow morning, the city talks about erasing this harbor off the map.\"\n" +
            "He looks you over like he’s weighing something.\n" +
            "Boss: \"Tonight, you walk it for me. Start simple: that diner on Market Street still owes us. Go see if they remember.\"",
            () =>
            {
                SetObjective("Go to Market Street and talk to the diner owner about the debt.");
            }
        );
    }

    void StartDebtChoiceAtMarket()
    {
        DialogueUI.Instance.ShowChoices(
            "The diner owner is wiping down empty tables. You can smell broth and anxiety.\n" +
            "He rubs his hands on his apron.\n" +
            "\"Business is dead,\" he says. \"With that redevelopment hearing tomorrow… I just need more time.\"",

            "Make it very clear that the city might change, but his debt doesn’t.",
            () =>
            {

                ReputationManager.Instance.AddReputation(10, 0);
                DialogueUI.Instance.ShowLine(
                    "You lean on the counter.\n" +
                    "You: \"Maps change. Names change. Numbers don’t. You owe us now, not on some new brochure.\"\n" +
                    "He fumbles open the till with shaking hands and counts out what he can.",
                    () =>
                    {
                        currentState = MissionState.ReportDebt;
                        SetObjective("Return to the bar and tell the Boss how you handled the diner.");
                    }
                );
            },

            "Give him a deadline, not a miracle, and keep your voice low.",
            () =>
            {
                ReputationManager.Instance.AddReputation(0, 10);
                DialogueUI.Instance.ShowLine(
                    "You tap the counter with two fingers.\n" +
                    "You: \"One more week. After the hearing, if this place is still standing, I expect this debt off your books.\"\n" +
                    "Relief floods his face.\n" +
                    "\"Thank you,\" he says, bowing his head. \"I won’t forget this.\"",
                    () =>
                    {
                        currentState = MissionState.ReportDebt;
                        SetObjective("Return to the bar and tell the Boss how you handled the diner.");
                    }
                );
            }
        );
    }

    void ReportDebtAtBar()
    {
        currentState = MissionState.GoAlley;

        DialogueUI.Instance.ShowLine(
            "Back at the bar, the Boss is nursing a drink he hasn’t touched.\n" +
            "Boss: \"Money is one thing. Loyalty is another.\"\n" +
            "He sets the glass down.\n" +
            "Boss: \"One of our own has been skimming. He’s hiding in the back alley tonight. Go… sort his priorities out.\"",
            () =>
            {
                SetObjective("Go to the back alley and deal with the traitor.");
            }
        );
    }

    void StartAlleyChoice()
    {
        DialogueUI.Instance.ShowChoices(
            "You find him in the alley, back against the wall, eyes darting between you and the exit.\n" +
            "\"I only took it to pay my family’s debts,\" he spits out. \"Once they’re clear, I was going to put it back.\"\n" +
            "The street is quiet enough that your next move will echo.",

            "Make an example out of him. Let the bricks remember the sound.",
            () =>
            {
                ReputationManager.Instance.AddReputation(15, -5);
                DialogueUI.Instance.ShowLine(
                    "You don’t give him time to beg twice.\n" +
                    "Your blows slam him into the wall; the alley records every impact.\n" +
                    "By the time you’re done, anyone within earshot knows what crossing you sounds like.",
                    () =>
                    {
                        currentState = MissionState.ReportAlley;
                        SetObjective("Return to the bar and report what happened in the alley.");
                    }
                );
            },

            "Stage a beating for the walls, and whisper the real verdict in his ear.",
            () =>
            {
                ReputationManager.Instance.AddReputation(5, 10);
                DialogueUI.Instance.ShowLine(
                    "You grab his collar and slam him once, hard enough for the sound to carry.\n" +
                    "Then you lean in close.\n" +
                    "You: \"Run. Tonight. If I ever see you again, I won’t be acting.\"\n" +
                    "He nods, eyes wet, and disappears into the dark as you keep throwing punches at the empty air.",
                    () =>
                    {
                        currentState = MissionState.ReportAlley;
                        SetObjective("Return to the bar and report what happened in the alley.");
                    }
                );
            }
        );
    }

    void ReportAlleyAtBar()
    {
        currentState = MissionState.GoWarehouse;

        DialogueUI.Instance.ShowLine(
            "The Boss listens, face unreadable.\n" +
            "Boss: \"The city will talk about numbers tomorrow. We still have cargo tonight.\"\n" +
            "He slides a key across the counter.\n" +
            "Boss: \"Warehouse down by the edge. Our goods, our name. You watch it until sunrise.\"",
            () =>
            {
                SetObjective("Go to the warehouse district and prepare for the night.");
            }
        );
    }

    void StartWarehouseChoice()
    {
        DialogueUI.Instance.ShowChoices(
            "Night deepens. Shouting rips through the dark as another crew hits the warehouse.\n" +
            "Shadows, firelight, and the sound of breaking glass. Your people turn toward you, waiting for a call.",

            "Order them to hold the line. The crates matter more than bodies.",
            () =>
            {
                ReputationManager.Instance.AddReputation(20, -10);
                DialogueUI.Instance.ShowLine(
                    "You: \"No one touches those crates. Anyone who runs doesn’t come back.\"\n" +
                    "The fight is ugly and close. By the end of it, the goods are intact.\n" +
                    "Some of your people are not.",
                    () =>
                    {
                        currentState = MissionState.FinalDecision;
                        SetObjective("Return to the bar for a final meeting.");

                        if (BGMManager.Instance != null)
                        {
                            BGMManager.Instance.PlayFinalTheme();
                        }

                        if (CameraShake.Instance != null)
                        {
                            CameraShake.Instance.Shake(0.35f, 0.35f);
                        }
                    }
                );
            },

            "Call the retreat and drag everyone you can out of the fire.",
            () =>
            {

                ReputationManager.Instance.AddReputation(-5, 20);
                DialogueUI.Instance.ShowLine(
                    "You: \"Fall back! Live to argue with me tomorrow.\" \n" +
                    "You pull bodies out of the smoke until you can’t see the crates anymore.\n" +
                    "By the time the flames die down, the cargo is gone—but your people are breathing.",
                    () =>
                    {
                        currentState = MissionState.FinalDecision;
                        SetObjective("Return to the bar for a final meeting.");

                        if (BGMManager.Instance != null)
                        {
                            BGMManager.Instance.PlayFinalTheme();
                        }

                        if (CameraShake.Instance != null)
                        {
                            CameraShake.Instance.Shake(0.35f, 0.35f);
                        }
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

        fear = Mathf.Max(0, fear);
        respect = Mathf.Max(0, respect);

        Debug.Log($"FINAL REPUTATION -> FEAR: {fear}, RESPECT: {respect}");

        int diff = fear - respect;


        int balancedThreshold = 25;  
        int highThreshold = 30; 
        int dominanceMargin = 8;

        if (fear >= balancedThreshold &&
            respect >= balancedThreshold &&
            Mathf.Abs(diff) <= dominanceMargin)
        {
            EndingUI.Instance.ShowEnding(
                "Harbor Boss",
                "The old harbor hasn’t been flattened yet, but your name is already "
              + "etched into the cracks between its bricks.\n\n"
              + "Some people fear you. Some respect you. Most have learned to lower "
              + "their voice when they say your name.\n"
              + "Before the city rewrites this place completely, you’ve held it in a shape "
              + "you can live with."
            );
        }

        else if (fear >= highThreshold && diff >= dominanceMargin)
        {
            EndingUI.Instance.ShowEnding(
                "Bloody King",
                "Everyone on this street knows your name.\n"
              + "They nod when they see you. They step aside. They look down.\n\n"
              + "They follow you not because they trust you, but because they’ve "
              + "done the math on what it costs to make you angry.\n"
              + "The street survives for now, but nobody dares use the word \"future\" in front of you."
            );
        }

        else if (respect >= highThreshold && diff <= -dominanceMargin)
        {
            EndingUI.Instance.ShowEnding(
                "Gentleman Boss",
                "Your enemies lower their voices when they speak your name;\n"
              + "your people say it with something like affection.\n\n"
              + "You tried not to spill more blood than this street could stand.\n"
              + "In this line of work, that’s already a kind of stubborn idealism."
            );
        }

        else
        {
            EndingUI.Instance.ShowEnding(
                "Burned Out",
                "You moved, talked, made choices—just not enough in any one direction.\n\n"
              + "When the machines finally rolled in, nobody knew whether to fear you "
              + "or to follow you.\n"
              + "The harbor changed its face, and you slipped through the gap without "
              + "leaving much of a mark."
            );
        }
    }


    #endregion
}
