using UnityEngine;
using System.Collections;
public class PlayerEat : MonoBehaviour
{
    private Food nearbyFood; 
    private HungerSystem hungerSystem;
    public GameObject eattext;
    public GameObject foodc;
    public AudioSource eatsound;

    void Start()
    {
        hungerSystem = GetComponent<HungerSystem>();
        eattext.SetActive(false);
    }

    void Update()
    {
        if (nearbyFood != null && Input.GetKeyDown(KeyCode.E))
        {
            hungerSystem.Eat(nearbyFood);
            Destroy(nearbyFood.gameObject);
            nearbyFood = null;
            eatsound.Play();
            eattext.SetActive(false);
            StartCoroutine(ActivateRoutine());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Food"))
        {

            nearbyFood = other.GetComponent<Food>();
            Debug.Log("Press E to eat " + nearbyFood.type);
            eattext.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            if (other.GetComponent<Food>() == nearbyFood)
            {
                nearbyFood = null;
                Debug.Log("Left food area");
            }
            eattext.SetActive(false);
        }
    }

    private IEnumerator ActivateRoutine()
    {
        foodc.SetActive(true);
        yield return new WaitForSeconds(2f);
        foodc.SetActive(false);
    }
}
