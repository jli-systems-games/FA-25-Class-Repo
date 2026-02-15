using UnityEngine;
using System.Collections;
public class HandKeys : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject keyboardup;
    public GameObject keyboarddown;
    public GameObject screen1;
    public GameObject screen2;
    public GameObject screen3;
    public GameObject screen4;
    public GameObject worklevel;
    public GameObject nightlevel;
    public int working;
    private KeyCode[] leftKeys = {
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T,
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B,
        KeyCode.Tab, KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.CapsLock
    };

    
    private KeyCode[] rightKeys = {
        KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
        KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.N, KeyCode.M,
        KeyCode.RightShift, KeyCode.RightControl, KeyCode.Return, KeyCode.Backspace
    };

    void Update()
    {


        foreach (KeyCode key in leftKeys)
        {
            if (Input.GetKeyDown(key))
            {
                ShowLeftHand();
             
            }
        }

     
        foreach (KeyCode key in rightKeys)
        {
            if (Input.GetKeyDown(key))
            {
                ShowRightHand();
              
            }
        }

        if (Input.anyKeyDown)
        {
            working++;
            keyboarddown.SetActive(true);
            keyboardup.SetActive(false);
        }
        else
        {
            keyboardup.SetActive(true);
            keyboarddown.SetActive(false);
        }

        if (working == 100)
        {
            worklevel.SetActive(false);
            nightlevel.SetActive(true);
        }
        else if (working == 25)
        {
            screen2.SetActive(true);
        }
        else if (working == 50)
        {
            screen3.SetActive(true);
        }
        else if (working == 90)
        {
            screen4.SetActive(true);
        }
        else
        {
            screen1.SetActive(true);
        }
    }
    void ShowLeftHand()
    {
        if (leftHand != null) leftHand.SetActive(true);
        if (rightHand != null) rightHand.SetActive(false);
    
   
    }

    void ShowRightHand()
    {
        if (rightHand != null) rightHand.SetActive(true);
        if (leftHand != null) leftHand.SetActive(false);
     
    }

}
