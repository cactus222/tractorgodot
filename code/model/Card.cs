using System.Collections;

public class Card {
	public Card(Rank r, Suit s) {
  
		this.rank = r;
		this.suit = s;
	}
	public Card(Suit s, Rank r) {

		this.rank = r;
		this.suit = s;
	}

	public Suit getSuit() {
		return suit;
	}

	public Rank getRank() {
		return rank;
	}
	public int getPointValue() {
		if (rank == Rank.FIVE) {
			return 5;
		} else if (rank == Rank.KING || rank == Rank.TEN) {
			return 10;
		}
		return 0;
	}

	public override bool Equals(object o) {
		if (o == null || !this.GetType().Equals(o.GetType())) {
			return false;
		}

		Card c = (Card)o;
		return c.getSuit() == suit && c.getRank() == rank;
	}

	public override int GetHashCode() {
		unchecked {
			int hash = 17;
			// Suitable nullity checks etc, of course :)
			hash = hash * 23 + rank.GetHashCode();
			hash = hash * 23 + suit.GetHashCode();
			return hash;
		}
	}


	public override string ToString() {

		return GlobalMembers.RANK_STRING_MAPPING[rank] + 
		GlobalMembers.SUIT_STRING_MAPPING[suit][0];
	}
	private Suit suit;
	private Rank rank;


}
