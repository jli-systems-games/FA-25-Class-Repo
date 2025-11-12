using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnFall : MonoBehaviour
{
    public float killY = -20f;   // 低于此高度判定掉落
    public float delay = 3f;     // 掉落后几秒重启
    bool scheduled;

    void Update()
    {
        // R 随时重开
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // 掉落判定
        if (!scheduled && transform.position.y < killY)
        {
            scheduled = true;
            // 播坠落声
            var sfx = GetComponent<PlayerSfx>();
            if (sfx) sfx.PlayFall();

            Invoke(nameof(Reload), delay);
        }
    }

    void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
