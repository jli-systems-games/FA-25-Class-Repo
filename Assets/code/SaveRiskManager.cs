using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveRiskManager : MonoBehaviour
{
    [Header("风险")]
    public float checkInterval = 10f; 
    public int maxRisk = 90;           
    public int riskStep = 30;          

    private float timer = 0f;
    private bool saveDetected = false;
    private int currentRisk = 0;

    void Update()
    {
        timer += Time.deltaTime;


        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S))
        {
            saveDetected = true;
            currentRisk = 0; 
    
        }

      
        if (timer >= checkInterval)
        {
            CheckSaveStatus();
            timer = 0f; 
            saveDetected = false; 
        }
    }

    void CheckSaveStatus()
    {
        if (!saveDetected)
        {
    
            currentRisk = Mathf.Min(currentRisk + riskStep, maxRisk);



            float rand = Random.Range(0f, 100f);
            if (rand < currentRisk)
            {
   
                SceneManager.LoadScene("Save");
            }
        }
        
    }
}
