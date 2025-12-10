using UnityEngine;
using System.Collections;
using TMPro;

public class WorldItemPickup : MonoBehaviour
{
    public ItemData itemData;

    public string requiredToolName = "";

    public TMP_Text toolHintText;

    public AudioClip pickUpSound;
    public AudioSource audioSource;

    public string hintMessage = "A  tool  is  required  to  pick  up  this  item";

    public void Pickup()
    {
        if (InventoryManager.Instance == null || itemData == null)
        {
            Debug.LogError("InventoryManager 또는 ItemData가 설정되지 않았습니다.");
            return;
        }

        bool requiresTool = !string.IsNullOrEmpty(requiredToolName);

        bool canPickUp = !requiresTool ||
                      (requiresTool && InventoryManager.Instance.HasItem(requiredToolName));
        if (canPickUp)
        {
            InventoryManager.Instance.AddItem(itemData);
            Destroy(gameObject);
        }
        else
        {
            if (toolHintText != null)
            {
                StartCoroutine(ShowHintTemporarily(hintMessage));
            }
        }

        PlayPickUpSound();
    }

    IEnumerator ShowHintTemporarily(string message)
    {
        if (toolHintText.gameObject.activeSelf)
        {

        }

        toolHintText.text = message;
        toolHintText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        toolHintText.gameObject.SetActive(false);

    }

    void PlayPickUpSound()
    {
        if (pickUpSound == null) return;

        if (audioSource != null)
            audioSource.PlayOneShot(pickUpSound);
        else
            AudioSource.PlayClipAtPoint(pickUpSound, transform.position);
    }
}