/*
Game Manager

Well, manages the game.. 
Most of the game logic is in this class
*/
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour, IDataManagable
{
    [SerializeField] private Sprite[] cardFace;
    private Timer gameTimer;
    [SerializeField] private PopupBehaviour _popupBehaviour;
    [SerializeField] private float _gameTime = 60;
    private int _matches;
    private bool _gameOver;
    private bool _loaded = false;
    [SerializeField] private TextMeshProUGUI _startButtonText;
    [SerializeField] private string _startText, _pauseText;
    [SerializeField] float _matchSoundDelay;
    [HideInInspector]
    public bool gameOver
    {
        get { return _gameOver; }
    }
    #region GameOperation
    private void Start()
    {
        _matches = Card.AllCards.Count / 2; // Set number of possible matches to half the number of cards
        if (!CheckBoard()) // Run some checks on our board setup
        {
            return;
        }
        AudioManager.Instance.PlaySound("BGM", false); // Resume paused BGM
        gameTimer = FindObjectOfType<Timer>(); // Cache our timer
        if (gameTimer != null)
        {
            gameTimer.gameTime = _gameTime; // Initialize game time
            _gameOver = false; // Just in case
        }
        InitializeCards(); // Initialize the cards
        DisableCards(); // Disable the cards
        _startButtonText.text = _startText; // Set the start button text
    }

    public void GameOver() // Start game over sequence
    {
        gameTimer.ToggleTime(0);
        DisableCards(); // Disable the cards
        _gameOver = true;
        string type = "GameOverWin";
        if (_matches != 0) // Check if the player won
        {
            type = "GameOverLose";
        }
        _popupBehaviour.TriggerPopup(type, PopupBehaviour.PopupMode.PopupIn); // trigger popup with message
        AudioManager.Instance.PlayPauseSound("BGM"); // Pause BGM
        AudioManager.Instance.PlaySound(type, true); // Play win or lose sound 
    }
    #endregion
    #region CardLogic
    void InitializeCards()
    {
        if (_loaded) // If we loaded a game before we started a new game - skip arraging the cards randomly
        {
            return;
        }
        List<int> _values = new List<int>(); // Initialize a list of values. We assign each pair of cards a corresponding value and use those values to set the cards and check matches 
        for (int i = 0; i < _matches; i++) // Generate a list with (cards * 0.5) pairs of values 
        {
            _values.Add(i);
            _values.Add(i);
        }
        foreach (Card card in Card.AllCards) // Assign values to cards
        {
            int randIndex = Random.Range(0, _values.Count); // Randomize a location in the list and retrieve the value at that location
            int randVal = _values[randIndex];
            _values.RemoveAt(randIndex); // Remove the randomized value from the list
            card.cardValue = randVal; // Set card value
            card.SetupGraphics(cardFace[randVal]); // Set card textures
        }
    }
    public void CheckCards()
    {
        List<int> cardNumbers = new List<int>(); // Create an empty list
        for (int i = 0; i < Card.AllCards.Count; i++) // Iterate over cards and populate the list with visible cards
        {
            if (Card.AllCards[i].state == Card.State.Visible)
            {
                cardNumbers.Add(i);
            }
            if (cardNumbers.Count == 2) // If there are two visible cards go to check if they match and break out of the loop
            {
                CardComparison(cardNumbers);
                break;
            }
        }
    }

    void CardComparison(List<int> cardNumbers)
    {
        Card.flip = false; // Disable flipping more than two cards

        Card.State x = Card.State.Obscured; // Initialize a state

        if (Card.AllCards[cardNumbers[0]].cardValue == Card.AllCards[cardNumbers[1]].cardValue) // Compare card values
        {
            Invoke("PlayMatchSound", _matchSoundDelay);
            x = Card.State.Matched; // Change the state
            _matches--; // Decrease matches
            if (_matches == 0)
            {
                GameOver(); // If all cards are matched - start the game over sequence
            }
        }
        for (int i = 0; i < cardNumbers.Count; i++)
        {
            Card.AllCards[cardNumbers[i]].state = x; // Set the card state
            if (x == Card.State.Obscured)
            {
                Card.AllCards[cardNumbers[i]].InitiateDelay(); // Trigger delay before flipping back (if card state is obscured)
            }
            else
            {
                Card.flip = true; // Set flippability back to true
            }
        }
    }
    #endregion
    #region InitialTests
    private bool CheckBoard()
    {
        // Consider implementing a proper error handler class
        bool result = true;
        if (Card.AllCards.Count % 2 != 0) // Make sure that we have an even amount of cards
        {
            Debug.LogWarning("You have an odd number of cards. Add a card or remove one, as long as you have an even number");
            DisableCards();
            result = false;
        }
        if (cardFace.Length < _matches) // Make sure we have enough card faces (1 card face for each pair of cards)
        {
            Debug.LogWarning("You don't have enough card faces (textures). Add at least " + (_matches - cardFace.Length).ToString() + "more textures");
            DisableCards();
            result = false;
        }
        return result;
    }
    #endregion
    #region Helper Methods
    // Helper methods
    public void StartPauseGame() // This is also our start game button
    {
        if (!_gameOver) // If the game is over this button is useless
        {
            ToggleButtonText(null);
            gameTimer.ToggleTime(); // Toggle the time
            Card.flip = !Card.flip; // Toggle the cards
        }
    }
    private void ToggleButtonText(string text)
    {
        if (text != null)
        {
            _startButtonText.text = text;
            return;
        }
        if (_startButtonText.text != _startText)
        {
            _startButtonText.text = _startText;
        }
        else
        {
            _startButtonText.text = _pauseText;
        }
    }
    private static void DisableCards()
    {
        Card.flip = false; // Disable flipping cards
    }
    private void PlayMatchSound()
    {
        AudioManager.Instance.PlaySound("Card Match", true);
    }

    public void PlayAgain() // Reset our timer and reload our scene
    {
        gameTimer.SetTime(0);
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }
    #endregion
    #region Data Utility
    // Data utility - Implementation of the IDataManagable interface
    // These two methods PackDataAndSave() and LoadDataAndUnPack() are completly independent, consider placing in a DataPackageUtil class
    public void PackDataAndSave()
    {
        if (!_gameOver)
        {
            _popupBehaviour.TriggerPopup("Saving");
            // Consider reusing the data container and lists instead of making new ones for each save
            // Is this collected?
            Data data = new Data(); // Prepare our data package
            data.seconds = gameTimer.seconds;
            data.matches = _matches;
            data.cardValues = new List<int>();
            data.states = new List<string>();
            data.positions = new List<Vector3>();
            foreach (Card card in Card.AllCards)
            {
                data.cardValues.Add(card.cardValue);
                data.states.Add(card.state.ToString());
                data.positions.Add(card.transform.position);
            }
            DataManager.Instance.SaveData<Data>("GameData", data); // Save data to API
        }
    }
    public void LoadDataAndUnPack()
    {
        if (!_gameOver)
        {
            _popupBehaviour.TriggerPopup("Loading");
            Data data = DataManager.Instance.LoadData<Data>("GameData"); // Load the data from the API
            if (data != default(Data)) // Check if we got back our data
            {
                for (var i = 0; i < data.states.Count; i++) // Unpack
                {
                    Card.AllCards[i].state = (Card.State)System.Enum.Parse(typeof(Card.State), data.states[i]); // Set card state
                    Card.AllCards[i].cardValue = data.cardValues[i]; // Set card value
                    Card.AllCards[i].SetupGraphics(cardFace[data.cardValues[i]]); // Set card face
                    Card.AllCards[i].transform.position = data.positions[i]; // Set card position
                    Card.AllCards[i].RealizeCardState();
                }
                _matches = data.matches;
                gameTimer.SetTime(data.seconds); // Set timer to saved time
                gameTimer.ToggleTime(0); // Freeze time
                DisableCards();
                ToggleButtonText(_startText);
                _loaded = true;
            }
        }
    }
    #endregion
}
