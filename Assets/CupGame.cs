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

    void Start()
    {
        ball.transform.position = new Vector3(0, -3.5f, 0);
        ball.SetActive(true);

        foreach (var cup in cups)
            cup.transform.position = new Vector3(cup.transform.position.x, 0f, 0);

        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
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

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * GameManager.speedManager.GetSpeed();
                cups[a].transform.position = Vector3.Lerp(posA, posB, t);
                cups[b].transform.position = Vector3.Lerp(posB, posA, t);
                yield return null;
            }
        }

        canClick = true;
    }

    private void Update()
    {
        if (!canClick) return;

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

                    Vector3 targetPos = new Vector3(clicked.transform.position.x, 0f, 0f);
                    StartCoroutine(MoveTo(ball, new Vector3(clicked.transform.position.x, -3.5f, 0), 0f));
                    StartCoroutine(MoveTo(clicked, targetPos, 0.5f));
                }
                else
                {
                    GameManager.speedManager.IncreaseSpeed();
                    GameManager.liveManager.LoseLife();

                    Vector3 targetPos = new Vector3(clicked.transform.position.x, 0f, 0f);
                    StartCoroutine(MoveTo(clicked, targetPos, 0.5f));
                }

                canClick = false;
            }
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
