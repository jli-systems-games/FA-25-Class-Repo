using System.Collections;
using UnityEngine;

public class FirstPuzzleManager : MonoBehaviour
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

                StartCoroutine(animalPlatformCheck.PuzzleEnd("First Platform", Color.green));

                Data.firstPuzzleSolved = true;
                isSolved = true;
            }
     
        }
    }
}
