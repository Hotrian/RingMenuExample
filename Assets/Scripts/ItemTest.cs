using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Demonstrates a simple way to connect with the Ring Menu.
/// </summary>
public class ItemTest : MonoBehaviour
{
    // The UI element.
    public Image Image;

    // The current item selected by the Ring Menu.
    // This only opens when the Ring Menu closes.
    private int _selectedItem;

    void Start()
    {
        // Register to be notified when the item selected by the Ring Menu has changed.
        RingMenu.Instance.OnItemSelected += (selected) =>
        {
            _selectedItem = selected;
            Image.sprite = RingMenu.Instance.Icons[_selectedItem];
            Debug.Log($"Selected Item: {_selectedItem}");
        };
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
}
