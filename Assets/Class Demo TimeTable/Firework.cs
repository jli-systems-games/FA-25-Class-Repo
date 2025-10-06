using UnityEngine;

public class Firework : MonoBehaviour
{
    public bool isHeart;
    public bool isBig;
    public bool isRed;
    public bool isSpiral;

    private GameObject heartFirework;
    private GameObject bigFirework;
    private GameObject redFirework;
    private GameObject spiralFirework;

    private AudioSource heartAudio;
    private AudioSource bigAudio;
    private AudioSource redAudio;
    private AudioSource spiralAudio;


    void Start()
    {
        if (isHeart)
        {
            Transform firstChildTransform = transform.GetChild(0);

            heartFirework = firstChildTransform.gameObject;
            heartAudio = GetComponent<AudioSource>();

        }
        else if (isBig)
        {
            Transform firstChildTransform = transform.GetChild(0);

            bigFirework = firstChildTransform.gameObject;
            bigAudio = GetComponent<AudioSource>();
        }
        else if (isRed)
        {
            Transform firstChildTransform = transform.GetChild(0);

            redFirework = firstChildTransform.gameObject;
            redAudio = GetComponent<AudioSource>();
        }
        else if (isSpiral)
        {
            Transform firstChildTransform = transform.GetChild(0);

            spiralFirework = firstChildTransform.gameObject;
            spiralAudio = GetComponent<AudioSource>();
        }

        FireWorkManager.startHeart += OnStartHeart;
        FireWorkManager.startBig += OnStartBig;
        FireWorkManager.startRed += OnStartRed;
        FireWorkManager.startSpiral += OnStartSpiral;
        FireWorkManager.end += EndFireworks;
    }

    void OnStartHeart()
    {
        PlayFireWork(heartFirework, heartAudio);
    }

    void OnStartBig()
    {
        PlayFireWork(bigFirework, bigAudio);
    }

    void OnStartRed()
    {
        PlayFireWork(redFirework, redAudio);
    }

    void OnStartSpiral()
    {
        PlayFireWork(spiralFirework, spiralAudio);
    }

    void EndFireworks()
    {
        FireWorkManager.startHeart -= OnStartHeart;
        FireWorkManager.startBig -= OnStartBig;
        FireWorkManager.startRed -= OnStartRed;
        FireWorkManager.startSpiral -= OnStartSpiral;
    }

    void PlayFireWork(GameObject firework, AudioSource audio)
    {
        firework.SetActive(true);
        audio.Play();
    }
}
