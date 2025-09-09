using UnityEngine;


public class NPC_Cypsel : NPCController //here, you replace MonoBehaviour with NPCController
{
    //start was already set up in NPCController, so unless we want to override it,
    //there is no reason to have it here

    //and even though our Attack is unique to John, we can still call Attack() in NPCController
    //without special code here!

    public override void Attack() //Now, I will add unique behaviour to John in this Attack function
    {
        Flash();
        TornadoChase();
    }

    //John might also have a special ability no other character has
    //and you can add that here. No need to add anything to NPCController!

    public void Flash() //John is special and has a bat. No other NPC has this behavior
    {
        Debug.Log("Cypsel flashed.");
    }

    public void TornadoChase() //John is special and has a bat. No other NPC has this behavior
    {
        Debug.Log("Cypsel .");
    }
}
