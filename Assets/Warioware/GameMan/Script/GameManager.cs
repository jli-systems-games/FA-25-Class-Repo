using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    // 진행할 미니게임 씬 이름(순서대로)
    [SerializeField] List<string> miniScenes = new() { "Mini01", "Mini02", "Mini03", "Mini04" };
    [SerializeField] string startScene = "Start";
    [SerializeField] string winScene = "Win";
    [SerializeField] string failScene = "Fail";

    int idx = -1;        // 현재 인덱스(-1 = 아직 시작 전)
    bool locked = false; // 중복 전환 방지

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);
    }

    // 시작 화면의 Start 버튼에서 호출
    public void StartRun()
    {
        idx = 0;
        Load(miniScenes[idx]);
    }

    // 각 미니게임에서 "성공" 시 호출
    public void MicroSuccess()
    {
        if (locked) return;
        locked = true;

        idx++;
        if (idx >= miniScenes.Count)
        {
            Load(winScene); // 모두 성공 = 승리
        }
        else
        {
            Load(miniScenes[idx]); // 다음 미니게임
        }
    }

    // 각 미니게임에서 "실패" 시 호출
    public void MicroFail()
    {
        if (locked) return;
        locked = true;
        Load(failScene); // 공통 실패 화면
    }

    // 실패/승리 화면에서 “다시하기” 버튼
    public void BackToStart()
    {
        idx = -1;
        Load(startScene);
    }

    void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        locked = false;
    }
}
