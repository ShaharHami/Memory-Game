/* 
Popup beahviour / Manager 

This API Manages the popup and overlay animation as well as activation / deactivation of popup content
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _popupTransform; // Our popup object
    [SerializeField] private Image _overlay; // The overlay
    [SerializeField] private List<Popup> popups;
    [SerializeField] private float _hiddenPosition, _visiblePosition, _popupDelay, _transitionTime, _overlayAlpha;
    private float _targetPosition, _targetAlpha;
    public enum PopupMode // Mode selector for desired animation ( show, hide, show and auto hide)
    {
        PopupInOut,
        PopupIn,
        PopupOut,
    }
    public static PopupMode popupMode;
    void Start()
    {

        MovePopup(default, false); // Set the popup to it's hidden location
        HandleOverlayDisplay(); // Set the overlay to transparent
    }
    void Update()
    {
        // Check if we need to run the block at all
        if (_popupTransform.localPosition.y != _targetPosition || _overlay.color.a != _targetAlpha)
        {
            float time = _transitionTime * Time.unscaledDeltaTime; // Calculate movement outside of scaled time
            // Calculate and execute an eased movement to the desired position
            _popupTransform.localPosition = Vector3.Lerp(_popupTransform.localPosition, new Vector3(_popupTransform.localPosition.x, _targetPosition, _popupTransform.localPosition.z), time);
            // Set the target alpha value to correspond to the popup position
            HandleOverlayDisplay();
            // Calculate and execute an eased change of the alpha value
            _overlay.color = new Color(_overlay.color.r, _overlay.color.g, _overlay.color.b, Mathf.Lerp(_overlay.color.a, _targetAlpha, time));
        }
    }
    // Exposed method acts as API
    // params: string, PopupMode (optional), float (optional)
    public void TriggerPopup(string name, PopupMode mode = default, float popupDelay = default)
    {
        if (name == null) // Catch no name provided
        {
            Debug.LogWarning("You did not specify a popup to trigger");
            return;
        }
        else
        {
            SetPopupType(name); // Set the popup content to use
        }
        if (popupDelay != 0)
        {
            _popupDelay = popupDelay; // Set the popup delay (for PopupMode.PopupInOut)
        }
        // Move popup by mode
        switch (mode)
        {
            case PopupMode.PopupIn:
                MovePopup(_visiblePosition);
                break;
            case PopupMode.PopupOut:
                MovePopup();
                break;
            case PopupMode.PopupInOut:
                MovePopup(_visiblePosition);
                StartCoroutine(MoveInOut()); // Use coroutine to auto hide popup when using PopupMode.PopupInOut
                break;
        }
    }
    private void SetPopupType(string name)
    {
        bool found = false; // Boolean used to check if popup content exists
        foreach (Popup popup in popups)
        {
            if (popup.name == name)
            {
                popup.popupGo.SetActive(true); // Activate the selected content
                found = true; // Set check bool to pass the check
            }
            else
            {
                popup.popupGo.SetActive(false); // Deactivate all others
            }
        }
        if (!found) // Handle content not found
        {
            Debug.LogWarning("There is no popup by the name " + name);
        }
    }
    private IEnumerator MoveInOut() // Coroutine to handle auto hide popup in PopupMode.Popup.InOut
    {
        yield return new WaitForSecondsRealtime(_popupDelay);
        MovePopup();
    }
    // Set the target position of the popup which will move it in the Update method
    private void MovePopup(float targetPosition = default, bool playSound = true)
    {
        if (playSound)
        {
            AudioManager.Instance.PlaySound("Swoosh", true);
        }
        if (targetPosition == 0)
        {
            targetPosition = _hiddenPosition;
        }
        _targetPosition = targetPosition;
    }
    // Set the alpha target value for the overlay which will be changed in the Update method
    private void HandleOverlayDisplay()
    {
        if (_targetPosition == _hiddenPosition)
        {
            _targetAlpha = 0;
        }
        else if (_targetPosition == _visiblePosition)
        {
            _targetAlpha = _overlayAlpha;
        }
    }
    // Simple class to organize our popup content elements
    [System.Serializable]
    private class Popup
    {
        public string name;
        public GameObject popupGo;
    }
}
