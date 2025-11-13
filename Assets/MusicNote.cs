using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicNote : MonoBehaviour
{
    public int trackIndex;
    public float fadeDuration = 1f;

    public SoundManager sm;
    bool playerInside = false;

    void Start()
    {
        sm = Object.FindAnyObjectByType<SoundManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
        UIInstructionManager.Instance.Show();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
        UIInstructionManager.Instance.Hide();

    }

    void Update()
    {
        if (playerInside && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(sm.FadeTrackTo1(trackIndex, fadeDuration));
            SCoreMAnager.Instance.AddScore();
            StartCoroutine(WaitForSecondsAndDestroy(1f));
        }
    }

    System.Collections.IEnumerator WaitForSecondsAndDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
