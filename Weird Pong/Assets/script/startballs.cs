using UnityEngine;

public class startballs : MonoBehaviour
{

    void Start()
    {
      
        int childCount = transform.childCount;

    
        int randomIndex = Random.Range(0, childCount);

        
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

          
            child.gameObject.SetActive(i == randomIndex);
        }
    }

}
