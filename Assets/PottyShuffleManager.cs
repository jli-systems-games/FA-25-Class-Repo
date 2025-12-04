using System.Collections;
using UnityEngine;

public class PottyShuffleManager : MonoBehaviour
{
    public PottyZone[] pottyZones;

    public Transform leftPos;
    public Transform middlePos;
    public Transform rightPos;

    public float spreadDuration = 0.3f;
    public float swapDuration = 0.3f;
    public int shuffleCount = 5;
    public float waitBetweenShuffles = 0.05f;

    public GameManager gameManager;

    bool isShuffling = false;

    public bool IsShuffling()
    {
        return isShuffling;
    }

    public void StartShuffle()
    {
        if (isShuffling) return;
        StartCoroutine(ShuffleRoutine());
    }

    IEnumerator ShuffleRoutine()
    {
        isShuffling = true;

        if (gameManager != null)
            gameManager.BlockShooting();

        var drops = GameObject.FindGameObjectsWithTag("Drop");
        foreach (var d in drops)
            Destroy(d);

        foreach (var p in pottyZones)
        {
            p.transform.position = middlePos.position;
            p.gameObject.SetActive(true);
            p.isReal = false;

            var col = p.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }

        int realIndex = Random.Range(0, pottyZones.Length);
        pottyZones[realIndex].isReal = true;

        Vector3[] targetPositions = new Vector3[3];
        targetPositions[0] = leftPos.position;
        targetPositions[1] = middlePos.position;
        targetPositions[2] = rightPos.position;

        yield return MoveAllToPositions(targetPositions, spreadDuration);

        for (int i = 0; i < shuffleCount; i++)
        {
            int a = Random.Range(0, pottyZones.Length);
            int b = a;

            while (b == a)
                b = Random.Range(0, pottyZones.Length);

            yield return SwapTwo(pottyZones[a].transform, pottyZones[b].transform, swapDuration);

            var temp = pottyZones[a];
            pottyZones[a] = pottyZones[b];
            pottyZones[b] = temp;

            yield return new WaitForSeconds(waitBetweenShuffles);
        }

        foreach (var p in pottyZones)
        {
            var col = p.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
        }

        if (gameManager != null)
            gameManager.AllowShooting();

        isShuffling = false;
    }

    IEnumerator MoveAllToPositions(Vector3[] targets, float duration)
    {
        float t = 0f;
        Vector3[] startPos = new Vector3[pottyZones.Length];

        for (int i = 0; i < pottyZones.Length; i++)
            startPos[i] = pottyZones[i].transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float eased = t * t * (3f - 2f * t); // smoothstep

            for (int i = 0; i < pottyZones.Length; i++)
            {
                pottyZones[i].transform.position =
                    Vector3.Lerp(startPos[i], targets[i], eased);
            }

            yield return null;
        }
    }

    IEnumerator SwapTwo(Transform a, Transform b, float duration)
    {
        float t = 0f;

        Vector3 startA = a.position;
        Vector3 startB = b.position;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float eased = t * t * (3f - 2f * t);

            a.position = Vector3.Lerp(startA, startB, eased);
            b.position = Vector3.Lerp(startB, startA, eased);

            yield return null;
        }
    }
}
