using UnityEngine;

public class FireworkHeart : MonoBehaviour
{
    public ParticleSystem firework;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FireworkController.onFireHearts += PlayFirework;
        firework.Stop();
        var main = firework.main;

        main.loop = false;
    }

    // Update is called once per frame
    void PlayFirework()
    {
        firework.Play();
    }
}
