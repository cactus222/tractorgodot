using System.Collections.Generic;


public class CardComparator : IComparer<Card> {
	private Rank trumpRank;
	private Suit trumpSuit;
	private bool reverse;
	public CardComparator(Rank r, Suit s) : this(r, s, false) {

	}
	public CardComparator(Rank r, Suit s, bool reverse) {
		this.trumpRank = r;
		this.trumpSuit = s;
		this.reverse = reverse;

	}

	public int mod(int original) {
		if (reverse) {
			return -original;
		}
		return original;

	}
	// 1 if card 1 > card 2
	//-1 if card 2 > card 1
	// 0 if whoever played first is larger.

	public int Compare(Card card1, Card card2) {

		Suit suit1 = CardUtils.getSuitFactorTrump(card1);
		Suit suit2 = CardUtils.getSuitFactorTrump(card2);
		Rank rank1 = card1.getRank();
		Rank rank2 = card2.getRank();

		bool isSameSuit = suit1 == suit2;
		bool isTrump = suit1 == Suit.TRUMP;
		bool isSameRank = rank1 == rank2;

		if (isSameSuit) {
			if (suit1 == Suit.TRUMP) {
				Dictionary<Card, int> mapping = CardUtils.getTrumpOrder();
				return mapping[card1].CompareTo(mapping[card2]);
			} else {
				Dictionary<Rank, int> mapping = CardUtils.getNonTrumpOrder();
				int size1 = mapping[rank1];
				int size2 = mapping[rank2];
				return size1.CompareTo(size2);
			}
		} else {
			if (suit2 == Suit.TRUMP) {
				return -1;
			} else if (suit1 == Suit.TRUMP) {
				return 1;
			}
			//else doesn't matter
			return 0;
		}
	}
}
