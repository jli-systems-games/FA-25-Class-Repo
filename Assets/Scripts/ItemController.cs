using UnityEngine;
using TMPro;
using System.Collections;

public class ItemController : MonoBehaviour
{
    public GameManager manager;
    public TMP_Text movementTextUI;

    private string movementPattern;
    private bool isActivated = false;
    private float timer = 7f;
    public int patternLength = 4;
    public AudioClip[] keyPressedSFX;
    public AudioSource audioSource;
    public AudioClip succeedSFX;
    public AudioClip failedSFX;


    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GenerateMovementPattern();
        StartCoroutine(SelfDestruct());
    }


    private void GenerateMovementPattern()
    {
        char[] directions = new char[] { '<', '>', '^', '_' };
        movementPattern = "";

        for (int i = 0; i < patternLength; i++)
        {
            movementPattern += directions[Random.Range(0, directions.Length)];
        }
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(timer);

        if (movementTextUI != null)
            movementTextUI.text = "";

        audioSource.PlayOneShot(failedSFX);
        yield return new WaitForSeconds(failedSFX.length);
        Destroy(gameObject);
        manager.SpawnNextItem();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            if (movementTextUI != null)
            {
                movementTextUI.text = movementPattern;
                RectTransform rect = movementTextUI.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    Random.Range(-290f, 290f),
                    Random.Range(-190f, 190f)
                );

            }

            StartCoroutine(WaitForInput());
        }
    }

    private IEnumerator WaitForInput()
    {
        foreach (char dir in movementPattern)
        {
            bool correct = false;
            while (!correct)
            {
                if (dir == '<' && Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    audioSource.PlayOneShot(keyPressedSFX[0]);
                    correct = true;
                }
                else if (dir == '>' && Input.GetKeyDown(KeyCode.RightArrow))
                {
                    audioSource.PlayOneShot(keyPressedSFX[1]);
                    correct = true;
                }
                else if (dir == '^' && Input.GetKeyDown(KeyCode.UpArrow))
                {
                    audioSource.PlayOneShot(keyPressedSFX[2]);
                    correct = true;
                }
                else if (dir == '_' && Input.GetKeyDown(KeyCode.DownArrow))
                {
                    audioSource.PlayOneShot(keyPressedSFX[3]);
                    correct = true;
                }
                yield return null;
            }
        }

        if (movementTextUI != null)
            movementTextUI.text = "";

        audioSource.PlayOneShot(succeedSFX);
        yield return new WaitForSeconds(succeedSFX.length);
        manager.AddScore();
        Destroy(gameObject);
        manager.SpawnNextItem();
    }
}
