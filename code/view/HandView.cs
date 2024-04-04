using Godot;
using System;
using System.Collections.Generic;

public partial class HandView : Node2D, CardObserver
{
	// 25 + kitty
	private static readonly int MAX_HAND_SIZE = 33;
	
	private static readonly int SELECTED_DELTA_Y = 20;
	
	List<CardView> cardViews;
	List<Card> cards;
	HashSet<int> selectedIndices;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cardViews = new List<CardView>();
		cards = new List<Card>();
		selectedIndices = new HashSet<int>();
		
		// initialize child views
		for (var i = 0; i < MAX_HAND_SIZE; i++) {
			var instance = GD.Load<PackedScene>("res://gameview/CardView.tscn").Instantiate();
			AddChild(instance);
			var cardView = (CardView)instance;
			// set self as the observer so we can get notified on click
			cardView.SetObserver(this);
			cardViews.Add(cardView);
			cardView.Deactivate();
		}
		GetTree().Root.Connect("size_changed", Callable.From(() => centerHand()));
		centerHand();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetCards(List<Card> cards) {
		this.cards = cards;
		deselectAll();
		updateView();
	}
	
	private void deselectAll() {
		foreach (var index in selectedIndices) {
			updateSelectedView(index, false);
		}
		selectedIndices = new HashSet<int>();
	}
	
	private void updateView() {
		var numCardsInHand = cards.Count;
		
		// start from the middle, move back X / 2 spaces and use that as the starting point
		var startIndex = MAX_HAND_SIZE / 2 - numCardsInHand/2;
		var endIndex = startIndex + numCardsInHand;
		

		for (int i = 0; i < MAX_HAND_SIZE; i++) {
			if (i >= startIndex && i < endIndex) {
				Card card = cards[i-startIndex]; // need to subtract start index.
				CardView cardView = cardViews[i];

				cardViews[i].SetCard(card);
			} else {
				cardViews[i].Deactivate();
			}
		}
		centerHand();
	}

	private void centerHand() {

		var cardWidth = cardViews[0].GetCardSize();
		var viewSizeWidth = (int)GetViewport().GetVisibleRect().Size.X;
		
		// if the hand is full, we want this length of buffer on each side
		var edgeBuffer = cardWidth / 2;
		 // need to subtract some portion of card width so we can draw the last card
		var remainingWidth = viewSizeWidth - 2 * edgeBuffer - cardWidth / 2;
		var sizePerCard = remainingWidth / MAX_HAND_SIZE;
		var startX = edgeBuffer;
		for (int i = 0; i < MAX_HAND_SIZE; i++) {
			cardViews[i].Position = new Vector2(startX, 0);
			startX += sizePerCard;
		}

	}
	
	// a card in this hand was clicked
	public void NotifyClicked(CardView c) {
		int index = this.cardViews.IndexOf(c);
		if (index >= 0) {
			if (selectedIndices.Contains(index)) {
				selectedIndices.Remove(index);
				updateSelectedView(index, false);
			} else {
				selectedIndices.Add(index);
				updateSelectedView(index, true);
			}
		}
	}
	
	private void updateSelectedView(int index, bool selected) {
		var cardView = cardViews[index];
		if (selected) {
			cardView.Position -= new Vector2(0, SELECTED_DELTA_Y);
		} else {
			cardView.Position += new Vector2(0, SELECTED_DELTA_Y);
		}
	}
	
	public List<Card> GetSelectedCards() {
		List<Card> selectedCards = new List<Card>();
		foreach (int index in selectedIndices) {
			// if we try to access it from our card list.. it wouldnt work... i guess
			selectedCards.Add(cardViews[index].GetCard());
		}
		return selectedCards;
	}
}
