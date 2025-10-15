using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Attributes : MonoBehaviour
{
    public TextMeshProUGUI courageText;
    public TextMeshProUGUI logicText;
    public TextMeshProUGUI empathyText;
    public TextMeshProUGUI techText;
    public TextMeshProUGUI remainingText;
    
    public Button couragePlusButton;
    public Button logicPlusButton;
    public Button empathyPlusButton;
    public Button techPlusButton;
    public Button resetButton;
    public Button startButton;
    
    void Start()
    {
        CoreData.Initialize();
        UpdateUI();
    }
    
    public void AddCourage()
    {
        if (CoreData.remaining > 0)
        {
            CoreData.courage++;
            CoreData.remaining--;
            UpdateUI();
        }
    }
    
    public void AddLogic()
    {
        if (CoreData.remaining > 0)
        {
            CoreData.logic++;
            CoreData.remaining--;
            UpdateUI();
        }
    }
    
    public void AddEmpathy()
    {
        if (CoreData.remaining > 0)
        {
            CoreData.empathy++;
            CoreData.remaining--;
            UpdateUI();
        }
    }
    
    public void AddTech()
    {
        if (CoreData.remaining > 0)
        {
            CoreData.tech++;
            CoreData.remaining--;
            UpdateUI();
        }
    }
    
    public void Reset()
    {
        CoreData.Reset();
        UpdateUI();
    }
    
    public void StartGame()
    {
        if (CoreData.remaining == 0)
        {
            SceneManager.LoadScene("MainGame");
        }
    }
    
    void UpdateUI()
    {
        courageText.text = CoreData.courage.ToString();
        logicText.text = CoreData.logic.ToString();
        empathyText.text = CoreData.empathy.ToString();
        techText.text = CoreData.tech.ToString();
        remainingText.text = CoreData.remaining.ToString();
        
        bool hasPoints = CoreData.remaining > 0;
        couragePlusButton.interactable = hasPoints;
        logicPlusButton.interactable = hasPoints;
        empathyPlusButton.interactable = hasPoints;
        techPlusButton.interactable = hasPoints;
        
        startButton.interactable = CoreData.remaining == 0;
    }
}