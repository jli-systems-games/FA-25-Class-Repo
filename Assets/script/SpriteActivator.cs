using UnityEngine;
using System.Collections.Generic;

public class SpriteActivator_AdvancedPairs : MonoBehaviour
{
    [System.Serializable]
    public class ButtonSpritePair
    {
        public string buttonName;
        public GameObject targetSprite;
    }

    [System.Serializable]
    public class ConditionalBonus
    {
        public string buttonName;
        public GameObject bonusSprite;
    }

    [Header("所有按钮和按钮对应的贴图")]
    public List<ButtonSpritePair> spritePairs = new List<ButtonSpritePair>();

    [Header("特殊组合用于精灵的")]
    public string specialButton = "A";

    [Header("精灵的耳朵启动启动")]
    public List<ConditionalBonus> conditionalBonuses = new List<ConditionalBonus>();

    [Header("父级物体")]
    public List<GameObject> parentGroups = new List<GameObject>();

    void Start()
    {
  
        foreach (var pair in spritePairs)
        {
            if (pair.targetSprite != null)
                pair.targetSprite.SetActive(false);
        }

        foreach (var bonus in conditionalBonuses)
        {
            if (bonus.bonusSprite != null)
                bonus.bonusSprite.SetActive(false);
        }

        bool hasPressedA = Data.clickedButtons.Contains(specialButton);

        foreach (string clickedName in Data.clickedButtons)
        {

            foreach (var pair in spritePairs)
            {
                if (pair.buttonName == clickedName && pair.targetSprite != null)
                {
                    pair.targetSprite.SetActive(true);
                }
            }

    
            if (hasPressedA)
            {
                foreach (var bonus in conditionalBonuses)
                {
                    if (bonus.buttonName == clickedName && bonus.bonusSprite != null)
                    {
                        bonus.bonusSprite.SetActive(true);
                    }
                }
            }
        }

   
        foreach (GameObject parent in parentGroups)
        {
            if (parent == null) continue;

            bool anyChildActive = false;
            List<Transform> children = new List<Transform>();

       
            foreach (Transform child in parent.transform)
            {
                children.Add(child);
                if (child.gameObject.activeSelf)
                {
                    anyChildActive = true;
                }
            }

            if (!anyChildActive && children.Count > 0)
            {
                int randomIndex = Random.Range(0, children.Count);
                children[randomIndex].gameObject.SetActive(true);
 
            }
        }
    }
}
