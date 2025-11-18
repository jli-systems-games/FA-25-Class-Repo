using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 2f;
    public float maxHeight = 10f;
    public Slider progressSlider;
    public string nextScene = "HeistMain3D";

    private float currentHeight = 0f;
    private bool climbing = false;
    private bool pressLeft = true;

    void Update()
    {
       
        if (climbing)
        {
            
            if (pressLeft && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Step();
                pressLeft = false;
            }
            else if (!pressLeft && Input.GetKeyDown(KeyCode.RightArrow))
            {
                Step();
                pressLeft = true;
            }
        }

        
        if (currentHeight >= maxHeight)
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    void Step()
    {
        currentHeight += climbSpeed * Time.deltaTime * 20f;
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        float progress = currentHeight / maxHeight;
        progressSlider.value = progress;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            climbing = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            climbing = false;
        }
    }
}
