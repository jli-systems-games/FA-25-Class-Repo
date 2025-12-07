using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void HandleInteraction(string id)
    {
        switch (id)
        {
            // ======================
            // 1. Market 小店老板（Shop_A）
            // 任务：TalkToShopOwner
            // ======================
            case "Shop_A":
                DialogueUI.Instance.ShowChoices(
                    "Shop owner: \"Business is dying. Rent is up. Your people still want the same money.\"",
                    "Tell him to stop whining.",
                    () =>
                    {
                        // Fear +3
                        ReputationManager.Instance.AddReputation(3, 0);
                        DialogueUI.Instance.ShowLine(
                            "You: \"Whining never paid anyone's rent. Figure it out or close.\" He shuts up, eyes full of quiet anger.",
                            null
                        );

                        // 支线：和店主交谈
                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToShopOwner");
                    },
                    "Listen for a moment and give a short, sharp advice.",
                    () =>
                    {
                        // Respect +4
                        ReputationManager.Instance.AddReputation(0, 4);
                        DialogueUI.Instance.ShowLine(
                            "You: \"Cut what doesn't sell. Cut hours. Bleed slower, or bleed out. Your choice.\" He nods, half grateful, half annoyed.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToShopOwner");
                    }
                );
                break;

            // ======================
            // 2. 卖烟小摊（Shop_B）
            // 任务：TalkToCigSeller
            // ======================
            case "Shop_B":
                DialogueUI.Instance.ShowChoices(
                    "Cigarette seller: \"You Lin guys smoke my stock, pay when you feel like it.\"",
                    "Remind her why she still has a stall.",
                    () =>
                    {
                        // Fear +4
                        ReputationManager.Instance.AddReputation(4, 0);
                        DialogueUI.Instance.ShowLine(
                            "You: \"You still have a stall because we 'feel like it'. Remember that.\" She laughs nervously and hands you a pack for free.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToCigSeller");
                    },
                    "Drop a few bills on the counter with a sharp comment.",
                    () =>
                    {
                        // Respect +3
                        ReputationManager.Instance.AddReputation(0, 3);
                        DialogueUI.Instance.ShowLine(
                            "You toss some notes down. \"Count it. Complain after it’s not enough.\" She counts, then quietly thanks you.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToCigSeller");
                    }
                );
                break;

            // ======================
            // 3. 撞到你的街头小孩（Street_Kid_1）
            // 任务：HelpStreetKid （只有帮他的选项算完成）
            // ======================
            case "Street_Kid_1":
                DialogueUI.Instance.ShowChoices(
                    "A kid runs into you and drops a plastic bag. Inside: cheap groceries.",
                    "Scare him so he won't run blind again.",
                    () =>
                    {
                        // Fear +3
                        ReputationManager.Instance.AddReputation(3, 0);
                        DialogueUI.Instance.ShowLine(
                            "You: \"Next time you run like that, it won't be me you hit. It'll be a truck.\" The kid nods, terrified, and runs off clutching the bag.",
                            null
                        );
                    },
                    "Mock him, then help pick up the groceries.",
                    () =>
                    {
                        // Respect +4
                        ReputationManager.Instance.AddReputation(0, 4);
                        DialogueUI.Instance.ShowLine(
                            "You: \"If you're going to sprint, try growing eyes first.\" You toss the groceries back into his arms. He mumbles a thanks and bows.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("HelpStreetKid");
                    }
                );
                break;

            // ======================
            // 4. 长椅上的老头（Street_OldMan_1）
            // 任务：ListenOldMan
            // ======================
            case "Street_OldMan_1":
                DialogueUI.Instance.ShowLine(
                    "Old man on a bench: \"Used to be kids played here. Now it's drunks and debt collectors.\" You don't correct him.",
                    () =>
                    {
                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("ListenOldMan");
                    }
                );
                break;

            // ======================
            // 5. 巷子涂鸦（Alley_Graffiti_1）
            // 任务：CheckGraffiti
            // ======================
            case "Alley_Graffiti_1":
                DialogueUI.Instance.ShowLine(
                    "Old graffiti, half-faded names and dates. You remember none of them stayed around.",
                    () =>
                    {
                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("CheckGraffiti");
                    }
                );
                break;

            // ======================
            // 6. 巷子里的流浪猫（Alley_StrayCat_1）
            // 任务：FeedCat（只有喂的时候完成）
            // ======================
            case "Alley_StrayCat_1":
                DialogueUI.Instance.ShowChoices(
                    "A stray cat watches you from a trash can, eyes glowing in the dark.",
                    "Hiss back at it and walk on.",
                    () =>
                    {
                        DialogueUI.Instance.ShowLine(
                            "You: \"Stare all you want. I bite harder.\" The cat flicks its tail and vanishes into the shadows.",
                            null
                        );
                    },
                    "Toss it a scrap from your pocket.",
                    () =>
                    {
                        // Respect +2
                        ReputationManager.Instance.AddReputation(0, 2);
                        DialogueUI.Instance.ShowLine(
                            "You: \"Fine. You eat, I pretend I didn't see you.\" The cat snatches the food and keeps staring.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("FeedCat");
                    }
                );
                break;

            // ======================
            // 7. 仓库门口小弟（Warehouse_Guard_1）
            // 任务：TalkToGuard（两个选项都算完成）
            // ======================
            case "Warehouse_Guard_1":
                DialogueUI.Instance.ShowChoices(
                    "A younger guard is nodding off at the warehouse gate.",
                    "Wake him up with a hard shove and a threat.",
                    () =>
                    {
                        // Fear +4
                        ReputationManager.Instance.AddReputation(4, 0);
                        DialogueUI.Instance.ShowLine(
                            "You shove his shoulder. \"Sleep when you're dead. Or when we are.\" He jolts awake, pale.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToGuard");
                    },
                    "Wake him with a jab and bitter humor.",
                    () =>
                    {
                        // Respect +3
                        ReputationManager.Instance.AddReputation(0, 3);
                        DialogueUI.Instance.ShowLine(
                            "You tap his head. \"If you get us robbed, you'll wish you never learned how to sleep.\" He laughs nervously and straightens up.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToGuard");
                    }
                );
                break;

            // ======================
            // 8. Bar 里的骰子小游戏（Bar_DiceGame_1）
            // 任务：PlayDice
            // ======================
            case "Bar_DiceGame_1":
                DialogueUI.Instance.ShowChoices(
                    "A couple of guys invite you to a quick dice game.",
                    "Play and win.",
                    () =>
                    {
                        DialogueUI.Instance.ShowLine(
                            "You roll, and the table groans. \"Beginner's luck,\" someone mutters. You smirk.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("PlayDice");
                    },
                    "Play and lose on purpose.",
                    () =>
                    {
                        DialogueUI.Instance.ShowLine(
                            "You roll just badly enough to lose. \"See? I'm generous,\" you say. They laugh, not sure if you're joking.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("PlayDice");
                    }
                );
                break;

            // ======================
            // 9. 重建计划海报（Crossroad_DevPoster_1）
            // 任务：ReadDevPoster（看 / 撕都算）
            // ======================
            case "Crossroad_DevPoster_1":
                DialogueUI.Instance.ShowChoices(
                    "A bright poster reads: \"Urban Renewal Project – A New Future for the Harbor District.\"",
                    "Rip it down without a word.",
                    () =>
                    {
                        // Fear +3
                        ReputationManager.Instance.AddReputation(3, 0);
                        DialogueUI.Instance.ShowLine(
                            "You tear it off. The wall looks older, and somehow more honest.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("ReadDevPoster");
                    },
                    "Smirk and leave it there.",
                    () =>
                    {
                        // Respect +2（或者理解为现实感）
                        ReputationManager.Instance.AddReputation(0, 2);
                        DialogueUI.Instance.ShowLine(
                            "You: \"New future, same old lies.\" You walk on, leaving the poster to fade in the rain.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("ReadDevPoster");
                    }
                );
                break;

            // ======================
            // 10. 警局附近盯着你看的人（Police_CornerWitness_1）
            // 任务：ConfrontWatcher
            // ======================
            case "Police_CornerWitness_1":
                DialogueUI.Instance.ShowChoices(
                    "Near the station, a man stares at you too long, then looks away.",
                    "Stare him down until he panics.",
                    () =>
                    {
                        // Fear +4
                        ReputationManager.Instance.AddReputation(4, 0);
                        DialogueUI.Instance.ShowLine(
                            "You don't blink. He folds first, vanishing into a side street like a roach.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("ConfrontWatcher");
                    },
                    "Walk past with a cutting remark.",
                    () =>
                    {
                        // Respect +3（不用动手，只用气场和嘴）
                        ReputationManager.Instance.AddReputation(0, 3);
                        DialogueUI.Instance.ShowLine(
                            "You: \"If you're going to stare, at least charge a ticket.\" He flinches, then pretends to check his phone.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("ConfrontWatcher");
                    }
                );
                break;

            // ======================
            // 这里可以继续加你自己的主线交互 case
            // 比如 "Main_MarketDebt", "Main_Traitorguy" 之类
            // ======================

            default:
                Debug.Log("No interaction handler for id: " + id);
                break;
        }
    }
}
