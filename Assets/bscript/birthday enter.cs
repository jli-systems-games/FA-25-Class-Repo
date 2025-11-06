using UnityEngine;
using UnityEngine.SceneManagement;
public class birthdayenter : MonoBehaviour
{
    public string sceneName;
    public GameObject text;
    private void OnTriggerStay(Collider other)
    {
        text.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (other.CompareTag("Player"))
            {

                SceneManager.LoadScene(sceneName);
            }
        }
       
    }
    private void OnTriggerExit(Collider other)
    {
        text.SetActive(false);
    }
}
