using System.Collections.Generic;
using System.Linq;

public class RandomComputerStrategy : ComputerStrategy {
	public List<Card> generateKitty(Game game, Computer comp, int size) {
		List<Card> cardsInHand = comp.getHand();
		List<Card> kitty = pickXRandomCards(cardsInHand, size);
		return kitty;
	}

	private List<Card> getPlayForPlayUnit(PlayUnit playUnit, List<Card> suitCards) {
		List<Card> cardsLeft = new List<Card>(suitCards);
		 if (playUnit.getMode() == PlayUnit.TRACTOR_MODE) {
			List<Card> pairs = CardUtils.findPairs(cardsLeft);
			pairs.Sort(CardUtils.getComparer());
			pairs.Reverse();
			List<Card> output = new List<Card>();
			// we can vomit all our pairs
			int expectedTractorSize = playUnit.getTractorSize();
			if (pairs.Count <= expectedTractorSize) {
				foreach (Card c in pairs) {
					output.Add(c);
					output.Add(c);
					cardsLeft.Remove(c);
					cardsLeft.Remove(c);
				}
				int burnCardsCount = (expectedTractorSize*2) - output.Count;
				List<Card> burns = pickXRandomCards(cardsLeft, burnCardsCount);
				output.AddRange(burns);
				return output;
			} else {
				//Gotta pick which pairs to vomit. and if they're tractors
				List<int> validTractors = CardUtils.getTractorIndices(expectedTractorSize, pairs, CardUtils.CARD_UTIL_TYPE_SINGLES_AS_PAIRS);
				if (validTractors.Count > 0) {
					//just pick the first :joy:
					int firstIndex = validTractors[0];
					for (int i = 0; i < expectedTractorSize; i++) {
						output.Add(pairs[firstIndex + i]);
						output.Add(pairs[firstIndex + i]);
					}
					return output;
				} else {
					//Rando pick all pairs
					List<Card> randomPairs = pickXRandomCards(pairs, expectedTractorSize);
					foreach (Card c in randomPairs) {
						output.Add(c);
						output.Add(c);
					}
					return output;
				}
			}
		} else if (playUnit.getMode() == PlayUnit.PAIR_MODE) {
			//Pick random pair
			List<Card> pairs = CardUtils.findPairs(cardsLeft);
			if (pairs.Count > 0) {
				//pick random pair
				Card c = pairs[CppUtils.randInt(0, pairs.Count)];
				return new List<Card>(){c, c};
			} else {
				//pick two random cards since we dont have a pair
				List<Card> output = pickXRandomCards(cardsLeft, 2);
				return output;
			}
		} else if (playUnit.getMode() == PlayUnit.SINGLE_MODE) {
			//pick random card
			List<Card> output = pickXRandomCards(cardsLeft, 1);
			return output;
		} else {
			throw new System.Exception("??? unknown playtype in random comp strat");
		}
	}
			   
	public List<Card> generateMove(Game game, Computer comp) {
		//Generate all moves
		Trick trick = game.getCurrentTrick();

		//if leading, just pick a random single
		if (trick.isLead()) {
			return new List<Card>() { comp.getHandObj().getRandomCard() };
		}

		PlayType playType = trick.getPlayType();
		Suit expectedSuit = playType.getSuit();
		List<Card> suitCards = comp.getHandObj().getCardsOfSuit(expectedSuit);
		int expectedSize = playType.getSize();
		if (suitCards.Count > expectedSize) {
			List<PlayUnit> units = playType.getPlayUnits();
			List<Card> outputCards = new List<Card>();
			
			foreach (PlayUnit unit in units) {
				List<Card> output = getPlayForPlayUnit(unit, suitCards);
				outputCards.AddRange(output);
				if (outputCards.Count == expectedSize) {
					return outputCards;
				}
				foreach (Card c in outputCards) {
					suitCards.Remove(c);
				}
			}
			throw new System.Exception("wdf random comp strat disjoint messed up play");
		} else if (suitCards.Count == expectedSize) {
			//play all suit cards
			return new List<Card>(suitCards);
		} else {
			//play all suit cards + burn rest
			
			int burnCardsCount = expectedSize - suitCards.Count;
			List<Card> allCards = new List<Card>(comp.getHand());
			CppUtils.removeAll(allCards, suitCards);
			List<Card> output = pickXRandomCards(allCards, burnCardsCount);
			output.AddRange(suitCards);
			return output;
		}
		//re turn random one
		throw new System.Exception("TODO generate move in random comp");
		// return new List<Card>();
	}
	public Suit generateBid(Game game, Computer comp) {
		Suit s = GlobalMembers.BASIC_SUITS_WITH_TRUMP[CppUtils.randInt(0, GlobalMembers.BASIC_SUITS_WITH_TRUMP.Length)];
		if (comp.getBidCards(s).Count > 0) {
			return s;
		}
		return Suit.SUIT_INVALID;
	}

	private List<Card> pickXRandomCards(List<Card> cards, int count) {
		if (cards.Count < count) {
			throw new System.Exception("wdf picking more cards than in set");
		}
		List<Card> copy = new List<Card>(cards);
		List<Card> output = new List<Card>();
		for (int i = 0; i < count; i++) {
			int idx = CppUtils.randInt(0, copy.Count);
			output.Add(copy[idx]);
			copy.RemoveAt(idx);
		}
		return output;
	}
}
