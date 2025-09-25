using UnityEngine;

public class interactions : MonoBehaviour
{
    public GameObject handObject;
    public GameObject itself;
    public Animator anime;
    public GameObject show;
    public GameObject texts;
    public GameObject text2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "hand")
        {
            texts.SetActive(true);
        if (Input.GetKeyDown(KeyCode.F)) { 
        
            show.SetActive(true);
            handObject.SetActive(false);
            texts.SetActive(false);
            itself.SetActive(false);
                text2.SetActive(false);
            }
        }
        //if (other.gameObject.tag == "fish")
        //{
        //    texts.SetActive(true);
        //    if (Input.GetKeyDown(KeyCode.F))
        //    {

        //        show.SetActive(true);
        //        handObject.SetActive(false);
        //        texts.SetActive(false);
        //        itself.SetActive(false);
        //        text2.SetActive(false);
        //    }
        //}
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
