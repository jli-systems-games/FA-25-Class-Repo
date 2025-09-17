using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    public Animator spider;
    public string idleTrigger = "IsItchy";
    public string eatTrigger = "IsEating";

    public GameObject[] bugPrefabs;
    public AudioSource bugAudioManager;
    public AudioClip bugAte;
    public float spawnMinTime = 20f;
    public float spawnMaxTime = 60f;

    private float timer;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnBug();
            ResetTimer();
        }
    }

    void SpawnBug()
    {
        Vector2 pos = new Vector2(Random.Range(-7f, 7f), Random.Range(-4.5f, 4.5f));
        int index = Random.Range(0, bugPrefabs.Length);
        Instantiate(bugPrefabs[index], pos, Quaternion.identity);
    }

    void ResetTimer()
    {
        timer = Random.Range(spawnMinTime, spawnMaxTime);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Respawn") && Input.GetKeyDown(KeyCode.Space))
        {
            bugAudioManager.PlayOneShot(bugAte);
            spider.SetTrigger(eatTrigger);
            Destroy(col.gameObject);
        }
    }

    void OnMouseDown()
    {
        spider.SetTrigger(idleTrigger);
    }
}
