using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Edescal
{
    public class ItemFrame : SelectableButton
    {
        private Item currentItem;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private Image itemImage;

        public void SetItem(Item item)
        {
            currentItem = item;
            if(currentItem == null)
            {
                itemImage.sprite = null;
                itemImage.enabled = false;
                return;
            }

            itemImage.sprite = currentItem.Sprite;
            itemImage.enabled = true;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            inventoryUI?.SetSelectedItem(currentItem);
            base.OnSelect(eventData);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            inventoryUI?.SendItem(currentItem);
            base.OnSubmit(eventData);
        }
    }
}
