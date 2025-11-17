using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
    public BladeSelector p1Selector;
    public BladeSelector p2Selector;

    public string nextSceneName = "BattleScene";

    public void OnReadyPressed()
    {
        // Save selected blade indices
        GameData.Instance.p1Index = p1Selector.GetIndex();
        GameData.Instance.p2Index = p2Selector.GetIndex();

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
