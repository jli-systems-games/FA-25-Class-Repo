using UnityEngine;
using UnityEngine.SceneManagement;

public class ViolinCustomizer : MonoBehaviour
{
    public Renderer violinBody;
    public Material[] bodyMaterials;

    // For string audio selection
    public AudioClip[] stringAudioOptions;
    private int[] chosenStringSound = new int[4];

    public void SelectBodyColor(int index)
    {
        CentralData.current.bodyColorIndex = index;
        violinBody.material = bodyMaterials[index];
    }

    public void SelectStringSound(int stringIndex, int soundIndex)
    {
        chosenStringSound[stringIndex] = soundIndex;
        CentralData.current.stringSoundIndices[stringIndex] = soundIndex;
    }

    public void GoToPlayScene()
    {
        SceneManager.LoadScene("PlayScene");
    }
}
