using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string[] gameSceneNames;
    public GridPlacementSystem gridPlacementSystem;

    public void LoadRandomGame()
    {
        int randomIndex = Random.Range(0, gameSceneNames.Length);

        if (gridPlacementSystem != null)
        {
            gridPlacementSystem.paintMode = Random.Range(0, 4);
        }

        SceneManager.LoadScene(gameSceneNames[randomIndex]);
    }
}
