using Godot;
using System;

public partial class ScoreView : Node2D, LoggerListener, ModelSub
{
	RichTextLabel scoreData;
	RichTextLabel roundData;
	RichTextLabel lastTrickData;
	RichTextLabel textLog;
	
	// laziness... just set the game here.
	Game game;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scoreData = GetNode<RichTextLabel>("ScoreData");
		roundData = GetNode<RichTextLabel>("RoundData");
		lastTrickData = GetNode<RichTextLabel>("LastTrickData");
		textLog = GetNode<RichTextLabel>("TextLog");
		Logger.subscribe(this);
	}
	
	public void SetGame(Game g) {
		this.game = g;
		game.addSubscriber(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void logMessage(string s) {
		textLog.AddText(s);
	}
	
	private void roundDataUpdated() {
		if (game == null) {
			return;
		}
		var text = "";
		GameState state = game.getState();
		if (state == GameState.PLAY) {
			var dealerName = game.getDealer().getName();
			var trumpRank = game.getTrumpRank();
			var trumpSuit = game.getTrumpSuit();
			var defenderPoints = game.getDefenderPoints();
			text = $"Dealer:{dealerName}\nTrump Rank:{trumpRank}\nTrump Suit:{trumpSuit}\nDefender Points:{defenderPoints}\n";
		} else {
			var trumpRank = game.getTrumpRank();
			var dealerName = game.getDealer().getName();
			var bid = game.getLargestBid();
			if (bid == null) {
				text = $"Dealer:{dealerName}\nTrump Rank:{trumpRank}\nHighest Bid: None\n";
			} else {
				var bidSuit = bid.getSuit();
				var bidCopies = bid.getNumCopies();
				var largestBidPlayer = bid.getPlayer().getName();
				text = $"Dealer:{dealerName}\nTrump Rank:{trumpRank}\nHighest Bid: {bidCopies} {bidSuit} by {largestBidPlayer}\n";
			}
		}
		
		if (game.getState() != GameState.DEAL) {
			if (game.getHumanPlayer() == game.getDealer() || game.getState() == GameState.PRE_DEAL) {
				text += $"Kitty: {CardUtils.getCardListString(game.getKitty())}\n";
			} else {
				text += "Kitty: ???\n";
			}
		}

		roundData.Text = text;
	}
	
	private void scoreDataUpdated() {
		var neTeam = game.getPlayer(0).getCurrentLevel();
		var ewTeam = game.getPlayer(1).getCurrentLevel();
		scoreData.Text = $"Seed:{CppUtils.GetSeed()}\nNorth/South Level: {neTeam}\nEast/West Level:{ewTeam}";
	}
	
	private void lastTrickDataUpdated()  {
		var text = "";
		var lastTrick = game.getLastTrick();
		if (lastTrick != null) {
			text += $"Last Trick:\n{lastTrick}\n";
		} else {
			text += "Last Trick:\n";
		}
		lastTrickData.Text = text;
	}
	
	// ModelSub
	public void GameStateChanged(GameState s) {
		if (s != GameState.PRE_DEAL && s != GameState.END) {
			roundDataUpdated();
		} else if (s == GameState.PRE_DEAL) {
			scoreDataUpdated();
			lastTrickDataUpdated();
			roundDataUpdated();
		}
	}
	public void GameEventOccurred(string s) {
		if (s == GameEvents.BID_UPDATE || s == GameEvents.TRICK_UPDATE || s == GameEvents.KITTY_DEPOSITED) {
			roundDataUpdated();
		} else if (s == GameEvents.TRICK_END) {
			lastTrickDataUpdated();
		}
	}
	
	public void NotifyHumanInput(string s) {
	}
}
