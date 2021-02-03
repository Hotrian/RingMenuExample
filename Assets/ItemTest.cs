using UnityEngine;
using UnityEngine.UI;

public class ItemTest : MonoBehaviour
{
    public Image Image;

    private int _selectedItem;
    // Start is called before the first frame update
    void Start()
    {
        RingMenu.Instance.OnItemSelected += (selected) =>
        {
            _selectedItem = selected;
            Image.sprite = RingMenu.Instance.Icons[_selectedItem];
            Debug.Log($"Selected Item: {_selectedItem}");
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (RingMenu.Instance.GetMenuIsClosed())
            {
                Debug.Log($"Using Item: {_selectedItem}");
            }
        }
    }
}
