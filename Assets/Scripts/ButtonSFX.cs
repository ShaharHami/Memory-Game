/*
Button Sound Effects

Simple helper class to play button sounds on hover and click
*/
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    [SerializeField] private string _buttonHoverSoundName, _buttonClickSoundName;
    private GameManager _gameManager;
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>(); // Cache GameManager
        if (_buttonClickSoundName == "" || _buttonHoverSoundName == "")
        {
            Debug.LogWarning("You didn't sepcify sound names");
        }
    }
    public void OnPointerEnter() // Triggered usign a trigger event component on the button prefab
    {
        if (!_gameManager.gameOver || gameObject.tag == "PopupButton") // Don't play button sounds if game over
        {
            AudioManager.Instance.PlaySound(_buttonHoverSoundName, true);
        }
    }
    public void OnPointerDown()
    {
        if (!_gameManager.gameOver || gameObject.tag == "PopupButton")
        {
            AudioManager.Instance.PlaySound(_buttonClickSoundName, true);
        }
    }
}
