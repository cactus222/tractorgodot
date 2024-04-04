using System.Collections.Generic;
public class CardUtils {

	private static Suit trumpSuit;
	private static Rank trumpRank;

	private static Dictionary<Rank, int> nonTrumpOrder;
	private static Dictionary<Card, int> trumpOrder;

	private static CardComparator comparer;

	public static CardComparator getComparer() {
		return comparer;
	}

	public static Dictionary<Rank, int> getNonTrumpOrder() {
		return nonTrumpOrder;
	}

	public static Dictionary<Card, int> getTrumpOrder() {
		return trumpOrder;
	}

	public static void setTrumpDetails(Rank trank, Suit tsuit) {
		trumpRank = trank;
		trumpSuit = tsuit;
		initOrderDictionaries();
		comparer = new CardComparator(trumpRank, trumpSuit);
	}

	private static void initOrderDictionaries() {
		//init 
		nonTrumpOrder = new Dictionary<Rank, int>();
		int counter = 0;
		foreach (KeyValuePair<Rank, int> kvp in GlobalMembers.RANK_MAPPING) {
			if (kvp.Key == trumpRank) {
				continue;
			}
			nonTrumpOrder.Add(kvp.Key, counter);
			counter++;
		}

		trumpOrder = new Dictionary<Card, int>();
		counter = 200;
		if (trumpSuit != Suit.NO_TRUMP) {
			foreach (KeyValuePair<Rank, int> kvp in GlobalMembers.RANK_MAPPING) {
				if (kvp.Key == trumpRank) {
					continue;
				}
				trumpOrder.Add(new Card(kvp.Key, trumpSuit), counter);
				counter++;
			}
		   
		}

		//all of trump rank but not trumpsuit is same 
		foreach(Suit s in GlobalMembers.BASIC_SUITS) {
			if (s != trumpSuit) {
				trumpOrder.Add(new Card(trumpRank, s), counter);
			}
		}
		counter++;
		//trumprank + trumpsuit is larger
		if (trumpSuit != Suit.NO_TRUMP) {
			trumpOrder.Add(new Card(trumpRank, trumpSuit), counter);
			counter++;
		}
		trumpOrder.Add(CardUtils.generateJoker(Rank.JOKER_UNC), counter);
		counter++;
		trumpOrder.Add(CardUtils.generateJoker(Rank.JOKER_COL), counter);
		counter++;
	}


	public static bool isAdjacentRank(Card card1, Card card2) {
		Suit suit = getSuitFactorTrump(card1);
		if (suit != getSuitFactorTrump(card2)) {
			return false;
		}

		if (suit == Suit.TRUMP) {
			int diff = trumpOrder[card1] - trumpOrder[card2];
			return (diff == 1) || (diff == -1);
		} else {
			int order1 = nonTrumpOrder[card1.getRank()];
			int order2 = nonTrumpOrder[card2.getRank()];
			int diff = order1 - order2;
			return (diff == 1) || (diff == -1);
		}
	}

//Cards incoing are pairs as singles
//cards need to be sorted highest to lowest before passing. output indices represent start of tractors that equal the expected tractor size.
//i dont remember what cardutiltypeis

	public static List<int> getTractorIndices(int expectedTractorSize, List<Card> cards, int cardUtilType) {

		List<int> tractorIndices = new List<int>();

		if (cardUtilType == CardUtils.CARD_UTIL_TYPE_SINGLES_AS_PAIRS) {

			CardUtils.sort(cards);

			List<Card> tractor = new List<Card>();
			if (cards.Count < expectedTractorSize) {
				return tractorIndices;
			}

			bool hasTractor = false;
			Card currentCard = cards[0];
			int currentTractorLength = 1;
			for (int i = 1; i < cards.Count; i++) {
				Card nextCard = cards[i];
				if (CardUtils.isAdjacentRank(currentCard, nextCard)) {
					hasTractor = true;
					currentTractorLength++;
					if (currentTractorLength >= expectedTractorSize) {
						tractorIndices.Add(i - expectedTractorSize + 1); // Ex if 0, 1 is tractor expected size = 2, we want to push 0
					}
				} else {
					if (hasTractor) {
						hasTractor = false;
						currentTractorLength = 1;
					}
				}
				currentCard = nextCard;
			}

		}
		return tractorIndices;
	}

	public static bool isAllSameSuit(List<Card> cards) {
		Suit s = CardUtils.getSuitFactorTrump(cards[0]);

		for (int i = 1; i < cards.Count; i++) {
			if (CardUtils.getSuitFactorTrump(cards[i]) != s) {
				return false;
			}
		}
		return true;
	}
	public static void sort(List<Card> cards) {
		cards.Sort(CardUtils.Compare);
	}
	public static int Compare(Card card1, Card card2) {
		return comparer.Compare(card1, card2);
	}

	public static Suit getSuitFactorTrump(Card c) {
		Suit s = c.getSuit();
		if (s == Suit.JOKER) {
			return Suit.TRUMP;
		}

		if (c.getRank() == trumpRank || c.getSuit() == trumpSuit) {
			return Suit.TRUMP;
		}
		return s;
	}

//returns list of singles that are paired
	public static List<Card> findPairs(List<Card> cards) {

		HashSet<Card> singles = new HashSet<Card>();
		List<Card> pairedCards = new List<Card>();

		for (int i = 0; i < cards.Count; i++) {
			if (singles.Contains(cards[i])) {
				pairedCards.Add(cards[i]);
			} else {
				singles.Add(cards[i]);
			}
		}

		return pairedCards;
	}

	public static Card generateJoker(Rank jokerRank) {
		if (jokerRank != Rank.JOKER_COL && jokerRank != Rank.JOKER_UNC) {
			throw new System.Exception("cannot create this card");
		}
		return new Card(jokerRank, Suit.JOKER);
	}
	//no jokers, todo dowhile :eyes:
	public static Card generateRandomSuitCard() {
		int index = CppUtils.randInt(0, GlobalMembers.BASIC_SUITS.Length);
		while (GlobalMembers.BASIC_SUITS[index] == trumpSuit) {
			index = CppUtils.randInt(0, GlobalMembers.BASIC_SUITS.Length);
		}
		Suit s = GlobalMembers.BASIC_SUITS[index];
		List<Rank> ranks = new List<Rank>(GlobalMembers.RANK_MAPPING.Keys);
		index = CppUtils.randInt(0, ranks.Count);
		while (ranks[index] == trumpRank) {
			index = CppUtils.randInt(0, ranks.Count);
		}
		Rank r = ranks[index];
		return new Card(r, s);
	}

	public static int getNumPoints(List<Card> cards) {
		int pts = 0;
		for (int i = 0; i < cards.Count; i++) {
			pts += cards[i].getPointValue();
		}
		return pts;
	}

	public static readonly int CARD_UTIL_TYPE_SINGLES_AS_PAIRS = 0;
	public static readonly int CARD_UTIL_TYPE_MISC = 1;
	public static int getTotalPoints(List<Card> cards) {
		int sum = 0;

		foreach (Card card in cards) {
			sum += card.getPointValue();
		}

		return sum;
	}
	public static List<List<Card>> allPermutations(int size, List<Card> cards) {
		//TODO
		return new List<List<Card>>();
	}

		//TODO MOVE TO SOME UTILS
	public static string getCardListString(List<Card> cards) {
		string msg = "";
		foreach (Card card in cards) {
			msg += card.ToString() + " ";
		}
		return msg;
	}
}
