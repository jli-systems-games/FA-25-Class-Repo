using UnityEngine;
using System.Collections;
using TMPro;

public class MicrogameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI readyText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI feedbackText;

    readonly string[] order = { "EatCake", "CatchIceCream", "WhipCream" };

    int index = 0;
    int loopCount = 0;
    MicrogameBase current;

    void Start()
    {
        feedbackText.gameObject.SetActive(false);
        timerText.text = "";
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        while (true)
        {
            string name = order[index];
            float timeLimit = Mathf.Max(1.2f, 3.5f - loopCount * 0.4f);

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

            while (current != null && current.IsRunning)
                yield return null;

            feedbackText.text = result ? "Success!" : "Fail!";
            feedbackText.color = result ? Color.green : Color.red;
            feedbackText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.6f);
            feedbackText.gameObject.SetActive(false);
            timerText.text = "";

            if (go) Destroy(go);

            index++;
            if (index >= order.Length) { index = 0; loopCount++; }
        }
    }
}
