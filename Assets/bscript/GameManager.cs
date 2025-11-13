using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool battleStarted = false;
    public TMP_Text resultText;
    public GameObject resultPanel;

    private void Awake()
    {
        Instance = this;
        resultPanel.SetActive(false);
    }
    public void StartBattle()
    {
        battleStarted = true;
        Debug.Log("Battle started!");
    }
    private void Update()
    {
        if (battleStarted)
        {
            CheckBattleState();
        }
    }

    void CheckBattleState()
    {
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
        GameObject[] enemyUnits = GameObject.FindGameObjectsWithTag("EnemyUnit");

       
        if (enemyUnits.Length == 0 && playerUnits.Length > 0)
        {
            BattleEnd(true);
        }
        else if (playerUnits.Length == 0 && enemyUnits.Length > 0)
        {
            BattleEnd(false);
        }
    }

    void BattleEnd(bool win)
    {
        battleStarted = false;
        resultPanel.SetActive(true);

        if (win)
            resultText.text = "You Win!!!";
        else
            resultText.text = "You Lose!!!";

        //StartCoroutine(ReturnToMenuAfterDelay());
    }

    IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSeconds(3f);
   
        // SceneManager.LoadScene("MainMenu");
    }
}
