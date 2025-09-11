using UnityEngine;
using System.Collections;

public class question : MonoBehaviour
{
    public GameObject c;
    public GameObject C;
    public GameObject cc;
    public GameObject CC;
    public GameObject w;
    public GameObject W;
    public int Cs;
    public GameObject levelquestion;
    public GameObject nextlevel;
    public GameObject Dmanhope;
    public GameObject Dmansurp;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            c.SetActive(false);
            C.SetActive(true);
            cc.SetActive(true);
            Cs++;
        }

        if (Cs >= 2)
        {
            CC.SetActive(true);
            w.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            Dmanhope.SetActive(true);
            Dmansurp.SetActive(false);
            StartCoroutine(ActiveNew()); 
        }
    }

    IEnumerator ActiveNew()
    {
        if (W != null)
        {
            W.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        if (nextlevel != null)
        {
            levelquestion.SetActive(false);
            nextlevel.SetActive(true);
        }
    }
}
