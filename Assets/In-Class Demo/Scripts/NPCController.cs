using UnityEngine;

public class NPCController : MonoBehaviour
{
    //Once we have our ScriptableObjects, we want to use them
    //We cannot directly drag a ScriptableObject onto a GameObject so we need an intermediary script
    //This script also gives us access back to the built in Unity funtions like Start and Update
    //and grabbing information from the gameobject this script is on

    //You also can have different intermediary script depending on what you want to use an NPC
    //statline on. Maybe you have an EnemyController, or special BossController scripts, but they all
    //have an NPC ScriptableObject attached to give that basic functionality
    public NPC npc;

    void Start()
    {
        //maybe load some stats into local variables here- stuff that we might want all
        //NPCs to set-up

        //for the sake of testing, we'll start off by calling our Attack function
        Attack();
    }


    //---Inheritance---
    //when we want to have variations to a single base, we might make classes that will inherit
    //from NPCController. Our NPCController inherits from MonoBehaviour, which is why it has 
    //access to things like Start and Update, when ScriptableObjects do not

    //when you make something inherit from THIS script, it will get anything established here
    //AND anything from MonoBehaviour!

    public virtual void Attack()  //a "virtual" function is a function that will have the same name,
                                  //but different behavior in each inherited class
                                  //in this case, we want ever NPC to "attack", when it is called
                                  //and what it does, might change based on the NPC
    {
        Debug.Log("This character doesn't attack.");
        //you can set a default behavior for NPCs, and OVERRIDE it for special characters
    }

    //when I go to edit this function in the inherited function, I will call it:
    //public override void Search()
}
