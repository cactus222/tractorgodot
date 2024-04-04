using System.Collections.Generic;
public class Hand {

	public Dictionary<Suit, List<Card>> cardMappings = new Dictionary<Suit, List<Card>>();
	public List<Card> pairedCards = new List<Card>();
	public Rank trumpRank;
	public Suit trumpSuit;

	public Hand() {
		//init card mappings
		//iter suits and init

		cardMappings[Suit.DIAMONDS] = new List<Card>();
		cardMappings[Suit.CLUBS] = new List<Card>();
		cardMappings[Suit.HEARTS] = new List<Card>();
		cardMappings[Suit.SPADES] = new List<Card>();
		cardMappings[Suit.TRUMP] = new List<Card>();
	}
	public Card getHighCard(bool ignorePoints, bool ignoreTrump) {
		//Get
		//TODO
		throw new System.Exception("TODO");
		return null;
	}
	// use kings if ace is trump rank
	public List<Card> findAcePlay(bool ignoreTrump) {

		Rank targetRank = Rank.ACE;
		if (trumpRank == Rank.ACE) {
			targetRank = Rank.KING;
		}
		//Iterate over 4 suits, ignore trump
		foreach (Suit s in GlobalMembers.BASIC_SUITS) {
			if (ignoreTrump && s == trumpSuit) {
				continue;
			}
			Card ace = new Card(s, targetRank);
			List<Card> acesOfSuit = getCopiesOfCard(ace);

			if (acesOfSuit.Count > 0) {
				return acesOfSuit;
			}
		}
		return new List<Card>();

	}

	public int getNumCardsOfSuit(Suit s) {
		return cardMappings[s].Count;
	}

	//Hand must always know trumprank, trumpsuit may be ???.
	public void reset(Rank r) {
		trumpSuit = Suit.SUIT_INVALID;
		trumpRank = r;
	}
	public void addCard(Card c) {
		Suit s = CardUtils.getSuitFactorTrump(c);
		if (!cardMappings.ContainsKey(s)) {
			throw new System.Exception(c.ToString());
		}
		List<Card> cardsOfSuit = cardMappings[s];

		for (int i = 0; i < cardsOfSuit.Count; i++) {
			if (c.Equals(cardsOfSuit[i])) {
				pairedCards.Add(c);
				break;
			}
		}

		cardsOfSuit.Add(c);
		if (s == Suit.TRUMP) {
			//gotta sort those same ranks or else it becomes weird 2H 2C 2H???
			cardsOfSuit.Sort(Comparer<Card>.Create( (a, b) => { 
				var regularCompare = CardUtils.Compare(a,b);
				if (regularCompare == 0) {
					return GlobalMembers.SUIT_MAPPING_WITH_JOKERS[a.getSuit()].CompareTo(GlobalMembers.SUIT_MAPPING_WITH_JOKERS[b.getSuit()]);
				}
				return regularCompare;
		}));          
		} else {
			cardsOfSuit.Sort(CardUtils.getComparer());
		}
		cardMappings[s] = cardsOfSuit;

	}
	public void addCards(List<Card> cards) {
		foreach (Card c in cards) {
			addCard(c);
		}
	}
	public void removeCards(List<Card> cards) {
		foreach (Card c in cards) {
			removeCard(c);
		}
	}

	//TODO DO BETTER., also only has 2 copy handling
	public bool hasCards(List<Card> cards) {
		HashSet<Card> seenCards = new HashSet<Card>();
		foreach (Card c in cards) {
			if (seenCards.Contains(c)) {
				if(!pairedCards.Contains(c)) {
					return false;
				}
			} else {
				if(getNumCopiesOfCard(c) == 0) {
					return false;
				}
				seenCards.Add(c);
			}
		}
		return true;
	}

	public List<Card> getCardsOfSuit(Suit s) {
		return new List<Card>(cardMappings[s]);
	}
	public void removeCard(Card c) {
		//create card comparator here
		Suit s = CardUtils.getSuitFactorTrump(c);
		List<Card> cardsOfSuit = cardMappings[s];

		//Remove its not paired anymore if it is.
		CppUtils.remove(pairedCards, c);
		if (!CppUtils.remove(cardsOfSuit, c)) {
			
			throw new System.Exception($"WDF COULDNT REMOVE THIS CARD!; {c.ToString()}");
		}

		cardMappings[s] = cardsOfSuit;
	}
	public Dictionary<Suit, List<Card>> getBiddableSuits() {
		Dictionary<Suit, List<Card>> biddableSuits = new Dictionary<Suit, List<Card>>();

		foreach (Card c in cardMappings[Suit.TRUMP]) {
			if (c.getRank() == trumpRank) {
				biddableSuits[c.getSuit()] = getCopiesOfCard(c);
			}
		}

		Card jokerUnc = CardUtils.generateJoker(Rank.JOKER_UNC);
		Card jokerCol = CardUtils.generateJoker(Rank.JOKER_COL);

		if (hasPair(jokerCol)) {
			biddableSuits[Suit.NO_TRUMP] = getCopiesOfCard(jokerCol);
		} else if (hasPair(jokerUnc)) {
			biddableSuits[Suit.NO_TRUMP] = getCopiesOfCard(jokerUnc);
		}

		return biddableSuits;
	}
	public List<Card> getCopiesOfCard(Card c) {
		List<Card> cardsInHandRet = new List<Card>();
		//ex pass in 2S, get 2S 2S
		Suit s = CardUtils.getSuitFactorTrump(c);
		foreach (Card cardInHand in cardMappings[s]) {
			if (cardInHand.Equals(c)) {
				cardsInHandRet.Add(cardInHand);
			}
		}
		return cardsInHandRet;
	}
	public bool hasPair(Card c) {
		return getNumCopiesOfCard(c) == 2;
	}
	public int getNumCopiesOfCard(Card c) {
		return getCopiesOfCard(c).Count;
	}
	public void setTrumpRank(Rank r) {
		trumpRank = r;
	}
	// Move cards from suit to trump vctor
	public void setTrumpSuit(Suit s) {
		trumpSuit = s;
		if (trumpSuit != Suit.NO_TRUMP) {
			List<Card> newTrumpCards = cardMappings[s];
			newTrumpCards.AddRange(cardMappings[Suit.TRUMP]);
			newTrumpCards.Sort(CardUtils.getComparer());
			cardMappings[Suit.TRUMP] = newTrumpCards;
			cardMappings[s] = new List<Card>();
		}
	}
	public List<Suit> getPossibleBids() {
		Card redJoker = new Card(Suit.NO_TRUMP, Rank.JOKER_COL);
		Card blackJoker = new Card(Suit.NO_TRUMP, Rank.JOKER_UNC);

		List<Suit> suits = new List<Suit>();
		if (getNumCopiesOfCard(redJoker) > 1 || getNumCopiesOfCard(blackJoker) > 1) {
			suits.Add(Suit.NO_TRUMP);
		}

		foreach (Suit s in GlobalMembers.BASIC_SUITS) {
			Card trumpCard = new Card(s, trumpRank);
			if (getNumCopiesOfCard(trumpCard) > 0) {
				suits.Add(s);
			}
		}

		return suits;
	}
	public List<Card> getPairsOfSuit(Suit s) {
		List<Card> pairsInSuit = new List<Card>();

		foreach (Card card in pairedCards) {
			if (CardUtils.getSuitFactorTrump(card) == s) {
				pairsInSuit.Add(card);
			}
		}
		return pairsInSuit;
	}
	public int getSize() {
		int total = 0;
		foreach (KeyValuePair<Suit, List<Card>> kvp in cardMappings) {
			total += kvp.Value.Count;
		}
		return total;
	}
	public List<Card> getCards() {
		List<Card> cards = new List<Card>();
		foreach (Suit s in GlobalMembers.BASIC_SUITS_WITH_TRUMP) {
			CppUtils.addAll(cards, cardMappings[s]);
		}
		return cards;
	}

	public int getNumCardsOfRank(Rank r) {
		int count = 0;
		List<Card> cards = getCards();
		foreach (Card c in cards) {
			if (c.getRank() == r) {
				count++;
			}
		}
		return count;
	}


	//Find first tractor of non trump
	public List<Card> findTractorPlay(bool ignoreTrump) {
		//TODO WHEN SORTING CONSIDER SUITS.

		Dictionary<Suit, List<Card>> pairsBySuit = new Dictionary<Suit, List<Card>>();
		foreach (Card c in pairedCards) {
			Suit s = CardUtils.getSuitFactorTrump(c);
			if (pairsBySuit.ContainsKey(s)) {
				List<Card> cards = pairsBySuit[s];
				cards.Add(c);
				pairsBySuit[s] = cards;
			} else {
				pairsBySuit[s] = new List<Card>(){c};
			}
		}

		foreach (KeyValuePair<Suit, List<Card>> kvp in pairsBySuit) {
			List<Card> cards = kvp.Value;
			if (cards.Count < 2) {
				continue;
			}
			cards.Sort(CardUtils.getComparer());
			bool hasTractor = false;
			Card currentCard = cards[0];
			List<Card> tractor = new List<Card>();
			for (int i = 1; i < cards.Count; i++) {
				Card nextCard = cards[i];
				if (ignoreTrump && CardUtils.getSuitFactorTrump(nextCard) == Suit.TRUMP) {
					return tractor;
				}
				if (CardUtils.isAdjacentRank(currentCard, nextCard)) {
					if (hasTractor) {
						CppUtils.addAll(tractor, getCopiesOfCard(nextCard));
					} else {
						hasTractor = true;
						CppUtils.addAll(tractor, getCopiesOfCard(currentCard));
						CppUtils.addAll(tractor, getCopiesOfCard(nextCard));

					}
				} else {
					if (hasTractor) {
						return tractor;
					}
				}
				currentCard = nextCard;
			}
			if (hasTractor) {
				return tractor;
			}
		}


		return new List<Card>();
	}

	//Find first tractor of non trump
	public List<Card> findHighPairPlay(bool ignoreTrump, Rank higherThanRank) {
		// MAke sure we're not on trumprank
		if (higherThanRank == trumpRank) {
			higherThanRank = GlobalMembers.INVERSE_RANK_MAPPING[GlobalMembers.RANK_MAPPING[higherThanRank] + 1];
		}
		CardComparator comp = new CardComparator(trumpRank, trumpSuit);

		Rank highestRankPair;
		Suit highestRankSuit;

		if (pairedCards.Count == 0) {
			return new List<Card>();
		}

		Card bestPair = null;


		for (int i = 0; i < pairedCards.Count; i++) {
			Card card = pairedCards[i];
			Suit s = CardUtils.getSuitFactorTrump(card);
			if (ignoreTrump && s == Suit.TRUMP) {
				continue;
			}
			if (s == Suit.TRUMP) {
				if (trumpSuit == Suit.NO_TRUMP) {
					//pick random 
					s = Suit.CLUBS;
				} else {
					s = trumpSuit;
				}
			}
			Card tempCard = new Card(higherThanRank, s);
			
			// TODO Verify
			if (comp.Compare(card, tempCard) >= 0) {
				if (bestPair == null) {
					bestPair = card;
				} else {
					//TODO verify
					if (comp.Compare(card, bestPair) >= 0) {
						bestPair = card;
					}
				}
			}

		}
		if (bestPair != null) {
			return getCopiesOfCard(bestPair);
		}
		return new List<Card>();

	}

	public Card getRandomCard() {
		List<Card> cardsInHand = getCards();
		return cardsInHand[CppUtils.randInt(0, cardsInHand.Count)];
	}

	//List<Card> burnCards(int numCardsToBurn, bool givePoints, Suit ignoreSuit);


	//need cards to ignore because it may be used in another set of the play...
	public List<Card> getBurnCards(int numCardsToBurn, bool givePoints, List<Card> ignoreCards) {
		//if (givePoints)
		//{
		//	List<Card > pointsCards;

		//	for (auto const& suitPair : SUIT_MAPPING)
		//	{
		//		Card card = new Card(Rank::TEN, suitPair.first);
		//		CppUtils::addAll(pointsCards, getCopiesOfCard(card));
		//	}

		//	for (auto const& suitPair : SUIT_MAPPING)
		//	{
		//		Card card = new Card(Rank::KING, suitPair.first);
		//		CppUtils::addAll(pointsCards, getCopiesOfCard(card));
		//	}

		//	for (auto const& suitPair : SUIT_MAPPING)
		//	{
		//		Card card = new Card(Rank::FIVE, suitPair.first);
		//		CppUtils::addAll(pointsCards, getCopiesOfCard(card));
		//	}

		//	//Filter suit
		//	for (int i = pointsCards.Count - 1; i >= 0; i--)
		//	{
		//		if(CardUtils.getSuitFactorTrump(pointsCards[i]) == ignoreSuit)
		//		{
		//			pointsCards.erase(pointsCards.begin()+i);
		//		}
		//	}

		//	if (pointsCards.Count >= numCardsToBurn)
		//	{
		//		//Return first X cards
		//		return List<Card >(pointsCards.begin(), pointsCards.begin() + numCardsToBurn);
		//	}
		//	else
		//	{
		//		//Need to find more cards to burn!
		//		for (auto const& suitPair : SUIT_MAPPING)
		//		{

		//		}
		//	}

		//}
		//else
		//{
		//	for (auto const& suitPair : SUIT_MAPPING)
		//	{

		//		List <Card > cardsOfSuit = cardMappings[suitPair.first];

		//		if (avoidPoints)
		//		{

		//		}



		//	}
		//}

		List<Card> burnCards = new List<Card>();
		foreach (KeyValuePair<Suit, int> suitPair in GlobalMembers.SUIT_MAPPING) {
			List<Card> cardsOfSuit = cardMappings[suitPair.Key];

			//Logger::logMessage("cards of suit size "  + std::to_string(cardsOfSuit.Count));
			//TODO Better logic hceck other suits first.
			/*if (cardsOfSuit.Count > numCardsToBurn)
			{
				CppUtils::addAll(burnCards, List<Card >(cardsOfSuit.begin(), cardsOfSuit.begin()+numCardsToBurn));
				for (Car)
				numCardsToBurn = 0;
			}
			else
			{
				//void this suit
				CppUtils::addAll(burnCards, cardsOfSuit);
				numCardsToBurn -= cardsOfSuit.Count;
			}
	
			if (numCardsToBurn == 0)
			{
				break;
			}
			else if (numCardsToBurn < 0)
			{
				Logger::logMessage("WDF IS HAPPENING HOW DID WE BURN SO MANY?");
			}*/
			for (int i = 0; i < cardsOfSuit.Count; i++) {
				Card card = cardsOfSuit[i];
				if (!CppUtils.contains(ignoreCards, card)) {
					burnCards.Add(card);
					numCardsToBurn--;
				}
				if (numCardsToBurn == 0) {
					return burnCards;
				}
			}
		}

		List<Card> trumpCards = cardMappings[Suit.TRUMP];
		//Burn lowest trump
		for (int i = 0; i < numCardsToBurn; i++) {
			burnCards.Add(trumpCards[i]);
		}

		return burnCards;


	}
	public bool hasAnyCards() {
		//int total = 0;
		foreach (KeyValuePair<Suit, List<Card>> it in cardMappings) {
			if (it.Value.Count > 0) {
				return true;
			}
		}
		return false;
	}
	public List<Card> getBidCards(Suit s) {
		List<Card> bidCards = new List<Card>();
		if (s == Suit.NO_TRUMP) {
			//todo ahndle more than 2 decks
			Card redJoker = CardUtils.generateJoker(Rank.JOKER_COL);
			List<Card> ans = getCopiesOfCard(redJoker);

			if (ans.Count >= 2) {
				return ans;
			}

			Card blackJoker = CardUtils.generateJoker(Rank.JOKER_UNC);
			ans = getCopiesOfCard(blackJoker);

			if (ans.Count >= 2) {
				return ans;
			}

		} else {
			Card trumpCard = new Card(s, trumpRank);

			bidCards = getCopiesOfCard(trumpCard);

		}
		return bidCards;
	}

	public override string ToString() {
		string msg = "";
		foreach (KeyValuePair<Suit, List<Card>> it in cardMappings) {
			List<Card> cards = it.Value;
			foreach (Card card in cards) {
				msg += card.ToString() + " ";
			}
		}
		return msg;
	}
}

