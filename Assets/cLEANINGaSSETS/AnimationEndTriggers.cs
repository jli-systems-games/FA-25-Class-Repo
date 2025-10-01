using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationEndTriggers : MonoBehaviour
{
    public GameObject canvas;
    public GameObject animationObj;
    public static int objectCount = 0;

    public void OnAnimationEnd()
    {
        canvas.SetActive(true);
        animationObj.SetActive(false);
        objectCount++;
        Debug.Log("Object count: " + objectCount);
        if (objectCount >= 3)
        {
            SceneManager.LoadScene("Motorcycle");
        }
    }
}
