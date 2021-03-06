﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Demonstrates a simple way to connect with the Ring Menu.
/// </summary>
public class ItemTest : MonoBehaviour
{
    // The UI element.
    public Image Image;

    // The current item selected by the Ring Menu.
    // This only updates when the Ring Menu closes.
    private int _selectedItem;

    void Start()
    {
        // Register to be notified when the item selected by the Ring Menu has changed.
        RingMenu.Instance.OnItemSelected += UpdateSelection;

        // Update the selected item manually in case we load after the RingMenu has fired the initial event.
        UpdateSelection(RingMenu.Instance.GetCurrentSelection());
    }

    void Update()
    {
        // Demonstrate a basic way to "use" an item selected by the Ring Menu, when the Menu is closed.
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (RingMenu.Instance.GetMenuIsClosed())
            {
                Debug.Log($"Using Item: {_selectedItem}");
            }
        }
    }

    private void UpdateSelection(int selected)
    {
        _selectedItem = selected;
        Image.sprite = RingMenu.Instance.Icons[_selectedItem];
        Debug.Log($"Selected Item: {_selectedItem}");
    }
}
