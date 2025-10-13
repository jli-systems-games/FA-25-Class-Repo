using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] passengers;
    public GameObject[] passengersInLine;
    public GameObject normalEnding;
    public GameObject excuseText;
    public GameObject nextScenebutton;
    public float stillTime = 2f;

    private List<int> order = new List<int>();
    private int currentIndex = -1;
    private int seatCount = 0;
    private int shownCount = 0;

    void Start()
    {
        for (int i = 0; i < passengers.Length; i++) order.Add(i);
        Shuffle(order);

        foreach (var c in passengers) c.SetActive(false);
        foreach (var a in passengersInLine) a.SetActive(false);

        ShowNextCharacter();
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    void ShowNextCharacter()
    {
        if (shownCount >= order.Count) return;

        int nextIndex = order[shownCount];
        passengersInLine[nextIndex].SetActive(true);
    }

    public void OnCharacterClicked(int index)
    {
        currentIndex = index;

        for (int i = 0; i < passengersInLine.Length; i++)
            passengersInLine[i].SetActive(i == index);
    }

    public void Choose(GameObject passenger, GameObject passengerInLine, bool giveSeat)
    {
        passengerInLine.SetActive(false);

        passenger.SetActive(giveSeat);

        if (giveSeat) seatCount++;


        currentIndex = -1;
        shownCount++;

        if (shownCount >= order.Count-1)
        {
            if (seatCount >= 5)
            {
                StartCoroutine(ShowLastPassengerRoutine());
            }
            else
            {
                StartCoroutine(NormalEndingRoutine());
            }
            return;
        }

        StartCoroutine(WaitAndShowNext(stillTime));
    }

    private IEnumerator ShowLastPassengerRoutine()
    {
        yield return new WaitForSeconds(2f);
        passengersInLine[shownCount].SetActive(true);
        excuseText.SetActive(true);

        yield return new WaitForSeconds(5f);
        passengersInLine[shownCount].SetActive(false);

        foreach (var c in passengers)
        {
            Button btn = c.GetComponent<Button>();
            btn.interactable = true;
        }
    }
    private IEnumerator WaitAndShowNext(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowNextCharacter();
    }

    private IEnumerator NormalEndingRoutine()
    {
        yield return new WaitForSeconds(2f);
        normalEnding.SetActive(true);
        yield return new WaitForSeconds(1f);
        nextScenebutton.SetActive(true);
    }
}
