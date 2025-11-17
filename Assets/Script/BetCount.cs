using UnityEngine;

public class BetCounter : MonoBehaviour
{
    public static BetCounter I { get; private set; }

    public int correctGuesses = 0;
    public int wrongGuesses = 0;
    public int maxWrong = 3;
    public int maxCorrect = 10;

    public GameObject errorImagePrefab;
    public Transform[] errorImagePositions;

    private GameObject[] _errorImages;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        if (errorImagePrefab != null && errorImagePositions != null && errorImagePositions.Length >= maxWrong)
        {
            _errorImages = new GameObject[maxWrong];
        }
    }

    public void AddCorrect()
    {
        correctGuesses++;
        
        if (correctGuesses >= maxCorrect)
        {
            Win();
        }
    }

    public void AddWrong()
    {
        if (wrongGuesses >= maxWrong) return;

        wrongGuesses++;

        if (_errorImages != null && wrongGuesses <= maxWrong && errorImagePrefab != null)
        {
            int idx = wrongGuesses - 1;
            if (idx < errorImagePositions.Length && errorImagePositions[idx] != null)
            {
                _errorImages[idx] = Instantiate(errorImagePrefab, errorImagePositions[idx].position, 
                    errorImagePositions[idx].rotation, errorImagePositions[idx]);
            }
        }

        if (wrongGuesses >= maxWrong)
        {
            GameOver();
        }
    }

    void Win()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "WIN")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("WIN");
        }
    }

    void GameOver()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "WRONG")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("WRONG");
        }
    }

    public bool IsGameOver() => wrongGuesses >= maxWrong;
    public bool IsWin() => correctGuesses >= maxCorrect;
}