using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

public class CupGame : MonoBehaviour
{
    public GameObject[] cups;
    public GameObject ball;
    private bool canClick = false;

    public void StartCupGame(float gameSpeed)
    {
        StartCoroutine(GameFlow(gameSpeed));
    }

    IEnumerator GameFlow(float speed)
    {
        ball.transform.position = new Vector3(0, -3.5f, 0);
        ball.SetActive(true);

        foreach (var cup in cups)
            cup.transform.position = new Vector3(cup.transform.position.x, 0f, 0);

        yield return new WaitForSeconds(0.5f);

        foreach (var cup in cups)
            StartCoroutine(MoveTo(cup, new Vector3(cup.transform.position.x, -3.5f, 0), 0.5f));

        yield return new WaitForSeconds(0.5f);

        ball.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            int a = Random.Range(0, 3);
            int b = Random.Range(0, 3);
            while (b == a) b = Random.Range(0, 3);

            Vector3 posA = cups[a].transform.position;
            Vector3 posB = cups[b].transform.position;

            float timeSwapping = 0;
            while (timeSwapping < 1f)
            {
                timeSwapping += Time.deltaTime / speed;
                cups[a].transform.position = Vector3.Lerp(posA, posB, timeSwapping);
                cups[b].transform.position = Vector3.Lerp(posB, posA, timeSwapping);
                yield return null;
            }
        }

        canClick = true;

        while (canClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject clicked = hit.collider.gameObject;

                    if (clicked.name == "1")
                    {
                        ball.SetActive(true);
                        GameManager.speedManager.IncreaseSpeed();

                        StartCoroutine(MoveTo(clicked, new Vector3(clicked.transform.position.x, 0f, 0f), 0.5f));
                        StartCoroutine(MoveTo(ball, new Vector3(clicked.transform.position.x, -3.5f, 0), 0f));
                    }
                    else
                    {
                        GameManager.speedManager.IncreaseSpeed();
                        GameManager.liveManager.LoseLife();

                        StartCoroutine(MoveTo(clicked, new Vector3(clicked.transform.position.x, 0f, 0f), 0.5f));
                    }

                    canClick = false;
                }
            }
            yield return null;
        }
    }

    IEnumerator MoveTo(GameObject obj, Vector3 target, float time)
    {
        Vector3 start = obj.transform.position;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / time;
            obj.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
    }
}
