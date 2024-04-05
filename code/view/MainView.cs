using Godot;
using System;
using System.Collections.Generic;

public partial class MainView : Node2D, BidMenuListener, MiscMenuListener, ModelSub
{
	HandView handView;
	ScoreView scoreView;
	BidMenu bidMenu;
	MiscMenu miscMenu;
	TrickView trickView;
	Timer bidPassTimer;
	
	Game game;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		bidPassTimer = new Timer();
		AddChild(bidPassTimer);
		bidPassTimer.Timeout += bidPass;
		bidPassTimer.OneShot = true;
		bidPassTimer.WaitTime = ViewSettings.GetBidDelay();
		
		var handScene = GD.Load<PackedScene>("res://gameview/HandView.tscn");
		var instance = handScene.Instantiate();
		AddChild(instance);
		handView = (HandView)instance;
		handView.SetSelectable(true);
		
		var bidScene = GD.Load<PackedScene>("res://gameview/BidMenu.tscn");
		var bidInstance = bidScene.Instantiate();
		AddChild(bidInstance);
		bidMenu = (BidMenu)bidInstance;
		bidMenu.SetListener(this);
		
		var miscScene = GD.Load<PackedScene>("res://gameview/MiscMenu.tscn");
		var miscInstance = miscScene.Instantiate();
		AddChild(miscInstance);
		miscMenu = (MiscMenu)miscInstance;
		miscMenu.SetListener(this);
		
		
		var scoreScene = GD.Load<PackedScene>("res://gameview/ScoreView.tscn");
		var scoreInstance = scoreScene.Instantiate();
		AddChild(scoreInstance);
		scoreView = (ScoreView)scoreInstance;
		
		var trickScene = GD.Load<PackedScene>("res://gameview/TrickView.tscn");
		var trickInstance = trickScene.Instantiate();
		AddChild(trickInstance);
		trickView = (TrickView)trickInstance;
		
		game = new Game();
		game.addSubscriber(this);
		scoreView.SetGame(game);
		
		// Start the game after we initialized everything
		game.newGame(4);
		rearrangeViews();
		GetTree().Root.Connect("size_changed", Callable.From(() => rearrangeViews()));

	}
	
	private void rearrangeViews() {
		var winHeight = (int)GetViewport().GetVisibleRect().Size.Y;
		var winWidth = (int)GetViewport().GetVisibleRect().Size.X;
		var scoreViewWidth = 150;
		handView.Position = new Vector2(0, winHeight-120);

		bidMenu.Position = new Vector2(500, 0);
		miscMenu.Position = new Vector2(700, 0);
		trickView.SetXOffset(scoreViewWidth);
		trickView.Position = new Vector2(scoreViewWidth, 0);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

	}

	// READ KEYBOARD EVENTS FOR SHORTCUTS
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey) {
			if (eventKey.Pressed) {
				if (eventKey.Keycode == Key.Escape) {
					GetTree().Quit();
				} else if (eventKey.Keycode == Key.Space) {
					GameState state = game.getState();
					if (state == GameState.PLAY) {
						PlayPressed();
					} else if (state == GameState.KITTY) {
						BuryPressed();
					} else if (state == GameState.DEAL) {
						bidPass();
					} else if (state == GameState.PRE_DEAL) {
						NextPressed();
					}  else if (state == GameState.END) {
						NewGamePressed();
					}
				}
			}
		}
	}

	// Bid menu
	public void BidPressed(Suit s) {
		game.receiveBid(game.getHumanPlayer(), s);
		if (game.getState() == GameState.DEAL) {
			// restart the clock
			startBidTimer();
		} else {
			stopBidTimer();
		}
	}
	
	// ModelSub
	public void RoundDataUpdated() {
		
	}
	
	public void ScoreDataUpdated() {
		
	}
	public void GameStateChanged(GameState s) {
		if (s == GameState.PLAY) {
			trickView.Show();
			stopBidTimer();
		} else if (s == GameState.DEAL) {
			trickView.Hide();
			startBidTimer();
		}
	}

	private void stopBidTimer() {
		bidPassTimer.Stop();
	}

	private void startBidTimer() {
		bidPassTimer.OneShot = true;
		bidPassTimer.WaitTime = ViewSettings.GetBidDelay();
		bidPassTimer.Start();
	}

	private void bidPass() {
		BidPressed(Suit.SUIT_INVALID);

	}

	public void GameEventOccurred(string s) {
		// GD.Print("EVENT ", s);
		if (s == GameEvents.HAND_UPDATE) {
			var hand = game.getHumanPlayer().getHand();
			handView.SetCards(hand);
		} else if (s == GameEvents.PLAY_REQUEST) {
			trickView.SetTrick(game.getCurrentTrick());
		} else if (s == GameEvents.TRICK_END) {
			trickView.EndTrick(game.getCurrentTrick());
		} else if (s == GameEvents.TRICK_UPDATE) {
			//trickView.SetTrick(game.getCurrentTrick());
		} else if (s == GameEvents.KITTY_DEPOSIT) {

		} else if (s == GameEvents.KITTY_DEPOSITED) {
			var hand = game.getHumanPlayer().getHand();
			handView.SetCards(hand);
		}
	}
	
	public void NotifyHumanInput(string s) {
		var hand = game.getHumanPlayer().getHand();
		handView.SetCards(hand);
	}
	
	// MiscMenu
	public void BuryPressed() {
		List<Card> cards = handView.GetSelectedCards();
		if (cards.Count != 0) {
			game.receiveBury(cards);
		}
	}
	
	public void PlayPressed() {
		List<Card> cards = handView.GetSelectedCards();
		if (cards.Count != 0) {
			game.receivePlay(game.getHumanPlayer(), cards);
		}
	}
	
	public void NextPressed() {
		game.newRound();
	}
	
	public void NewGamePressed() {
		game.newGame(4);
	}
}





