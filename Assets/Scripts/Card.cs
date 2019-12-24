/*
The Card

Holds all the inner logic of a single card, including sound and animation
*/

using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public static List<Card> AllCards; // easy access to all the cards
    [HideInInspector] public static bool flip = false;
    [HideInInspector]
    public enum State // card state
    {
        Obscured,
        Visible,
        Matched,
    }
    [HideInInspector] public State state;
    [SerializeField] private float _delay = 0.3f;
    private int _cardValue;
    private GameManager _manager;
    private Vector3 desiredRotation;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private SpriteRenderer _faceImage;
    [SerializeField] private SpriteRenderer _backImage;
    [SerializeField] private GameObject _cardHover;
    public int cardValue
    {
        get { return _cardValue; }
        set { _cardValue = value; }
    }
    public float delay
    {
        get { return _delay; }
        set { _delay = value; }
    }
    public bool Flip
    {
        get { return flip; }
        set { flip = value; }
    }
    private void OnEnable()
    {
        if (AllCards == null) // Set up Allcards list. If the list ain't initialized - initialize that puppy
        {
            AllCards = new List<Card>();
        }
        AllCards.Add(this); // Add this card to the list
    }
    private void OnDisable()
    {
        AllCards.Remove(this); // Remove this card from the list on disable
    }
    void Start()
    {
        desiredRotation = transform.rotation.eulerAngles; // Cache rotation
        _manager = GameObject.FindObjectOfType<GameManager>(); // Cache GameManager
        _cardHover.SetActive(false);
    }
    void Update()
    {
        if (transform.rotation.eulerAngles != desiredRotation) // Rotate the card if true
        {
            transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, desiredRotation, rotationSpeed * Time.unscaledDeltaTime));
        }
    }
    void OnMouseOver()
    {
        if (flip && !_manager.gameOver)
        {
            _cardHover.SetActive(true); // Show hover layer
        }
        if (Input.GetMouseButtonDown(0)) // Flip the card on click
        {
            Flipcard();
        }
    }
    void OnMouseExit()
    {
        _cardHover.SetActive(false); // Hide hover layer
    }
    public void SetupGraphics(Sprite front) // Set card face
    {
        _faceImage.sprite = front;
    }

    public void Flipcard() // OnClick handler when card is clicked
    {
        if (flip && state != State.Matched)
        {
            AudioManager.Instance.PlaySound("Card Flip", true);
            state = State.Visible;
            desiredRotation.y = 180;
        }
        _manager.CheckCards();
    }
    public void InitiateDelay() // Start delay
    {
        Invoke("RealizeCardState", delay);
    }

    public void RealizeCardState() // flip the card back if it is unmatched or keep it visible if it is matched
    {
        if (state != State.Matched)
        {
            state = State.Obscured;
            desiredRotation.y = 0;
        }
        else
        {
            desiredRotation.y = 180;
        }
        Card.flip = true;
    }
}
