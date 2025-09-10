using UnityEngine;
using System.Collections;
using TMPro;

public class MicrogameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI readyText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI feedbackText;

    [Header("Menus & Backgrounds")]
    public GameObject mainMenu; 
    public GameObject[] levelBackgrounds = new GameObject[3];

    readonly string[] order = { "EatCake", "CatchIceCream", "WhipCream" };

    int index = 0; 
    int successesInARow = 0; 
    int loopCount = 0;
    MicrogameBase current;
    Coroutine runner;
    bool inRun = false;

    void Start()
    {
        Debug.Log("Game started, mainMenu active=" + mainMenu.activeSelf);

        ShowMenu(true);
        feedbackText.gameObject.SetActive(false);
        timerText.text = "";
        ActivateBG(-1);
    }

    public void StartGame()
    {
        if (inRun) return;
        ShowMenu(false);
        index = 0;
        successesInARow = 0;
        loopCount = 0;
        runner = StartCoroutine(RunSequence());
    }

    void ShowMenu(bool show)
    {
        if (mainMenu) mainMenu.SetActive(show);
    }

    void ActivateBG(int levelIndex)
    {
        for (int i = 0; i < levelBackgrounds.Length; i++)
        {
            if (levelBackgrounds[i] == null) continue;
            levelBackgrounds[i].SetActive(i == levelIndex);
        }
    }

    IEnumerator RunSequence()
    {
        inRun = true;

        while (true)
        {
            string name = order[index];

            float timeLimit = Mathf.Max(1.2f, 3.5f - loopCount * 0.4f);

            ActivateBG(index);

            readyText.text = $"Ready: {name}";
            readyText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.9f);
            readyText.gameObject.SetActive(false);

            var prefab = Resources.Load<GameObject>($"Microgames/{name}");
            var go = Instantiate(prefab);
            current = go.GetComponent<MicrogameBase>();

            current.OnTick += (t) => { timerText.text = t.ToString("0.0"); };

            bool result = false;
            current.OnFinished += (ok) => { result = ok; };

            current.Begin(timeLimit);

            while (current != null && current.IsRunning) yield return null;

            feedbackText.text = result ? "Success!" : "Fail!";
            feedbackText.color = result ? Color.green : Color.red;
            feedbackText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.6f);
            feedbackText.gameObject.SetActive(false);
            timerText.text = "";

            if (go) Destroy(go);

            if (!result)
            {
                index = 0;
                successesInARow = 0;
                loopCount = 0;
                continue; 
            }
            else
            {
                successesInARow++;
                index++;
                if (index >= order.Length) { index = 0; loopCount++; }
                if (successesInARow >= 3)
                {
                    break;
                }
            }
        }

        ActivateBG(-1);
        ShowMenu(true);
        inRun = false;
        runner = null;
    }
}
