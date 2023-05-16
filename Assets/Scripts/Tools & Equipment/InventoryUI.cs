using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Edescal;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class InventoryUI : MonoBehaviour
{
    public bool open = false;
    public bool inTransition = false;
    public CanvasGroup canvasGroup;
    public Button firstSelected;
    public GameObject inventory;
    public ItemFrame[] itemFrames;
    [Space(10)]
    public CanvasGroup currentItemCanvas;
    public Image currentItemSprite;
    public TMP_Text currentItemName;
    public TMP_Text currentItemDescription;

    [Header("Sound FX")]
    public AudioClip openInventorySound;
    public AudioClip closeInventorySound;
    public AudioSource audioSource;

    public InputActionReference openInventory;
    public InputActionReference submit;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canvasGroup.alpha = 0;
        //inventory.SetActive(false);
        SetSelectedItem(null);
        SendItem(null);
    }

    private void OnEnable()
    {
        if (openInventory != null)
        {
            openInventory.action.performed += HandleInput;
            openInventory.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (openInventory != null)
        {
            openInventory.action.performed -= HandleInput;
            openInventory.action.Disable();
        }
    }

    public void HandleInput(InputAction.CallbackContext ctx) => HandleInput();
    public void HandleInput()
    {
        if (inTransition) return;

        open = !open;
        if (open)
        {
            OpenInventory();
            return;
        }
        CloseInventory();
    }

    private void OpenInventory()
    {
        if (inTransition) return;

        inTransition = true;
        audioSource.PlayOneShot(openInventorySound);

        IEnumerator OnOpen()
        {
            firstSelected.Select();
            firstSelected.OnSelect(null);
            //inventory.SetActive(true);
            float counter = 0;
            while(counter < 0.15f)
            {
                counter += Time.unscaledDeltaTime;
                float t = counter / 0.15f;
                if(t > 1)
                {
                    t = 1;
                }
                t = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
                canvasGroup.alpha = Mathf.Lerp(0, 1, t);
                yield return null;
            }
            canvasGroup.alpha = 1;
            inTransition = false;
        }
        StopAllCoroutines();
        StartCoroutine(OnOpen());
    }

    private void CloseInventory()
    {
        if (inTransition) return;

        inTransition = true;
        audioSource.PlayOneShot(closeInventorySound);

        IEnumerator OnClose()
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            float counter = 0;
            while (counter < 0.15f)
            {
                counter += Time.unscaledDeltaTime;
                float t = counter / 0.15f;
                if (t > 1)
                {
                    t = 1;
                }
                t = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
                canvasGroup.alpha = Mathf.Lerp(1, 0, t);
                yield return null;
            }
            canvasGroup.alpha = 0;
            //inventory.SetActive(false);
            inTransition = false;
        }
        StopAllCoroutines();
        StartCoroutine(OnClose());
    }

    public void UpdateView(List<Item> items)
    {
        for (int i = 0; i < itemFrames.Length; i++)
        {
            var itemFrame = itemFrames[i];
            if (i < items.Count)
            {
                var item = items[i];
                itemFrame.SetItem(item);
            }
            else
            {
                itemFrame.SetItem(null);
            }
        }
    }

    public void SetSelectedItem(Item item)
    {
        if (currentItemSprite == null || currentItemName == null || currentItemDescription == null) return;

        if (item == null)
        {
            currentItemSprite.enabled = false;
            currentItemName.text = string.Empty;
            currentItemDescription.text = string.Empty;
            currentItemCanvas.alpha = 0;
            return;
        }
        currentItemCanvas.alpha = 1;
        currentItemSprite.sprite = item.Sprite;
        currentItemSprite.enabled = true;
        currentItemName.text = item.Name;
        currentItemDescription.text = $"Descripción del objeto {item.Name}";
    }

    public void SendItem(Item item)
    {
        if (item == null) return;

        print($"Sended {item.Name} to listener...");
    }
}