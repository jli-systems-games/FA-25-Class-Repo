using UnityEngine;
using MoreMountains.TopDownEngine;
using System.Collections;

public class CustomLevelSetup : MonoBehaviour
{
    public Transform SpawnPointP1;
    public Transform SpawnPointP2;
    public Transform SpawnPointP3;
    public Transform SpawnPointP4;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (var character in FindObjectsOfType<Character>())
        {
            if (character.PlayerID == "Player1" && SpawnPointP1 != null)
            {
                character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
                character.transform.position = SpawnPointP1.position;
            }
            else if (character.PlayerID == "Player2" && SpawnPointP2 != null)
            {
                character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
                character.transform.position = SpawnPointP2.position;
            }
            else if (character.PlayerID == "Player3" && SpawnPointP3 != null)
            {
                character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
                character.transform.position = SpawnPointP3.position;
            }
            else if (character.PlayerID == "Player4" && SpawnPointP4 != null)
            {
                character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
                character.transform.position = SpawnPointP4.position;
            }
        }
    }
}