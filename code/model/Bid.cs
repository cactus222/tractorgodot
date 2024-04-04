using System.Collections.Generic;

//include pass logic
public class Bid {
	private Player player;
	private List<Card> cards = new List<Card>();
	public Bid(Player player, List<Card> cards) {
		this.player = player;
		this.cards = cards;
	}

	public Suit getSuit() {
		if (cards[0].getRank() == Rank.JOKER_UNC || cards[0].getRank() == Rank.JOKER_COL) {
			return Suit.NO_TRUMP;
		}
		return cards[0].getSuit();
	}
	public int getNumCopies() {
		return cards.Count;
	}
	public bool isValid() {
		if (cards.Count == 0 || cards.Count > 2) {
			return false;
		}

		Suit s = cards[0].getSuit();
		for (int i = 1; i < cards.Count; i++) {
			if (cards[i].getSuit() != s) {
				return false;
			}
		}
		return true;
	}
	public Rank getRank() {
		return cards[0].getRank();
	}
	public Player getPlayer() {
		return player;
	}

	public override string ToString() {
		return $"{player.getName()} Rank: {getRank()} Suit: {getSuit()} Copies:{getNumCopies()}";
	}
	//Suit,
	//Rank,
	//NT needs same colour jokers + 2of
}
