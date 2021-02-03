using System.Collections;
using UnityEngine;
using Image = UnityEngine.UI.Image;

/// <summary>
/// A simple Ring Menu, which can be opened and closed with Tab, and navigated with Q and E respectively.
/// </summary>
public class RingMenu : MonoBehaviour
{
    public static RingMenu Instance;

    #region Properties and Fields
    // Unity Properties
    public GameObject Anchor;
    public GameObject SelectorPrefab;
    public GameObject RingPrefab;
    public Sprite[] Icons;
    public float Distance = 50f;
    public float OpenCloseTime = 0.5f;
    public int OpenCLoseSpins = 2;
    public float SelectTime = 0.25f;

    // Private Fields
    private Image _selector;
    private RingItem[] _slots;

    private float _ringAngleOffset = Mathf.Deg2Rad * 90f;
    private float _ringAngleDiff;

    private bool _ringOpenCloseAnimation;
    private bool _ringOpen;
    private bool _selectorMoving;
    private int _ringSelected;

    // Public Events
    public ItemSelectedDelegate OnItemSelected;
    public delegate void ItemSelectedDelegate(int item);
    #endregion

    #region Unity Methods
    public void Awake()
    {
        if (Instance != null)
            Destroy(this);

        Instance = this;
    }

    public void Start()
    {
        // Create the slots and the selector from prefabs
        _slots = new RingItem[Icons.Length];
        _ringAngleDiff = Mathf.Deg2Rad * (360f / Icons.Length);
        for (var i = 0; i < Icons.Length; i++)
        {
            _slots[i] = Instantiate(RingPrefab, Anchor.transform).GetComponent<RingItem>();
            _slots[i].ItemSprite.sprite = Icons[i];
        }
        _selector = Instantiate(SelectorPrefab, Anchor.transform).GetComponent<Image>();

        // Set the default state of the slots and the selector
        SetRingActive(false);
        SetRingAlpha(0f);

        OnItemSelected?.Invoke(_ringSelected);
    }

    public void Update()
    {
        if (!_ringOpenCloseAnimation && !_selectorMoving)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                    _ringOpenCloseAnimation = true;
                    _ringOpen = !_ringOpen;
                    StartCoroutine(_ringOpen ? OpenRing() : CloseRing());
            }
            else if (_ringOpen)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    _selectorMoving = true;
                    _ringSelected--;
                    StartCoroutine(MoveSelector(_ringSelected + 1, _ringSelected));
                    if (_ringSelected < 0)
                        _ringSelected = Icons.Length - 1;
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    _selectorMoving = true;
                    _ringSelected++;
                    StartCoroutine(MoveSelector(_ringSelected - 1, _ringSelected));
                    if (_ringSelected >= Icons.Length)
                        _ringSelected = 0;
                }
            }
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Returns true if the menu is closed and not animating, false otherwise.
    /// </summary>
    public bool GetMenuIsClosed() => !_ringOpen && !_ringOpenCloseAnimation;
    #endregion

    #region Coroutines
    /// <summary>
    /// Moves the Selection ring to the desired slot when the Ring menu is open.
    /// </summary>
    private IEnumerator MoveSelector(int last, int next)
    {
        float angle;
        for (var frameTime = 0f; frameTime < SelectTime; frameTime += Time.deltaTime)
        {
            var time = frameTime / SelectTime;
            // Move the selector every frame
            angle = _ringAngleOffset - (_ringAngleDiff * Mathf.Lerp(last, next, time));
            _selector.gameObject.transform.localPosition = new Vector3(Mathf.Cos(angle) * Distance, Mathf.Sin(angle) * Distance, 0f);
            yield return new WaitForEndOfFrame();
        }
        // Set the final position so we don't miss a step
        angle = _ringAngleOffset - (_ringAngleDiff * next);
        _selector.gameObject.transform.localPosition = new Vector3(Mathf.Cos(angle) * Distance, Mathf.Sin(angle) * Distance, 0f);
        yield return new WaitForEndOfFrame();
        _selectorMoving = false;
    }

    /// <summary>
    /// Play the animation to Open the Ring menu.
    /// </summary>
    private IEnumerator OpenRing()
    {
        SetRingActive(true);
        for (var frameTime = 0f; frameTime < OpenCloseTime; frameTime += Time.deltaTime)
        {
            var time = frameTime / OpenCloseTime;
            // Move the slots every frame
            SetPosDuringOpenClose(Mathf.Lerp(0, Distance, time), Mathf.Deg2Rad * 360f * OpenCLoseSpins * time);
            // Change the transparency of the slots every frame
            SetRingAlpha(time);
            yield return new WaitForEndOfFrame();
        }
        // Set the final position so we don't miss a step
        SetPosDuringOpenClose(Distance, 0f);
        yield return new WaitForEndOfFrame();
        _ringOpenCloseAnimation = false;
    }

    /// <summary>
    /// Play the animation to Close the Ring.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CloseRing()
    {
        for (var frameTime = 0f; frameTime < OpenCloseTime; frameTime += Time.deltaTime)
        {
            var time = frameTime / OpenCloseTime;
            // Move the slots every frame
            SetPosDuringOpenClose(Mathf.Lerp(Distance, 0f, time), Mathf.Deg2Rad * 360f * OpenCLoseSpins * (1f - time));
            // Change the transparency of the slots every frame
            SetRingAlpha(1f - time);
            yield return new WaitForEndOfFrame();
        }
        // Set the final position so we don't miss a step
        SetPosDuringOpenClose(0f, 0f);
        SetRingActive(false);
        yield return new WaitForEndOfFrame();
        _ringOpenCloseAnimation = false;

        // Fire an event so other systems can see we have closed the menu
        OnItemSelected?.Invoke(_ringSelected);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Sets the Slots and the Selector active or inactive.<br/>
    /// This makes them visible or invisible, respectively.
    /// </summary>
    private void SetRingActive(bool active)
    {
        for (var i = 0; i < Icons.Length; i++)
        {
            _slots[i].gameObject.transform.localPosition = Vector3.zero;
            _slots[i].gameObject.SetActive(active);
        }
        _selector.gameObject.SetActive(active);
    }

    /// <summary>
    /// Sets the relative position of the Slots and the Selector.
    /// </summary>
    private void SetPosDuringOpenClose(float distance, float offset)
    {
        // Set the position of the slots to the relative angle and distance
        for (var i = 0; i < Icons.Length; i++)
        {
            var x = _ringAngleOffset - (_ringAngleDiff * i) + offset;
            _slots[i].gameObject.transform.localPosition = new Vector3(Mathf.Cos(x) * distance, Mathf.Sin(x) * distance, 0f);
        }
        // Set the position of the selector to the relative angle and distance of the selected slot
        var y = _ringAngleOffset - (_ringAngleDiff * _ringSelected) + offset;
        _selector.gameObject.transform.localPosition = new Vector3(Mathf.Cos(y) * distance, Mathf.Sin(y) * distance, 0f);
    }

    /// <summary>
    /// Sets the alpha (transparency) of the Slots and the Selector.
    /// </summary>
    private void SetRingAlpha(float alpha)
    {
        var color = new Color(1f, 1f, 1f, alpha);
        // Set the alpha of the slots
        for (var i = 0; i < Icons.Length; i++)
        {
            _slots[i].Sprite.color = color;
            _slots[i].ItemSprite.color = color;
        }
        // Set the alpha of the selector
        _selector.color = color;
    }
    #endregion
}
