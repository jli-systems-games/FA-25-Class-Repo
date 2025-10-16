using UnityEngine;
using UnityEngine.UI;

public class RecordButton : MonoBehaviour
{
    [Header("需要监听这个小按钮喵")]
    public GameObject targetObject; 

    private Text targetText; 

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);

  
        if (targetObject != null)
        {
            targetText = targetObject.GetComponent<Text>();
        }
    }

    void OnClicked()
    {
    
        Data.clickedButtons.Add(gameObject.name);
  


        if (targetText != null)
        {
            string colorCode = targetText.text;
            Data.colorRecords[gameObject.name] = colorCode;

        }
    }
}
