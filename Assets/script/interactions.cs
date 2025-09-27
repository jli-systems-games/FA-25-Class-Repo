using UnityEngine;

public class interactions : MonoBehaviour
{
    public GameObject handObject;
    public GameObject itself;
    public GameObject showup;
    public GameObject show;
    public GameObject texts;
    public GameObject text2;
    public GameObject noshowup;
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

            text2.SetActive(false);
            show.SetActive(true);
            handObject.SetActive(false);
            texts.SetActive(false);
            itself.SetActive(false);
              showup.SetActive(true);
                noshowup.SetActive(false);

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
    private void OnTriggerExit(Collider other)
    {
        texts.SetActive(false);
    }
    void Update()
    {
        
    }
}
