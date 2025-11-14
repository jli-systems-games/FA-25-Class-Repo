using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;

public class RouletteGame : MonoBehaviour
{
    public Character player;
    public Character npc;
    public Button fireButton;
    public GameObject fireButtonObj;


    public int chamberSize = 6;
    public int lethalBullets = 1;


    public PlayableDirector playerShootBlankTimeline;  
    public PlayableDirector playerShootHitTimeline;   


    public PlayableDirector npcShootBlankTimeline;  
    public PlayableDirector npcShootHitTimeline;   

    private List<bool> chamber;
    private int currentIndex = 0;
    private Character currentShooter;

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        chamber = new List<bool>();
        for (int i = 0; i < lethalBullets; i++)
            chamber.Add(true);
        for (int i = 0; i < chamberSize - lethalBullets; i++)
            chamber.Add(false);
        ShuffleChamber();

        currentShooter = npc;  
        fireButton.onClick.AddListener(OnFireButtonClicked);
    }

    void ShuffleChamber()
    {
        for (int i = chamber.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = chamber[i];
            chamber[i] = chamber[j];
            chamber[j] = temp;
        }
        currentIndex = 0;

        
    }

    void OnFireButtonClicked()
    {
        fireButtonObj.SetActive(false);
        StartCoroutine(FireSequence());
    }

    IEnumerator FireSequence()
    {

        Character shooter = currentShooter;
        Character target = (currentShooter == player) ? npc : player;

        if (currentIndex >= chamber.Count)
        {
            yield break;
        }

        bool isLethal = chamber[currentIndex];
        currentIndex++;

        PlayableDirector timeline = GetTimeline(shooter, isLethal);

        if (timeline == null)
        {
            yield return new WaitForSeconds(2f);
        }
        else
        {
            timeline.Play();
            yield return new WaitWhile(() => timeline.state == PlayState.Playing);
        }

        if (isLethal)
        {
            target.Die();  
            EndGame();
        }
        else
        {
  
            SwitchTurn();
            fireButtonObj.SetActive(true);
        }
    }

    PlayableDirector GetTimeline(Character shooter, bool isLethal)
    {
        if (shooter == player)
        {

            return isLethal ? playerShootHitTimeline : playerShootBlankTimeline;
        }
        else
        {
            return isLethal ? npcShootHitTimeline : npcShootBlankTimeline;
        }
    }

    void SwitchTurn()
    {
        currentShooter = (currentShooter == npc) ? player : npc;
    }

    void EndGame()
    {
        fireButtonObj.SetActive(false);

        Character winner = player.IsAlive ? player : npc;

}