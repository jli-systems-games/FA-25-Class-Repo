using UnityEngine;

public class startballs : MonoBehaviour
{

    void Start()
    {
      
        int childCount = transform.childCount;

        // Pick random index
        int randomIndex = Random.Range(0, childCount);

        // Loop through children
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // Only enable the randomly chosen one
            child.gameObject.SetActive(i == randomIndex);
        }
    }

}
