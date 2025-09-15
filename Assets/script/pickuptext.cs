using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickuptext : MonoBehaviour
{
    public GameObject talk;
    public GameObject talkbefore;
   
    // Start is called before the first frame update
    void Start()
    {
        talk.SetActive(false);
     
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            talk.SetActive(true);
            talkbefore.SetActive(false);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        talk.SetActive(false);
    }
}
