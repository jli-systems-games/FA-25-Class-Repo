using System.Collections;
using UnityEngine;

public class NinthPuzzleManager : MonoBehaviour
{
    public AnimalPlatformCheck animalPlatformCheck;

    private bool isSolved = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isSolved)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                animalPlatformCheck.PlatformClickedAppearance(gameObject);

                StartCoroutine(animalPlatformCheck.PuzzleEnd("Ninth Platform", Color.green));

                Data.ninthPuzzleSolved = true;
                isSolved = true;
            }
     
        }
    }
}
