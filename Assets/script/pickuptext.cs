using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickuptext : MonoBehaviour
{
    public GameObject pickuptexts;
    public GameObject groundObject;
    public GameObject inhand;
   
    // Start is called before the first frame update
    void Start()
    {
        pickuptexts.SetActive(false);
     
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            pickuptexts.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                groundObject.SetActive(false);
                pickuptexts.SetActive(false);
                inhand.SetActive(true);
            }
        }
      
    }

    private void OnTriggerExit(Collider other)
    {
        pickuptexts.SetActive(false);
    }
}
