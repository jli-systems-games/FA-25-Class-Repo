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
            case "Shop_A":
                DialogueUI.Instance.ShowChoices(
                    "The market shop owner is closing up, counting what¡¯s left.\n" +
                    "\"I heard they¡¯re talking redevelopment at city hall tomorrow,\" he mutters.\n" +
                    "\"On those new blueprints my stall probably doesn¡¯t even exist.\n" +
                    "But your family still wants the same money.\"",
                    "Cut him off and tell him to stop whining.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(3, 0);
                        DialogueUI.Instance.ShowLine(
                            "You: \"Whining won¡¯t pay your rent. If you can¡¯t hold the spot, someone else will.\n" +
                            "Street doesn¡¯t care who stands here, only who pays on time.\"\n" +
                            "He bites back whatever he was about to say, eyes hard and quiet.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToShopOwner");
                    },
                    "Let him talk, then give him some cold, sharp advice.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(0, 4);
                        DialogueUI.Instance.ShowLine(
                            "You: \"Cut the dead stock first. Trim hours before you bleed yourself dry.\n" +
                            "If the city tears this place up tomorrow, you¡¯ll need cash more than pride.\"\n" +
                            "He nods slowly. Still complains, but you can see he¡¯s already rearranging shelves in his head.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToShopOwner");
                    }
                );
                break;

            case "Shop_B":
                DialogueUI.Instance.ShowChoices(
                    "The cigarette stall owner, a woman in a worn bomber jacket, is half-packing up her crates.\n" +
                    "\"So they¡¯re really going to city hall tomorrow for that big redevelopment meeting, huh?\" she says.\n" +
                    "\"Once they redraw this harbor, what does a tiny stall like mine count as? A mistake on the map?\"\n" +
                    "She flicks ash to the ground and looks at you.\n" +
                    "\"Meanwhile, your Lin people smoke my stock and call it ¡®put it on the tab¡¯.\"",
                    "Remind her why she still has a stall on this street.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(4, 0);
                        DialogueUI.Instance.ShowLine(
                            "You: \"You¡¯re still on this corner because we allow it.\n" +
                            "When the maps change, the only question is whether we bother to move you onto the new one.\"\n" +
                            "She gives a tight, crooked smile, tears open a pack, and presses it into your hand without another word.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToCigSeller");
                    },
                    "Drop some bills and answer her with a barbed kind of fairness.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(0, 3);
                        DialogueUI.Instance.ShowLine(
                            "You slap a few notes down on the crate.\n" +
                            "You: \"Call it tonight¡¯s bill. Tomorrow, if the street survives the meeting, we¡¯ll see who¡¯s still selling what.\"\n" +
                            "She counts the money, her voice softer: \"Then come back and smoke on the house¡ªif the house is still here.\"",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToCigSeller");
                    }
                );
                break;

            case "Street_Kid_1":
                DialogueUI.Instance.ShowChoices(
                    "A kid slams into you at full speed. A plastic bag explodes across the pavement¡ªcheap vegetables everywhere.",

                    "Scare him so he won¡¯t run blind again.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(3, 0);
                        DialogueUI.Instance.ShowLine(
                            "You: \"Next time you run like that, it won¡¯t be me you hit. It¡¯ll be a car.\n" +
                            "Cars don¡¯t say sorry. They just don¡¯t stop.\"\n" +
                            "He nods frantically, grabs what he can and runs, still looking back over his shoulder.",
                            null
                        );
                    },

                    "Mock him, then help pick up the groceries.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(0, 4);
                        DialogueUI.Instance.ShowLine(
                            "You: \"You sprint like that without eyes, you¡¯ll end up on a poster, not a podium.\"\n" +
                            "You still crouch down, shove vegetables back into the torn bag and push it into his arms.\n" +
                            "He mumbles a thank you and bows before bolting off into the dark.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("HelpStreetKid");
                    }
                );
                break;

            case "Street_OldMan_1":
                DialogueUI.Instance.ShowLine(
                    "An old man sits on a bench at the corner, staring at the street like it¡¯s a TV he can¡¯t switch off.\n" +
                    "\"They say tomorrow they¡¯re going to rename this whole block at city hall,\" he sighs.\n" +
                    "\"New district, new brand, new everything.\"\n" +
                    "He shakes his head.\n" +
                    "\"Used to be kids everywhere here. Now it¡¯s drunks and debt collectors.\"\n" +
                    "You don¡¯t correct him. You just listen, then keep walking.",
                    () =>
                    {
                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("ListenOldMan");
                    }
                );
                break;

            case "Alley_Graffiti_1":
                DialogueUI.Instance.ShowLine(
                    "The graffiti on the alley wall is half-gone¡ªnames and dates fading into damp concrete.\n" +
                    "You recognize a few. None of them lasted long enough to worry about redevelopment meetings.",
                    () =>
                    {
                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("CheckGraffiti");
                    }
                );
                break;

            case "Alley_StrayCat_1":
                DialogueUI.Instance.ShowChoices(
                    "A scrawny alley cat watches you from the lip of a trash can, eyes glowing in the dark.\n" +
                    "Somewhere farther down the street, you can hear someone tearing down old flyers about tomorrow¡¯s hearing.",

                    "Hiss back and walk on.",
                    () =>
                    {
                        DialogueUI.Instance.ShowLine(
                            "You: \"Stare all you want. If this place goes under, you¡¯ll be digging in construction dust instead.\"\n" +
                            "The cat flicks its tail once and melts into the shadows.",
                            null
                        );
                    },

                    "Toss it a scrap from your pocket.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(0, 2);
                        DialogueUI.Instance.ShowLine(
                            "You: \"Fine. Eat while the street is still here to feed you.\"\n" +
                            "The cat snatches the food and stays where it is, still staring like it knows more than it should.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("FeedCat");
                    }
                );
                break;

            case "Warehouse_Guard_1":
                DialogueUI.Instance.ShowChoices(
                    "A younger guard is half asleep at the warehouse gate, chin nearly hitting his chest.\n" +
                    "\"Of all nights to pull gate duty, it had to be the one before the redevelopment hearing,\" he mutters.",

                    "Shove him awake and remind him what happens if he screws this up.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(4, 0);
                        DialogueUI.Instance.ShowLine(
                            "You shove his shoulder hard.\n" +
                            "You: \"If something walks in tonight because you were napping, it won¡¯t be the city that tears this place down. It¡¯ll be us.\"\n" +
                            "He snaps fully awake, color draining from his face.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToGuard");
                    },

                    "Tap his head and wake him with bitter humor.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(0, 3);
                        DialogueUI.Instance.ShowLine(
                            "You rap your knuckles lightly against his skull.\n" +
                            "You: \"If the only thing guarding this place before tomorrow¡¯s meeting is your snoring, we¡¯re already done.\"\n" +
                            "He laughs nervously and forces himself to stand straighter.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("TalkToGuard");
                    }
                );
                break;

            case "Bar_DiceGame_1":
                DialogueUI.Instance.ShowChoices(
                    "In the corner of the bar, a few guys wave you over.\n" +
                    "\"One quick round of dice?\" one of them grins.\n" +
                    "\"Tomorrow they talk about tearing this place down anyway. Might as well lose our money to you while it still exists.\"",

                    "Play to win and take their money.",
                    () =>
                    {
                        DialogueUI.Instance.ShowLine(
                            "You shake the cup, let the dice fall, and the table groans in unison.\n" +
                            "\"Of course,\" someone mutters. \"Last night on this street and he¡¯s still taking our cash.\"\n" +
                            "You just smirk. If the street disappears tomorrow, the money won¡¯t matter anyway.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("PlayDice");
                    },

                    "Lose on purpose and treat it like a farewell gift.",
                    () =>
                    {
                        DialogueUI.Instance.ShowLine(
                            "You judge the numbers and hold back just enough to lose.\n" +
                            "You: \"Enjoy it. Spend it before the city paints over this place.\"\n" +
                            "They laugh too loud, half at the money, half at the fact that you showed up at all tonight.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("PlayDice");
                    }
                );
                break;

            case "Crossroad_DevPoster_1":
                DialogueUI.Instance.ShowChoices(
                    "A bright poster is pasted to the wall:\n" +
                    "\"Urban Renewal Project ¨C A New Future for the Harbor District.\"\n" +
                    "Tiny print at the bottom reads: \"Public hearing date: tomorrow morning.\"",

                    "Rip it down without a word.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(3, 0);
                        DialogueUI.Instance.ShowLine(
                            "You tear the poster off in one motion. The paper shreds in your hand.\n" +
                            "The wall looks older, more honest¡ªcracked, stained, and still here. For now.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("ReadDevPoster");
                    },

                    "Leave it there and walk away with a smirk.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(0, 2);
                        DialogueUI.Instance.ShowLine(
                            "You: \"New future, same old lies.\"\n" +
                            "You walk on, letting the poster stay, waiting for rain and time to do their work.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("ReadDevPoster");
                    }
                );
                break;

            case "Police_CornerWitness_1":
                DialogueUI.Instance.ShowChoices(
                    "Near the station, a man lingers at the corner, watching you too long.\n" +
                    "On the notice board behind him, the flyer about tomorrow¡¯s redevelopment hearing flaps in the wind.\n" +
                    "When your eyes meet, he suddenly pretends to be interested in anything else.",

                    "Stare him down until he breaks.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(4, 0);
                        DialogueUI.Instance.ShowLine(
                            "You stop walking and don¡¯t blink.\n" +
                            "He lasts a few seconds before folding, slipping into the side street like a roach avoiding the light.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("ConfrontWatcher");
                    },

                    "Walk past and cut him with one line.",
                    () =>
                    {
                        ReputationManager.Instance.AddReputation(0, 3);
                        DialogueUI.Instance.ShowLine(
                            "You pass close enough for him to smell your smoke.\n" +
                            "You: \"If you¡¯re going to stare this hard the night before a hearing, at least charge for tickets.\"\n" +
                            "He flinches and fumbles for his phone, pretending he was never looking at you at all.",
                            null
                        );

                        if (SideQuestManager.Instance != null)
                            SideQuestManager.Instance.CompleteQuest("ConfrontWatcher");
                    }
                );
                break;


            default:
                Debug.Log("No interaction handler for id: " + id);
                break;
        }
    }
}
