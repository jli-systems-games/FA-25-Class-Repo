using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogM : MonoBehaviour
{
    public Image actorImage;
    public Text actName;
    public Text messagetext;
    public RectTransform backgroundBox;
    Message[] currentMessages;
    Actor[] currentActors;
    int activeMessage = 0;
    public static bool isActive = false;

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        isActive = true;
        DisplayMessage();
        Debug.Log("Started conversation! Loaded messages: " + messages.Length);
        
        backgroundBox.LeanScale(new Vector3(6.375f, 1f, 1f), 0.5f).setEaseInOutExpo();
    }

    void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage];
        messagetext.text = messageToDisplay.message;

        Actor actorToDisplay = currentActors[messageToDisplay.actorId];
        actName.text = actorToDisplay.name;
        actorImage.sprite = actorToDisplay.sprite;
    }

   public void NextMessage()
    {
        activeMessage++;
        if(activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            Debug.Log("conversation ended!");
            backgroundBox.LeanScale(Vector3.zero,0.5f).setEaseInOutExpo();
            isActive = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        backgroundBox.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.anyKeyDown||Input.GetMouseButtonDown(0) && isActive == true)
        {
            NextMessage();
        }
    }
}
