using System.Collections;
using UnityEngine;

public class PawGameControllerUI : MonoBehaviour
{
    public PawSlotUI[] pawSlots;
    public float timeLimit = 1.5f;
    public float nextDelay = 1.0f;
    public string letterPool = "ASDFJKL";
    public int correctHappy = +5;
    public int correctHunger = 0;
    public int correctHealth = 0;
    public int wrongHappy = -3;
    public int wrongHunger = 0;
    public int wrongHealth = 0;
    public GameObject failIndicator;
    public AudioClip successSound;
    public AudioClip failSound;
    PawSlotUI current;
    char currentLetter;
    bool waitingInput;

    void Start()
    {
        foreach (var p in pawSlots) if (p) p.Show(false);
        if (failIndicator) failIndicator.SetActive(false);
        StartCoroutine(MainLoop());
    }

    void Update()
    {
        if (!waitingInput) return;

        for (int i = 0; i < 26; i++)
        {
            if (Input.GetKeyDown(KeyCode.A + i))
            {
                var pressed = (char)('A' + i);
                if (pressed == 'B') return;
                
                if (pressed == currentLetter)
                {
                    ResolveSuccess();
                }
                else
                {
                    ResolveFail();
                }
                break;
            }
        }
    }

    IEnumerator MainLoop()
    {
        while (true)
        {
            current = RandomActivePaw();
            currentLetter = RandomLetter();
            current.SetLetter(currentLetter);
            current.Show(true);

            waitingInput = true;
            float t = 0f;
            while (waitingInput && t < timeLimit)
            {
                t += Time.deltaTime;
                yield return null;
            }

            if (waitingInput) ResolveFail();

            yield return new WaitForSeconds(nextDelay);
        }
    }

    void ResolveSuccess()
    {
        waitingInput = false;
        if (GameState.Instance != null)
            GameState.Instance.Add(correctHappy, correctHunger, correctHealth);

        if (successSound && Camera.main)
            AudioSource.PlayClipAtPoint(successSound, Camera.main.transform.position);

        if (current) current.Show(false);
        if (failIndicator) failIndicator.SetActive(false);
    }

    void ResolveFail()
    {
        waitingInput = false;
        if (GameState.Instance != null)
            GameState.Instance.Add(wrongHappy, wrongHunger, wrongHealth);

        if (failSound && Camera.main)
            AudioSource.PlayClipAtPoint(failSound, Camera.main.transform.position);

        if (current) current.Show(false);
        if (failIndicator)
        {
            failIndicator.SetActive(true);
            StartCoroutine(HideFailAfter(nextDelay));
        }
    }

    IEnumerator HideFailAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        if (failIndicator) failIndicator.SetActive(false);
    }

    PawSlotUI RandomActivePaw()
    {
        foreach (var p in pawSlots) if (p) p.Show(false);

        var list = new System.Collections.Generic.List<PawSlotUI>();
        foreach (var p in pawSlots) if (p) list.Add(p);
        int idx = Random.Range(0, list.Count);
        return list[idx];
    }

    char RandomLetter()
    {
        if (string.IsNullOrEmpty(letterPool)) return 'A';
        int i = Random.Range(0, letterPool.Length);
        return char.ToUpper(letterPool[i]);
    }
}