using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Edescal
{
    public class ItemFrame : SelectableButton
    {
        [Header("Item settings")]
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text itemName;
        private Item currentItem;

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
            if (currentItem != null)
            {
                itemName.text = currentItem.Name;
            }
            else
            {
                itemName.text = string.Empty;
            }

            base.OnSelect(eventData);
        }
    }

    public class AnimatorManager : MonoBehaviour
    {
        private Animator animator;


    }
}
