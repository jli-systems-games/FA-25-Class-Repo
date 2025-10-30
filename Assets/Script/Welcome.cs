using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Welcome : MonoBehaviour
{
    public string nextSceneName;
    public AudioClip transitionSound;
    
    private AudioSource audioSource;
    private bool triggered = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (!triggered && Input.GetKeyDown(KeyCode.Space))
        {
            triggered = true;
            StartCoroutine(TransitionWithDelay());
        }
    }

    private System.Collections.IEnumerator TransitionWithDelay()
    {
        if (transitionSound) audioSource.PlayOneShot(transitionSound);
        yield return new WaitForSeconds(1f);
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}