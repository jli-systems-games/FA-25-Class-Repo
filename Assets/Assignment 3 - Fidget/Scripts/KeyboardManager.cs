using System.Security.Cryptography;
using UnityEngine;
using System.Collections;
using static UnityEditor.PlayerSettings;

public class KeyboardManager : MonoBehaviour
{
    public Material keyboardWhite;
    public Material keyboardBlack;
    public AudioSource audioSource;
    public AudioClip keyAudio;

    private Vector3 OGpos;
    private float keyDownAmount = 0.0024f;
    private bool isDown = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        int randomColor = Random.Range(0, 4);

        if (randomColor == 0)
        {
            gameObject.GetComponent<Renderer>().material = keyboardBlack;
            gameObject.tag = "Black";
        }
        else
        {
            gameObject.GetComponent<Renderer>().material = keyboardWhite;
            gameObject.tag = "Untagged";
        }

        OGpos = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDown)
        {
            Vector3 newPos = transform.localPosition;
            newPos.z -= keyDownAmount;
            transform.localPosition = newPos;

            isDown = true;

            audioSource.PlayOneShot(keyAudio);

            Debug.Log("Player hit keyboard");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DelayBeforeReturningKey(0.5f));
        }
    }

    private IEnumerator DelayBeforeReturningKey(float delay)
    {
        yield return new WaitForSeconds(delay);

        transform.localPosition = OGpos;

        isDown = false;

        Debug.Log("Player left keyboard");
    }
}
