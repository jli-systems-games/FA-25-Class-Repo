using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Slider slider;
    private float healthHurtAmount = 20f;

    public Canvas hurtCanvas;

    void Update()
    {
        slider.value -= Data.healthDecreaseAmount;

        Debug.Log(Data.healthDecreaseAmount);

        if (slider.value <= 1 )
        {
            SceneManager.LoadScene("Game Over Scene");
            Debug.Log("Lost Health");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Oil"))
        {
            Debug.Log("Hit Oil");

            slider.value -= healthHurtAmount;

            hurtCanvas.gameObject.SetActive(true);

            StartCoroutine(DelayBeforeRed(0.5f));
        }
    }

    IEnumerator DelayBeforeRed(float delay)
    {
        yield return new WaitForSeconds(delay);

        hurtCanvas.gameObject.SetActive(false);
    }
}
