/*
The Game Timer

Handles all the time aspects of the game, including stopping time (pause state), stopwatch logic and display
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _counterText; // Our text field to display the time
    [SerializeField] private Image _timerImage; // Background image for graphic effect
    private GameManager _gameManager;
    private float _seconds, _startTime, _gameTime = 60f, _timeLeft; // We'll be needing those for calculations
    private bool _runTimer;
    public float seconds
    {
        get { return _seconds; } // Make this variable available for the data packager
    }
    public float gameTime
    {
        set { _gameTime = value; } // Enable setting this from the game manager
    }
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>(); // Cache game manager
        SetTime(0); // Initialize time 
        SetTimerText(); // Initialize the text
        ToggleTime(0); // Stop time
        _runTimer = true;
    }
    void Update()
    {
        if (_runTimer)
        {
            _seconds = (Time.time - _startTime); // Calculate seconds that have passed sinf the game started
            _timeLeft = ((_gameTime) - _seconds); // Calculate the time left till the game ends
            if (_timeLeft <= 0) // If the timer hits 0 - end the game
            {
                _runTimer = false;
                _timeLeft = 0;
                _gameManager.GameOver();
            }
            SetTimerText(); // Update the timer text
            SetTimerFill(); // Update the graphic effect
        }

    }
    private void SetTimerFill()
    {
        _timerImage.fillAmount = _timeLeft / _gameTime;
    }
    private void SetTimerText()
    {
        _counterText.text = _timeLeft.ToString("0.0"); // Normalize the time and update
    }
    public void SetTime(float savedTime)
    {
        _startTime = Time.time - savedTime; // Helper for loading saved time
    }
    public string ToggleTime() // Helper method to toggle time itself
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
        return null;
    }
    public void ToggleTime(int timeScale = default) // Overloaded method - recieves an int representing time scale and sets time to that scale
    {
        Time.timeScale = timeScale;
    }
}
