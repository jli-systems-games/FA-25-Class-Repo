using UnityEngine;
using System.Collections.Generic;

public class EatCakeMicrogame : MicrogameBase
{
    [Header("Setup")]
    public List<Collider2D> clickableItems;
    public string cakeTag = "Cake";

    protected override void OnBegin()
    {
        foreach (var c in clickableItems)
        {
            var go = c.gameObject;
            var listener = go.AddComponent<ItemClickListener>();
            listener.onClick = () =>
            {
                if (!running) return;
                bool isCake = go.CompareTag(cakeTag);
                Finish(isCake);
            };
        }
    }

    protected override bool CheckAutoComplete() => false;

    class ItemClickListener : MonoBehaviour
    {
        public System.Action onClick;
        void OnMouseDown() { onClick?.Invoke(); }
    }
}
