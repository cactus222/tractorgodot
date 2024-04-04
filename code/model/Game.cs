using System.Collections.Generic;


public class Game {

	GameState state;

	public Game() {
		state = GameState.END;
	}

	public GameState getState() {
		return state;
	}

	public void newGame(int numPlayers) {
		if (state != GameState.END) {
			Logger.logMessage("cant state new game while in the middle one of one");
			return;
		}
		
		players = new Player[numPlayers];
		players[0] = new Human(this);
		players[0].setName("South");
		for (int i = 1; i < numPlayers; i++) {
			players[i] = new Computer(this);
		}
		players[1].setName("West");
		players[2].setName("North");
		players[3].setName("East");
		NUM_PLAYERS = numPlayers;
		dealer = players[lastWinner];
		lastWinner = 0;
		setGameState(GameState.PRE_DEAL);
		newRound();
	}
	public void newRound() {
		if (state != GameState.PRE_DEAL) {
			Logger.logMessage("can't start new round, in the middle of one.");
			return;
		}

		largestBid = null;
		dealer = players[lastWinner];
		currentTrick = null;
		turn = 0;
		trumpSuit = Suit.NO_TRUMP;
		trumpRank = getPlayer(lastWinner).getCurrentLevel();
		attackers.Clear();
		defenders.Clear();
		kitty.Clear();
		// player across from dealer is partner
		attackers.Add(players[lastWinner]);
		attackers.Add(players[(lastWinner+2)%4]);
		Logger.logMessage($"attackers added {lastWinner} {(lastWinner+2)%4}");

		// others def
		defenders.Add(players[(lastWinner+1)%4]);
		defenders.Add(players[(lastWinner+3)%4]);
		Logger.logMessage($"defenders added {(lastWinner+1)%4} {(lastWinner+3)%4}");

		//currentTrick.Clear();
		//roundTricks.Clear();

		for (int i = 0; i < NUM_PLAYERS; i++) {
			Player player = players[i];
			player.newRound(trumpRank);
		}


		currentDeal = lastWinner;
		KITTY_SIZE = 8;

		//Add in regular cards.
		for (int deckNum = 0; deckNum < 2; deckNum++) {

			foreach (KeyValuePair<Suit, int> suitPair in GlobalMembers.SUIT_MAPPING) {

				foreach (KeyValuePair<Rank, int> rankPair in GlobalMembers.RANK_MAPPING) {
					deck.Add(new Card(rankPair.Key, suitPair.Key));
				}
			}
			//Add in jokers
			deck.Add(CardUtils.generateJoker(Rank.JOKER_COL));
			deck.Add(CardUtils.generateJoker(Rank.JOKER_UNC));
		}

		clearTricks();

		shuffleDeck();
		createKitty();
		CardUtils.setTrumpDetails(trumpRank, Suit.NO_TRUMP);

		setGameState(GameState.DEAL);
		dealCard(true);
	}

	

	public void clearTricks() {
		wonTricks.Clear();
	}

	public Suit getTrumpSuit() {
		return trumpSuit;
	}
	public Rank getTrumpRank() {
		return trumpRank;
	}
	public Player getPlayer(int pos) {
		return players[pos];
	}
	public Player getHumanPlayer() {
		return players[0];
	}

	public Player getCurrentPlayer() {
		if (state == GameState.DEAL) {
			return players[currentDeal];
		} else {
			return players[turn];
		}
	}
	public void addSubscriber(ModelSub m) {
		observers.Add(m);
	}
	//Ret valid play or not
	public bool receivePlay(Player player, List<Card> cards) {

		// Logger.logMessage($"{getPlayerIndex(player)} Attempted to Play " + cardsToString(cards));
		// Logger.logMessage("Player Index " + std::to_string(getPlayerIndex(player)));
		if (turn != getPlayerIndex(player)) {
			Logger.logMessage("Playing out of order?");
			return false;
		}
		if (cards.Count == 0) {
			Logger.logMessage("CANT PLAY 0 cards ");
			return false;
		}
		if (!player.hasCards(cards)) {
			Logger.logMessage("PLAYER DOESNT EVEN HAVE THOSE CARDS");
			return false;
		}
		if (currentTrick.isLead()) {
			if (isValidFirstPlay(player, cards)) {

				player.removeCards(cards);
				if (isHumanPlayer(player)) {
					notifyEvent(GameEvents.HAND_UPDATE);
				}
				currentTrick.setPlay(getPlayerIndex(player), cards);
				notifyEvent(GameEvents.TRICK_UPDATE);
			} else {
				Logger.logMessage("INVALID FIRST PLAY");
				return false;
			}
		} else {
			//Must follow suit/style

			if (isValidFollowTrick(player, cards)) {

				player.removeCards(cards);
				if (isHumanPlayer(player)) {
					notifyEvent(GameEvents.HAND_UPDATE);
				}

				currentTrick.setPlay(getPlayerIndex(player), cards);
			} else {
				Logger.logMessage("INVALID FOLLOW TRICK");
				return false;
			}
		}
		
		logGameState();
		notifyEvent(GameEvents.TRICK_UPDATE);
		//Check if trick is full

		if (currentTrick.isDone()) {
			handleTrick();
			//Just get hand size of any player
			if (players[0].getHand().Count == 0) {
				//finish
				endRound();
				return true;
			}
			newTrick(currentTrick.getWinner());
		} else {
			turn = (turn + 1) % NUM_PLAYERS;
			requestPlay();
		}
		return true;
	}

	public void receiveBid(Player player, Suit s = Suit.SUIT_INVALID) {
		if (state != GameState.DEAL) {
			Logger.logMessage("wdf can't receive bid when not dealing");
			return;
		}
		if (s != Suit.SUIT_INVALID) {
			List<Card> bidCards = player.getBidCards(s);
			Bid bid = new Bid(player, bidCards);
			setBid(player, bid);
		} else {
			setBid(player, null);
		}

		dealCard();
	}

	public int getPlayerIndex(Player player) {
		for (int i = 0; i < NUM_PLAYERS; i++) {
			if (players[i] == player) {
				return i;
			}
		}
		return -1;
	}
	public Player getDealer() {
		return dealer;
	}
	public Bid getLargestBid() {
		return largestBid;
	}
	public Trick getCurrentTrick() {
		return currentTrick;
	}

	public Trick getLastTrick() {
		if (wonTricks.Count > 0) {
			return wonTricks[wonTricks.Count - 1];
		}
		return null;
	}

	public int getAttackerPoints() {
		int attackerPoints = 0;

		//double won kitty last trick pts.

		foreach (Player player in attackers) {
			attackerPoints += player.getPoints();
		}
		return attackerPoints;
	}

	public int getDefenderPoints() {
		int defenderPoints = 0;

		//double won kitty last trick pts.

		foreach (Player player in defenders) {
			defenderPoints += player.getPoints();
		}
		return defenderPoints;
	}

	public List<Card> getKitty() {
		return kitty;
	}


	public int getPlayerType(Player player) {
		if (attackers.IndexOf(player) != -1) {
			return PLAYER_TYPE_ATTACKER;
		} else if (defenders.IndexOf(player) != -1) {
			return PLAYER_TYPE_DEFENDER;
		} else {
			throw new System.Exception($"WDF? this player type is unknown {getPlayerIndex(player)}");
		}
	}



	public void notifyEvent(string gameEvent) {
		foreach (ModelSub sub in observers) {
			sub.GameEventOccurred(gameEvent);
		}
	}
	public const int PLAYER_TYPE_ATTACKER = 1;
	public const int PLAYER_TYPE_UNKNOWN = 0;
	public const int PLAYER_TYPE_DEFENDER = 2;


	//TODO change to kitty?
	public void receiveBury(List<Card> bury) {
		if (state != GameState.KITTY) {
			Logger.logMessage("wdf cant recieve bury during non kitty decl state");
		}
		Logger.logMessage($"{dealer.getName()} attempted to bury {CardUtils.getCardListString(bury)}");
		//Validate player maybe..
		if (bury.Count != KITTY_SIZE) {
			Logger.logMessage("Invalid bury received");
			requestKitty();
			return;
		}

		dealer.removeCards(bury);
		notifyEvent(GameEvents.KITTY_DEPOSITED);

		kitty = bury;
		logGameState();

		beginPlay();
	}

	private void beginPlay() {
		// begin play
		setGameState(GameState.PLAY);
		newTrick(getPlayerIndex(dealer));
	}

	public string cardsToString(List<Card> cards) {
		string str = "";
		foreach (Card card in cards) {
			str = str + " " + card.ToString();
		}
		return str;
	}

	private void notifyBidUpdate() {
		foreach (ModelSub sub in observers) {
			sub.GameEventOccurred(GameEvents.BID_UPDATE);
		}
	}
	private List<ModelSub> observers = new List<ModelSub>();
	private Player[] players;

	private int NUM_PLAYERS;
	private int KITTY_SIZE;

	//Dealing business
	private int lastWinner;
	private Player dealer;
	private int currentBidder;
	private List<Card> deck = new List<Card>();
	private int currentDeal;
	private Bid largestBid;
	private List<Card> kitty = new List<Card>(); // Also used for last trick calc.


	//Round Details
	private Suit trumpSuit;
	private Rank trumpRank;
	private List<Player> attackers = new List<Player>();
	private List<Player> defenders = new List<Player>();
	
	//Trick details
	private Trick currentTrick;

	private List<Trick> wonTricks = new List<Trick>();
	private int turn;


	private void shuffleDeck() {
		for (int i = 0; i < deck.Count; i++) {
			Card c = deck[i];
			int randomIndex = CppUtils.randInt(i, deck.Count);
			deck[i] = deck[randomIndex];
			deck[randomIndex] = c;
		}
	}

	private void createKitty() {
		kitty.Clear();
		for (int i = 0; i < KITTY_SIZE; i++) {
			kitty.Add(deck[deck.Count - 1]);
			deck.RemoveAt(deck.Count - 1);

		}
	}

	private void notifyHumanInput(string s) {
		foreach (ModelSub sub in observers) {
			sub.NotifyHumanInput(s);
		}	
	}
	
	private void dealCard(bool skipInc = false) {
		if (deck.Count > 0) {
			if (!skipInc) {
				currentDeal = (currentDeal + 1) % NUM_PLAYERS;
			}
			Player player = players[currentDeal];
			Card topDeck = deck[deck.Count - 1];

			deck.RemoveAt(deck.Count - 1);
			player.addCard(topDeck);
			player.requestPlayerBid();
			if (player.getType() == Player.HUMAN) {
				notifyHumanInput(GameEvents.HUMAN_INPUT_BID);
			}
		} else {
			//Finish bidding
			if (largestBid != null) {
				trumpSuit = largestBid.getSuit();
			} else {
				trumpSuit = Suit.NO_TRUMP;
			}
			// INIT CardUtils

			CardUtils.setTrumpDetails(trumpRank, trumpSuit);
			handleEndBid();
			handleKitty();
			notifyEvent(GameEvents.BID_END);
			//requestPlay();
		}
		notifyEvent(GameEvents.HAND_UPDATE);
		logGameState();
	}

	private void handleEndBid() {
		for (int i = 0; i < NUM_PLAYERS; i++) {
			// Player player = players[i];
			players[i].endBid(trumpSuit);
		}
	}

	private void handleKitty() {
		dealer.addKitty(kitty);
		requestKitty();
	}

	private void requestKitty() {
		setGameState(GameState.KITTY);
		dealer.respondKitty(KITTY_SIZE);
		if (dealer.getType() == Player.HUMAN) {
			notifyEvent(GameEvents.KITTY_DEPOSIT);
		}
	}


	private void setBid(Player player, Bid bid) {
		//TODO check bid > largestBid (pair etc. & set if possible)
		if (bid != null && bid.isValid()) {
			if (trumpRank == bid.getRank() || bid.getRank() == Rank.JOKER_UNC || bid.getRank() == Rank.JOKER_COL) {
				if (largestBid == null) {
					largestBid = bid;
					notifyBidUpdate();
					return;
				} else {
					if (largestBid.getSuit() != Suit.NO_TRUMP) {
						// NT overrides no matter what - Hand will req atleast 2 copies?
						if (bid.getSuit() == Suit.NO_TRUMP) {
							largestBid = bid;
							notifyBidUpdate();
							return;
						} else {
							// Choose based on copies
							if (largestBid.getNumCopies() < bid.getNumCopies()) {
								largestBid = bid;
								notifyBidUpdate();
								return;
							} else {
								Logger.logMessage("You need more copies than the previous bid.");
							}
						}
					} else {
						//Can't override NT
						Logger.logMessage("Can't overbid NT");
					}
				}
			} else {
				//?.... cant bid iwth this
				Logger.logMessage("Cant bid with this.");
			}
		} else {
			Logger.logMessage("Invalid bid?");
		}
	}

	private void newTrick(int starter) {
		turn = starter;
		currentTrick = new Trick(this, starter);
//        Logger.logMessage("NEW TRICK CREATED");
//        Logger.logMessage($"islead {currentTrick.isLead()}");
		logGameState();
		requestPlay();
	}

	private void handleTrick() {
		int pts = currentTrick.getTotalPoints();
		int winnerIndex = currentTrick.getWinner();
		Logger.logMessage("Player " + winnerIndex + " WINS THIS TRICK + worth " + pts);
		getPlayer(winnerIndex).addPoints(pts);
		wonTricks.Add(currentTrick);
		notifyEvent(GameEvents.TRICK_END);
		// if (getPlayer(winnerIndex).hasAnyCards())
		// {
		// 	newTrick(winnerIndex);
		// }
		// else
		// {
		// 	endRound();
		// }
	}

	//
	//void Game::returnKitty(List<Card > cards)
	//{
	//	if (cards.Count == KITTY_SIZE)
	//	{
	//		//too lazy to verify;
	//		kitty = cards;
	//		newTrick(getPlayerIndex(dealer));
	//	}
	//}

	private bool isHumanPlayer(Player p) {
		return p.getType() == Player.HUMAN;
	}


	private void requestPlay() {
		//use turn
		Player currentPlayer = getPlayer(turn);

		// Logger.logMessage("REQUESTING PLAY" + getPlayerIndex(currentPlayer));
		if (currentPlayer.getType() == Player.HUMAN) {
			//Notify & return
			notifyEvent(GameEvents.PLAY_REQUEST);

		} else {
			currentPlayer.requestMove();
		}
	}



	//Ret valid play or not
	private bool isValidFirstPlay(Player originPlayer, List<Card> cards) {
		PlayType playType = new PlayType(cards);
		if (CardUtils.isAllSameSuit(cards)) {
			if (playType.isDisjoint()) {

				Logger.logMessage("DISJOINT");
				Suit expectedSuit = playType.getSuit();
				for (int i = 0; i < NUM_PLAYERS; i++) {
					Player player = players[i];
					if (player == originPlayer) {
						continue;
					} else {
						List<Card> suitCards = player.getCardsOfSuit(expectedSuit);
						if (playType.hasLargerUnit(suitCards)) {
							Logger.logMessage("Can't play this disjoint, someone has large TODO remove 10pt? + play smallest");
							return false;
						}
					}
				}
			}
			currentTrick.setPlayType(playType);
			return true;
		} else {
			Logger.logMessage("all cards must be same suit");
			return false;
		}
	}

	//Ret valid play or not
	private bool isValidFollowTrick(Player originPlayer, List<Card> cards) {
		PlayType trickPlayType = currentTrick.getPlayType();

		PlayType playType = new PlayType(cards);

		Suit expectedSuit = trickPlayType.getSuit();
		List<Card> suitCards = originPlayer.getCardsOfSuit(expectedSuit);

		// must be same size as current play.
		if (cards.Count != currentTrick.GetPlayLength()) {
			Logger.logMessage($"DIFF play length {cards.Count} {currentTrick.GetPlayLength()}");
			return false;
		}

		//Player cant follow suit, can play whatever
		if (suitCards.Count == 0) {
			//Can play whatever
			return true;
		}

		//Player can partially follow suit, needs to dump rest of suit
		if (suitCards.Count <= trickPlayType.getSize()) {
			//Barfed out rest of suit
			if (playType.getNumCardsOfSuit(expectedSuit) == suitCards.Count) {
				return true;
			} else {
				//You need to dump the rest of your suit.
				return false;
			}
		}

		//Player must follow suit/rules
		if (CardUtils.isAllSameSuit(cards) && playType.getSuit() == trickPlayType.getSuit()) {
			//Wait do i need to follow tractor if possible? lol too much work
			//TODO FIX THIS if need to follow tradctor
			int pairsExpected = trickPlayType.findPairs().Count;
			int pairsPlayed = playType.findPairs().Count; //#pairs played
			int pairsPossible = originPlayer.getPairsOfSuit(expectedSuit).Count; //#pairs possibly played
			if (pairsPlayed < pairsExpected && pairsPossible != pairsPlayed) {
				return false;
			}
		} else {
			//You didnt even try to follow suit, when you must've
			return false;
		}
		return true;
	}

	private void endRound() {

		Logger.logMessage("Round over?");
	
		int defenderPoints = 0;

		foreach (Player player in defenders) {
			defenderPoints += player.getPoints();
		}
		Logger.logMessage($"Defender points: {defenderPoints}");

		Trick lastTrick = wonTricks[wonTricks.Count - 1];
		int winnerIndex = lastTrick.getWinner();
		Player lastTrickWinner = getPlayer(winnerIndex);
		int kittyPoints = 0;
		foreach (Card card in kitty) {
			kittyPoints += card.getPointValue() * 2;
		}
		Logger.logMessage($" Kitty Contained: {CardUtils.getCardListString(kitty)}, worth {kittyPoints}. ");

		if (getPlayerType(lastTrickWinner) != PLAYER_TYPE_ATTACKER) {
			defenderPoints += kittyPoints;
		}
		Logger.logMessage($"Defender points: {defenderPoints}");

		bool attackersWin = false;
		if (defenderPoints >= 200) {
			//+3
			foreach (Player player in defenders) {
				player.levelUp(3);
			}
		} else if (defenderPoints >= 160) {
			//+2
			foreach (Player player in defenders) {
				player.levelUp(2);
			}
		} else if (defenderPoints >= 120) {
			//+1
			foreach (Player player in defenders) {
				player.levelUp(1);
			}
		} else if (defenderPoints >= 80) {
			//NO CHANGE
		} else if (defenderPoints >= 40) {
			attackersWin = true;
			//-1
			foreach (Player player in attackers) {
				player.levelUp(1);
			}
		} else if (defenderPoints > 0) {
			attackersWin = true;
			//-2
			foreach (Player player in attackers) {
				player.levelUp(2);
			}
		} else if (defenderPoints == 0) {
			attackersWin = true;
			//-3
			foreach (Player player in attackers) {
				player.levelUp(3);
			}
		}
		List<Player> winners = new List<Player>();
		foreach (Player player in players) {
			if (player.getCurrentLevel() == Rank.JOKER_COL || player.getCurrentLevel() == Rank.JOKER_UNC) {
				winners.Add(player);
			}
		}
		if (winners.Count >= 1) {
			endGame(winners);
			return;
		}
		

		//Setting next dealer.
		if (attackersWin) {
			lastWinner = getPlayerIndex(attackers[1]);
			Logger.logMessage($"attackers won, switching last winner to {lastWinner}");
		} else {
			lastWinner = (getPlayerIndex(dealer) + 1) % NUM_PLAYERS;
			Logger.logMessage($"defenders won, switching last winner to {lastWinner}");
		}
		setGameState(GameState.PRE_DEAL);
	}
	private void endGame(List<Player> winners) {
		setGameState(GameState.END);
		Logger.logMessage("game over, players won");
		foreach( Player player in winners) {
			Logger.logMessage(player.getName());
		}
	}
	public void logGameState() {
		int index = 0;
		// foreach (Player player in players) {
		// 	Logger.logMessage($"{player.getName()}: {player.getHandMessage()} points:{player.getPoints()} type:{getPlayerType(player)} level:{player.getCurrentLevel()}");
		// 	index++;
		// }
		if (state == GameState.DEAL) {
			Logger.logMessage($"dealing, {getTrumpRank()}");
			if (largestBid != null) {
				Logger.logMessage(largestBid.ToString());
			} else {
				Logger.logMessage("Bid: none");
			}
		} else if (state == GameState.KITTY) {
			Logger.logMessage("kitty selection");
			if (kitty != null) {
				Logger.logMessage($"Kitty: {CardUtils.getCardListString(kitty)}");
			} else {
				Logger.logMessage("Kitty: None");
			}
		} else if (state == GameState.PLAY) {
			if (currentTrick != null) {
				Logger.logMessage(currentTrick.ToString());
			}
		} else if (state == GameState.PRE_DEAL) {
			Logger.logMessage("wait for new round call");
		}

	}

	private void setGameState(GameState s) {
		this.state = s;
		foreach (ModelSub sub in this.observers) {
			sub.GameStateChanged(s);
		}
	}




}
