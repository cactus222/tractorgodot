using System.Collections.Generic;
using System.Linq;


public class IDKComputerStrategy : ComputerStrategy {

	private Comparer<List<Card>> cardListCountComparer = Comparer<List<Card>>.Create( (a, b) => b.Count.CompareTo(a.Count) );

	// just evaluates all cards and then sorts + takes worst 8
	// higher value = less likely to be buried
	private List<Card> generateKitty2(Game game, Computer comp, int size) {
		Hand hand = comp.getHandObj();
		List<Card> cards = hand.getCards();
		List<CardEval> cardEvals = new List<CardEval>();
		foreach (Card card in cards) {
			int evaluation = GlobalMembers.RANK_MAPPING_WITH_JOKERS[card.getRank()];

			if (card.getRank() == Rank.JOKER_COL) {
				evaluation += 200;
			} else if (card.getRank() == Rank.JOKER_UNC) {
				evaluation += 150;
			} else if (card.getRank() == game.getTrumpRank()) {
				evaluation += 100;
			} else if (card.getRank() == Rank.ACE) {
				evaluation += 70;
			}

			Suit suit = CardUtils.getSuitFactorTrump(card);
			if (suit == Suit.TRUMP) {
				evaluation += 80;
			}
			if (hand.hasPair(card)) {
				evaluation += 30;
			}
			// attempt to void other suits if NO TRUMP
			if (game.getTrumpSuit() != Suit.NO_TRUMP) {
				if (suit != Suit.TRUMP) {
					List<Card> suitCards = hand.getCardsOfSuit(suit);
					int numSuitCards = suitCards.Count;
					// if we have very few, we should attempt to bury this suit (decrease evaluation)
					if (numSuitCards < 5) {
						evaluation = evaluation - (5 - numSuitCards) * (5 - numSuitCards) * 3;
					}
				}
			}

			var newCardEval = new CardEval(card, evaluation);
			cardEvals.Add(newCardEval);
		}

		cardEvals.Sort(new KittyComparer());
		var	resCardEvals = cardEvals.Take(size).ToList();
		var res = new List<Card>();
		foreach (CardEval eval in resCardEvals) {
			res.Add(eval.GetCard());
		}
		return res;
	}

	private class CardEval {
		Card card;
		int eval;
		public CardEval(Card card, int eval) {
			this.card = card;
			this.eval = eval;
		}
		
		public Card GetCard() {
			return card;
		}
		public int GetEval() {
			return eval;
		}
	}

	private class KittyComparer : IComparer<CardEval> {
		public KittyComparer() {
		}
		public int Compare(CardEval card1, CardEval card2) {
			return card1.GetEval().CompareTo(card2.GetEval());
		}
	}


	public List<Card> generateKitty(Game game, Computer comp, int size) {
		return generateKitty2(game, comp, size);

		Hand hand = comp.getHandObj();
		// Assume trump strength is good./can defend kitty

		List<List<Card>> buryTargetsBySuit = new  List<List<Card>>();
		foreach (KeyValuePair<Suit, int> kvp in GlobalMembers.SUIT_MAPPING) {
			Suit suit = kvp.Key;
			//Dont bother?
			List<Card> badCards = new List<Card>();
			//Lowest to highest
			List<Card> suitCards = hand.getCardsOfSuit(suit);
			suitCards.Sort(CardUtils.getComparer());

			if (suitCards.Count == 0) {
				continue;
			}

			if (suitCards.Count > 8) {
				buryTargetsBySuit.Add(badCards);
				continue;
			}
			
			//Keep pairs/aces/kings?, void rest.
			if (suitCards.Count < size/2) {
				foreach (Card c in suitCards) {
					if (hand.hasPair(c)) {
						continue;
					} else {
						if (GlobalMembers.RANK_MAPPING[c.getRank()] < GlobalMembers.RANK_MAPPING[Rank.KING]) {
							badCards.Add(c);
							//suitCards.Remove(c);
						}
					}
				}
			} else {
				foreach (Card c in suitCards) {
					if (hand.hasPair(c)) {
						continue;
					} else {
						if (GlobalMembers.RANK_MAPPING[c.getRank()] < GlobalMembers.RANK_MAPPING[Rank.JACK]) {
							badCards.Add(c);
							//suitCards.Remove(c);
						}
					}
				}
			}
			buryTargetsBySuit.Add(badCards);
		}

	   
		//Reverse order? by b , a
		// go low to high sort by count
		buryTargetsBySuit.Sort(cardListCountComparer);
		// buryTargetsBySuit.Reverse();

		// Attempt to void suits
		List<Card> bury = new List<Card>();
		foreach (List<Card> cards in buryTargetsBySuit) {
			foreach (Card c in cards) {
				if (bury.Count < size) {
					bury.Add(c);
				} else {
					return bury;
				}
			}
		}

		// bury X lowest trump?..
		List<Card> trumpCards = hand.getCardsOfSuit(Suit.TRUMP);
		int burnTrump = size - bury.Count;
		for (int i = 0; i < burnTrump; i++) {
			bury.Add(trumpCards[i]);
		}

		//what the fk just default to rnadom strat.
		if(bury.Count < size) {
			Logger.logMessage("WDF COMPUTER COULDNT FIND ENOUGH TO BURY?");
			List<Card> cardsInHand = comp.getHand();
			List<Card> kitty = pickXRandomCards(cardsInHand, size);
			return kitty;
		}

		return bury;
	}

	// the play unit to follow
	// should prioritize dumping points
	// whether or not a teammate is winning
	// should try to win
	private List<Card> getPlayForPlayUnit(PlayUnit playUnit, List<Card> suitCards, 
	bool shouldPrioritizePoints, 
	bool isTeammateWinning, 
	bool shouldTryToWin) {
		List<Card> cardsLeft = new List<Card>(suitCards);
		 if (playUnit.getMode() == PlayUnit.TRACTOR_MODE) {
			 //highest to lowest
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
				//We're losing this
				List<Card> burns = pickXCards(cardsLeft, burnCardsCount, shouldPrioritizePoints);
				output.AddRange(burns);
				return output;
			} else {
				//Gotta pick which pairs to vomit. and if they're tractors TODO verify order, maybe pick largest if its bigger?
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
					//we're losing this
					List<Card> burnPairs = pickXCards(pairs, expectedTractorSize, shouldPrioritizePoints);
					foreach (Card c in burnPairs) {
						output.Add(c);
						output.Add(c);
					}
					return output;
				}
			}
		} else if (playUnit.getMode() == PlayUnit.PAIR_MODE) {
			List<Card> pairs = CardUtils.findPairs(cardsLeft);
			pairs.Sort(CardUtils.getComparer());

			if (pairs.Count > 0) {
				if (isTeammateWinning) {
					List<Card> burnPairs = pickXCards(pairs, 1, shouldPrioritizePoints);
					return new List<Card>(){burnPairs[0], burnPairs[0]};
				} else {

					if (shouldTryToWin) {
						//TODO better LOGIC, just beat it with next largest pair (ascending)
						foreach (Card c in pairs) {
							//If this card is better, use it
							if (CardUtils.Compare(c, playUnit.getHighestCard()) == 1){
								return new List<Card>(){c, c};
							}
						}
					}
					//all pairs too weak, just burn weakest
					return new List<Card>(){pairs[0], pairs[0]};
				}
			} else {
				//pick two random cards since we dont have a pair
				List<Card> output = pickXCards(cardsLeft, 2, shouldPrioritizePoints);
				return output;
			}
		} else if (playUnit.getMode() == PlayUnit.SINGLE_MODE) {
			
			if (isTeammateWinning) {
				List<Card> burn = pickXCards(cardsLeft, 1, shouldPrioritizePoints);
				return burn;
			} else {
				//Descending
				cardsLeft.Sort(CardUtils.getComparer());
				
				if (shouldTryToWin) {
					// beat it with next largest...
					foreach (Card c in cardsLeft) {
						//If this card is better, use it
						if (CardUtils.Compare(c, playUnit.getHighestCard()) == 1){
							return new List<Card>(){c};
						}
					}
				}
				List<Card> burn = pickXCards(cardsLeft, 1, shouldPrioritizePoints);
				return burn;
			}
		} else {
			throw new System.Exception("??? unknown playtype in random comp strat");
		}
	}

	// findHighDisjointPlay looks to see if we can play a disjoint hand with high cards/pairs of a suit
	// CHEATS AND LOOKS AT OTHER PEOPLE'S HANDS
	// if only one pair or single high card is found, will return as if nothing happened.
	private List<Card> findHighDisjointPlay(Game game, Computer comp) {
		foreach (Suit suit in GlobalMembers.BASIC_SUITS) {
			List<Card> suitCards = comp.getCardsOfSuit(suit); // these will already be sorted, lets reverse and look from the top
			suitCards.Reverse();
			// well... we need at least 2 cards (pair + singles or single + single)
			if (suitCards.Count >= 2) {
				int totalPlays = 0;
				List<Card> possibleDisjointHighCardPlays = new List<Card>();
				List<Player> otherPlayers = game.GetOtherPlayers(comp);

				// iterate over the cards from highest to lowest, check opponents to see if they can beat this pair OR single high card
				// if they can then break out and see if we found a play with > 1 card - if so then lets play it
				for (var i = 0; i < suitCards.Count; i++) {
					List<Card> cardsToCheck = new List<Card>();
					cardsToCheck.Add(suitCards[i]);

					Card targetCard = suitCards[i];
					// check if this card is a pair, if so we should handle it as a pair instead of a single
					if (i+1 < suitCards.Count) {
						if (suitCards[i+1].Equals(targetCard)) {
							i++; // we need to also advance the pointer to skip this card
							cardsToCheck.Add(suitCards[i]); // also add the second card
						}
					}

					bool opponentHasLargerPlay = false;
					// use the play type utils to check if others can beat this
					PlayType playType = new PlayType(cardsToCheck);
					// check other players to see if they can beat this play
					foreach (Player player in otherPlayers) {
						List<Card> targetHand = player.getCardsOfSuit(suit);
						if (playType.hasLargerUnit(targetHand)) {
							opponentHasLargerPlay = true;
							break;
						}
					}
					if (!opponentHasLargerPlay) {
						totalPlays++;
						possibleDisjointHighCardPlays.AddRange(cardsToCheck);
					}
				}
				// if 2+ plays, lets do it
				if (totalPlays >= 2) {
					return possibleDisjointHighCardPlays;
				}
			}
		}
		return new List<Card>();
	}
			  
	public List<Card> generateMove(Game game, Computer comp) {
		//Generate all moves
		Trick trick = game.getCurrentTrick();

		//if leading, just pick a random single
		if (trick.isLead()) {
			Hand hand = comp.getHandObj();
			List<Card> tractors = hand.findTractorPlay(false);
			if (tractors.Count > 0) {
				return tractors;
			}

			List<Card> highDisjoint = findHighDisjointPlay(game, comp);
			if (highDisjoint.Count > 0) {
				return highDisjoint;
			} 

			List<Card> aces = hand.findAcePlay(true);
			if (aces.Count > 0) {
				return aces;
			}
			//Pairs >= 8, TODO better
			List<Card> highPairs = hand.findHighPairPlay(true, Rank.EIGHT);
			if (highPairs.Count > 0) {
				return highPairs;
			}

			List<Card> trumpPairs = hand.findHighPairPlay(false, Rank.EIGHT);
			if (trumpPairs.Count > 0) {
				return trumpPairs;
			}

			highPairs = hand.findHighPairPlay(true, Rank.TWO);
			if (highPairs.Count > 0) {
				return highPairs;
			}
			trumpPairs = hand.findHighPairPlay(false, Rank.TWO);
			if (trumpPairs.Count > 0) {
				return trumpPairs;
			}

			// LEAD HIGHEST CARD OF LONGEST SUIT
			List<List<Card>> suitCardsSorted = new List<List<Card>>();
			foreach (Suit s in GlobalMembers.BASIC_SUITS) {

				//This ignores trump (will be empty hand)
				List<Card> cards = hand.getCardsOfSuit(s);
				suitCardsSorted.Add(cards);
				
			}
			suitCardsSorted.Sort(cardListCountComparer);
			if (suitCardsSorted[0].Count > 0) {
				return new List<Card>(){suitCardsSorted[0][0]};
			}


			//TODO PLAY RANDOM CARD
			return new List<Card>() { comp.getHandObj().getRandomCard() };
		}


		PlayType playType = trick.getPlayType();
		Suit expectedSuit = playType.getSuit();
		List<Card> suitCards = comp.getHandObj().getCardsOfSuit(expectedSuit);
		int expectedSize = playType.getSize();

		bool prioritizePts = shouldPrioritizePoints(game, comp);
		bool teamWinning = isTeammateWinning(game, comp);

		if (suitCards.Count > expectedSize) {
			var largestPlayType = trick.GetLargestPlayType();
			var tryToWin = true;

			// if the trick is trumped already, theres no point in us trying to win at all
			if (largestPlayType.getSuit() == Suit.TRUMP && playType.getSuit() != Suit.TRUMP) {
				tryToWin = false;
			}

			List<PlayUnit> targetUnits = playType.getPlayUnits();
			List<Card> outputCards = new List<Card>();
			
			// if we want to try to win, compare our play against the strongest on suit play
			if (tryToWin) {
				targetUnits = largestPlayType.getPlayUnits();
			}

			foreach (PlayUnit unit in targetUnits) {
				List<Card> output = getPlayForPlayUnit(unit, suitCards, prioritizePts, teamWinning, tryToWin);
				outputCards.AddRange(output);
				foreach (Card c in outputCards) {
					suitCards.Remove(c);
				}
			}
			

			// if we were prioritizing dumping cards or already didnt try to win, just return
			if (!prioritizePts || !tryToWin) {
				return outputCards;
			}
			
			// if we lose even with our best play, lets just give up
			if (trick.IsLargerThanCurrentPlays(outputCards)) {
				// reset and retry
				suitCards.AddRange(outputCards);
				outputCards.Clear();

				targetUnits = playType.getPlayUnits();
				foreach (PlayUnit unit in targetUnits) {
					List<Card> output = getPlayForPlayUnit(unit, suitCards, prioritizePts, teamWinning, false);
					outputCards.AddRange(output);
					foreach (Card c in outputCards) {
						suitCards.Remove(c);
					}
				}
			}
			return outputCards;
			throw new System.Exception("wdf random comp strat disjoint messed up play");
		} else if (suitCards.Count == expectedSize) {
			//play all suit cards
			return new List<Card>(suitCards);
			//We lose + void
		} else if (suitCards.Count > 0) {
			//play all suit cards + burn rest
			
			int burnCardsCount = expectedSize - suitCards.Count;
			List<Card> allCards = new List<Card>(comp.getHand());
			CppUtils.removeAll(allCards, suitCards);
			List<Card> output = pickXCards(allCards, burnCardsCount, prioritizePts);
			output.AddRange(suitCards);
			return output;
		} else { // We're completely free from this suit, can trump or throw whatever.
			if (teamWinning && prioritizePts) {
				List<Card> output = pickXCards(comp.getHand(), expectedSize, prioritizePts);
				return output;
			} else {
				//try to win?
				if (!teamWinning && trick.getPlayersLeftToPlay().Count >= 1 && trick.getTotalPoints() >= 5) {
					List<Card> trumpCards = comp.getHandObj().getCardsOfSuit(Suit.TRUMP);
					// var targetPlayType = playType;
					var targetPlayType = trick.GetLargestPlayType();
					// WE NEED TO COMPARE AGAINST THE LARGEST PLAY TYPE, NOT THE LEADER, SINCE OTHER PEOPLE MAY HAVE TRUMPED

					List<PlayUnit> units = targetPlayType.getPlayUnits();
					List<Card> outputCards = new List<Card>();
					//attempt to win
					if (trumpCards.Count >= targetPlayType.getSize()) {
						foreach (PlayUnit unit in units) {
							List<Card> trumpPlay = getPlayForPlayUnit(unit, trumpCards, prioritizePts, teamWinning, true);
							outputCards.AddRange(trumpPlay);
							if (outputCards.Count == expectedSize) {
								
								// can trump it -- check if we're bigger than all otherwise give up
								if (trick.IsLargerThanCurrentPlays(outputCards)) {
									return outputCards;
								} else {
									break;
								}
							}
							foreach (Card c in trumpPlay) {
								trumpCards.Remove(c);
							}
						}
					}

				}
			   
				//ok we cant win/dont care
				List<Card> output = pickXCards(comp.getHand(), expectedSize, prioritizePts);
				return output;
			}
		}
		//re turn random one
		throw new System.Exception("TODO generate move in random comp");
		// return new List<Card>();
	}
	public Suit generateBid(Game game, Computer comp) {
		int numCardsInHand = comp.getHandSize();
		foreach (KeyValuePair<Suit, List<Card>> kvp in comp.getHandObj().getBiddableSuits()) {
			Suit s = kvp.Key;
			if (GlobalMembers.BASIC_SUITS.Contains(s)) {
				if ((double)(comp.getHandObj().getNumCardsOfSuit(s)) / numCardsInHand  > 0.33) {
					return s;
				}
			} else if (s == Suit.NO_TRUMP) {
				// NT? use bridge convention?
				int numAces = comp.getHandObj().getNumCardsOfRank(Rank.ACE);
				int numKings = comp.getHandObj().getNumCardsOfRank(Rank.KING);
				int numQueen = comp.getHandObj().getNumCardsOfRank(Rank.QUEEN);
				int numJack = comp.getHandObj().getNumCardsOfRank(Rank.JACK);

				int totalPts = numAces * 4 + numKings * 3 + numQueen * 2 + numJack * 1;
				if (totalPts > 26) {
					return s;
				}
			}
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

//TODO
	private bool shouldPrioritizePoints(Game game, Computer comp) {
		int myPlayerType = game.getPlayerType(comp);
	  

		List<int> playersLeftToPlayIndicesExcludingMyself = game.getCurrentTrick().getPlayersLeftToPlay();
		var myIndex = game.getPlayerIndex(comp);
		playersLeftToPlayIndicesExcludingMyself.Remove(myIndex);

		// teammate is winning
		bool isWinning = isTeammateWinning(game, comp);
		if (isWinning) {
			// we are the last
			if (playersLeftToPlayIndicesExcludingMyself.Count == 0) {
				return true;
			}
			// TODO better logic maybe cheat and check if the next player/partner can beat the current trick
			return true;
		} else {
			if (playersLeftToPlayIndicesExcludingMyself.Count == 0) {
				return false;
			}
			// TODO better logic maybe cheat and check if the next player/partner can beat the current trick
			return false;
		}
	}
	private bool isTeammateWinning(Game game, Computer comp) {
		int playerType = game.getPlayerType(comp);
		//Winning player so far
		int winningPlayerType = game.getPlayerType(game.getPlayer(game.getCurrentTrick().getWinner()));
		return playerType == winningPlayerType;
	}

	private List<Card> pickXCards(List<Card> cards, int count, bool prioritizePoints) {
		if (cards.Count < count) {
			throw new System.Exception("wdf picking more cards than in set");
		}
		List<Card> copy = new List<Card>(cards);
		copy.Sort(new BurnCardComparator(prioritizePoints));
		var	res = copy.Take(count).ToList();	
		return res;
	}
	//Prioritizes points, 10 K 5, 3 ... A
	private class BurnCardComparator : IComparer<Card> {
		private bool prioritizePoints = false;
		public BurnCardComparator(bool prioritizePoints) {
			this.prioritizePoints = prioritizePoints;
		}

		// if we want to prioritize points, we want to.... drop 10, K, 5, 2 -> the rest
		// if we dont want to prioritize points, we want to drop 2-> the rest, 5, 10, K
		public int Compare(Card card1, Card card2) {
			int pts1 = card1.getPointValue();
			int pts2 = card2.getPointValue();
			int pointCompare = pts1.CompareTo(pts2);

			if (this.prioritizePoints) {
				// Card 1 worth more pts than 2
				if (pointCompare > 0) {
					return -1;
					//Worth same pts
				} else if (pointCompare < 0) {
					// card 2 worth more pts than 1
					return 1;
				}
			} else {
				// Card 1 worth more pts than 2
				if (pointCompare > 0) {
					return 1;
					//Worth same pts
				} else if (pointCompare < 0) {
					// card 2 worth more pts than 1
					return -1;
				}
			}
			// then compare cardutil if both equal (this handles trump)
			var cardCompare = CardUtils.Compare(card1, card2);
	
			if (cardCompare == 0) {
				// compare by rank if both are unknown/equal
				return GlobalMembers.RANK_MAPPING_WITH_JOKERS[card1.getRank()].CompareTo(GlobalMembers.RANK_MAPPING_WITH_JOKERS[card2.getRank()]);
			} else {
				return cardCompare;
			}
		}
	}
}
