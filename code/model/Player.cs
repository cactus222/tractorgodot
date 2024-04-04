using System.Collections.Generic;

public abstract class Player {

	public const int HUMAN = 0;
	public const int COMPUTER = 1;


	public Player(Game game) {
		this.game = game;
		this.level = Rank.TWO;
		hand = new Hand();
	}

	public void setName(string name) {
		this.name = name;
	}
	public string getName() {
		return this.name;
	}

	public void levelUp(int numRanks) {
		int rankVal = GlobalMembers.RANK_MAPPING[level];
		rankVal += numRanks;
		if (!GlobalMembers.INVERSE_RANK_MAPPING.ContainsKey(rankVal)) {
			rankVal = GlobalMembers.RANK_MAPPING_WITH_JOKERS[Rank.JOKER_COL];
		}
		level = GlobalMembers.INVERSE_RANK_MAPPING[rankVal];
	}
	public void endBid(Suit s) {
		hand.setTrumpSuit(s);
	}
	public Rank getCurrentLevel() {
		return level;
	}

	public bool hasCards(List<Card>cards) {
		return hand.hasCards(cards);
	}
	public void removeCards(List<Card> cards) {
		hand.removeCards(cards);
	}
	public void addPoints(int pts) {
		points += pts;
	}
	public void addCard(Card card) {
		//TODO sort when insert?
		hand.addCard(card);
	}
	public int getPoints() {
		return points;
	}
	public List<Card> getBidCards(Suit s) {
		return hand.getBidCards(s);
	}
	public Hand getHandObj() {
		return hand;
	}
	public List<Card> getHand() {
		return hand.getCards();
	}
	public List<Card> getPairsOfSuit(Suit s) {
		return hand.getPairsOfSuit(s);
	}
	public void newRound(Rank trumpRank) {
		resetPoints();
		resetHand(trumpRank);
	}
	public void resetPoints() {
		points = 0;
	}
	public void resetHand(Rank trumpRank) {
		hand.reset(trumpRank);
	}
	public abstract int getType();
	public List<Card> getCardsOfSuit(Suit s) {
		return hand.getCardsOfSuit(s);
	}
	public void addKitty(List<Card> kitty) {
		hand.addCards(kitty);
	}

	public int getHandSize() {
		return hand.getSize();
	}

	public abstract void respondKitty(int size);
	public abstract void requestMove();
	public abstract void requestPlayerBid();
	public bool hasAnyCards() {
		return hand.hasAnyCards();
	}
	public List<Suit> getPossibleBids() {
		return hand.getPossibleBids();
	}

	public string getHandMessage() {
		return hand.ToString();
	}

	public int getCurrentRoundPlayerType() {
		return game.getPlayerType(this);
	}

	protected Game game;
	protected Hand hand;
	protected Rank level;
	protected int points;

	protected string name;

	//TODO HM??
	//	void sortHand();




}


