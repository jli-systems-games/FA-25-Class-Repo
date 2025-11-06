using UnityEngine;
using UnityEngine.UI;

public class OpeningMAnager : MonoBehaviour
{
    public Button playFullButton;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        playFullButton.gameObject.SetActive(GameManager.Instance.isClipFullfilled);

        playFullButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.mixTrack.Play();
        });
    }
}
