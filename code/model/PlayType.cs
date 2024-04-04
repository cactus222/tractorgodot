using System.Collections.Generic;
using System.Linq;

//TODO WDF IS THIS

public class PlayType {
	public int getPlayUnitTractorSize() {
		return playUnits[0].getTractorSize();
	}
	public PlayType(List<Card> cardsPlayed) {

		this.cardsPlayed = cardsPlayed;
		playUnits = splitIntoPlayUnits(cardsPlayed, false);

	}
	public PlayType(List<Card> cardsPlayed, PlayType followPlayType) {
		this.cardsPlayed = cardsPlayed;
		playUnits = splitIntoPlayUnitsFollow(cardsPlayed, followPlayType);

	}

	//TODO MERGE IS BIGGER AND THIS SOMEHOW!!!
	//TODO fix isbigger first.
	public List<PlayUnit> splitIntoPlayUnitsFollow(List<Card> follow, PlayType followPlayType) {
		List<PlayUnit> newPlayUnits = new List<PlayUnit>();
		if (follow.Count != getSize()) {
			//return false;
			throw new System.Exception("error split play units follow smaller?");
		}
		if (follow.Count == 0) {
			throw new System.Exception("WDF ZERO CARDS HERE");
		}
		if (CardUtils.getSuitFactorTrump(follow[0]) != getSuit() && CardUtils.getSuitFactorTrump(follow[0]) != Suit.TRUMP) {
			//return false;
			throw new System.Exception("error split play units follow smaller?");
		}

		List<Card> cardsToConsider = new List<Card>(follow);

		//bool isLarger = false;
		//Only check largest, if larger then just need tomatch play type!
		foreach (PlayUnit playUnit in followPlayType.playUnits) {
			if (playUnit.getMode() == PlayUnit.TRACTOR_MODE) {
				int tractorSize = playUnit.getTractorSize();

				List<Card> pairs = findPairs(cardsToConsider);
				if (pairs.Count < tractorSize) {
					//return false;
					throw new System.Exception("error split play units follow smaller?");
				}
				pairs.Sort(CardUtils.getComparer());
				pairs.Reverse();
				// std::sort(pairs.begin(), pairs.end(), comp.functorMethod);

				//Find largest tractor of size.
				//Reverse order!!! to find largest
				//If tractor length match, go to next
				int tractorLength = 0;
				List<Card> tractorCardRefs = new List<Card>();
				Card largestTractorCard = null;
				Card currentCard = pairs[pairs.Count - 1];

				for (int i = pairs.Count - 1; i >= 0; i--) {

					Card nextCard = pairs[i];
					if (CardUtils.isAdjacentRank(currentCard, nextCard)) {
						if (tractorLength == 0) {
							largestTractorCard = currentCard;
							tractorCardRefs.Add(currentCard);
							tractorCardRefs.Add(getOtherCopy(currentCard)); //OTHER
							tractorLength = 2;
						} else {
							tractorLength++;
						}

						tractorCardRefs.Add(nextCard);
						tractorCardRefs.Add(getOtherCopy(nextCard));

						if (tractorLength == tractorSize) {
							//If tractor card si bigger
							//TODO CHECK
							if (CardUtils.Compare(largestTractorCard, playUnit.getHighestCard()) > 0) {
								//isLarger = true;
								PlayUnit unit = new PlayUnit(tractorCardRefs, PlayUnit.TRACTOR_MODE);
								newPlayUnits.Add(unit);
								CppUtils.removeAll(cardsToConsider, tractorCardRefs);
								continue;
							} else if (largestTractorCard.Equals(playUnit.getHighestCard())) {
								//We matched, compare against next play unit!
								PlayUnit unit = new PlayUnit(tractorCardRefs, PlayUnit.TRACTOR_MODE);
								newPlayUnits.Add(unit);
								CppUtils.removeAll(cardsToConsider, tractorCardRefs);
								continue;
							} else {
								//Since we're sorted, we're screwed
								//return false;
								throw new System.Exception("error split play units follow smaller?");
							}
						}
					} else {
						tractorLength = 0;
						largestTractorCard = null;
						tractorCardRefs.Clear();
					}
					currentCard = nextCard;
				}

				///We could not beat the top tractor, exit
				//if (!isLarger)
				//{
				//return false;
				//	throw "error split play units follow smaller?";
				//}
			} else if (playUnit.getMode() == PlayUnit.PAIR_MODE) {
				List<Card> pairs = findPairs(cardsToConsider);
				if (pairs.Count < 1) {
					//return false;
					throw new System.Exception("error split play units follow smaller?");
				}
				pairs.Sort(CardUtils.getComparer());
				//If tractor card si bigger
				if (CardUtils.Compare(pairs[0], playUnit.getHighestCard()) > 0) {
					//isLarger = true;
					List<Card> remove = new List<Card>();
					remove.Add(pairs[0]);
					remove.Add(getOtherCopy(pairs[0]));
					PlayUnit unit = new PlayUnit(remove, PlayUnit.PAIR_MODE);
					newPlayUnits.Add(unit);
					CppUtils.removeAll(cardsToConsider, remove);
					continue;
				} else if (pairs[0].Equals(playUnit.getHighestCard())) {
					//We matched, compare against next play unit!
					List<Card> remove = new List<Card>();
					remove.Add(pairs[0]);
					remove.Add(getOtherCopy(pairs[0]));
					PlayUnit unit = new PlayUnit(remove, PlayUnit.PAIR_MODE);
					newPlayUnits.Add(unit);
					CppUtils.removeAll(cardsToConsider, remove);
					continue;
				} else {
					//Since we're sorted, we're screwed
					//return false;
					throw new System.Exception("error split play units follow smaller?");
				}
			} else if (playUnit.getMode() == PlayUnit.SINGLE_MODE) {
				cardsToConsider.Sort(CardUtils.getComparer());
				cardsToConsider.Reverse();
				// std::sort(cardsToConsider.begin(), cardsToConsider.end(), comp.functorMethod);
				if (cardsToConsider.Count < 1) {
					//return false;
					throw new System.Exception("error split play units follow smaller?");
				}
				if (CardUtils.Compare(cardsToConsider[0], playUnit.getHighestCard()) > 0) {
					//isLarger = true;
					PlayUnit unit = new PlayUnit(cardsToConsider[0], PlayUnit.SINGLE_MODE);
					newPlayUnits.Add(unit);
					CppUtils.remove(cardsToConsider, cardsToConsider[0]);
					continue;
				} else if (cardsToConsider[0].Equals(playUnit.getHighestCard())) {
					//We matched, compare against next play unit!
					PlayUnit unit = new PlayUnit(cardsToConsider[0], PlayUnit.SINGLE_MODE);
					newPlayUnits.Add(unit);
					CppUtils.remove(cardsToConsider, cardsToConsider[0]);
					continue;
				} else {
					//Since we're sorted, we're screwed
					//return false;
					throw new System.Exception("error split play units follow smaller?");
				}
			}
		}

		return newPlayUnits;
		//We're equal... so we're smaller since we played 2nd.
		//return false;
	}
	public bool isBigger(List<Card> follow) {
		if (follow.Count != getSize()) {
			return false;
		}
		if (CardUtils.getSuitFactorTrump(follow[0]) != getSuit() && CardUtils.getSuitFactorTrump(follow[0]) != Suit.TRUMP) {
			return false;
		}
		List<Card> cardsToConsider = new List<Card>(follow);
		if (!CardUtils.isAllSameSuit(follow)) {
			return false;
		}

		bool isLarger = false;
		//Only check largest, if larger then just need tomatch play type!

//todo fix this for smaller future pairs, if larger jsut skip if exists same playtype.

		for(int playUnitIndex = 0; playUnitIndex < playUnits.Count; playUnitIndex++) {
			PlayUnit playUnit = playUnits[playUnitIndex];
			if (playUnit.getMode() == PlayUnit.TRACTOR_MODE) {
				int tractorSize = playUnit.getTractorSize();
				List<Card> pairs = findPairs(cardsToConsider);

				// Logger.logMessage($" isbigger pairs {CardUtils.getCardListString(pairs)} {pairs.Count} {tractorSize}");
				if (pairs.Count < tractorSize) {
					return false;
				}
				// throw new System.Exception($"pairs {CardUtils.getCardListString(pairs)} {tractorSize}");
				// std::sort(pairs.begin(), pairs.end(), comp.functorMethod);
				pairs.Sort(CardUtils.getComparer());
				pairs.Reverse();
				// Logger.logMessage($" isbigger pairs {CardUtils.getCardListString(pairs)}");

				//Find largest tractor of size.
				//Reverse order!!! to find largest
				//If tractor length match, go to next
				int tractorLength = 0;
				List<Card> tractorCardRefs = new List<Card>();
				Card largestTractorCard = null;
				Card currentCard = pairs[pairs.Count - 1];

				for (int i = pairs.Count - 1; i >= 0; i--) { 
					
					Card nextCard = pairs[i];
					// Logger.logMessage($"bigger nextCard {nextCard.ToString()}");
					if (CardUtils.isAdjacentRank(currentCard, nextCard)) {
						if (tractorLength == 0) {
							largestTractorCard = currentCard;
							tractorCardRefs.Add(currentCard);
							tractorCardRefs.Add(getOtherCopy(currentCard)); //OTHER
							tractorLength = 2;
						} else {
							tractorLength++;
						}
						tractorCardRefs.Add(nextCard);
						tractorCardRefs.Add(getOtherCopy(nextCard));
						if (tractorLength == tractorSize) {
							//If tractor card is bigger, OR if we're already larger, just extract a tractor.
							int comparison = CardUtils.Compare(largestTractorCard, playUnit.getHighestCard());
							if (isLarger || comparison > 0) {
								isLarger = true;
								CppUtils.removeAll(cardsToConsider, tractorCardRefs);
								continue;
								//TODO how the hell does this happen.
							} else if (comparison == 0) {
								//We matched, compare against next play unit!
								CppUtils.removeAll(cardsToConsider, tractorCardRefs);
								continue;
							} else {
								//Since we're sorted, we're screwed
								return false;
							}
						}
					} else {
						tractorLength = 0;
						largestTractorCard = null;
						tractorCardRefs.Clear();
					}
					currentCard = nextCard;
				}

				//We could not beat the top tractor, exit
				if (!isLarger) {
					return false;
				}
			} else if (playUnit.getMode() == PlayUnit.PAIR_MODE) {
				List<Card> pairs = findPairs(cardsToConsider);
					// Logger.logMessage("consider " + CardUtils.getCardListString(cardsToConsider));
				if (pairs.Count < 1) {
					return false;
				}
				pairs.Sort(CardUtils.getComparer());
				pairs.Reverse();
				int comparison = CardUtils.Compare(pairs[0], playUnit.getHighestCard());
				//If pair card is bigger or we're already larger, just extract the pair
				if (isLarger || comparison == 1) {
					isLarger = true;
					List<Card> remove = new List<Card>();
					remove.Add(pairs[0]);
					remove.Add(getOtherCopy(pairs[0]));
					CppUtils.removeAll(cardsToConsider, remove);
					continue;
					//TODO how the hell does this happen -- same trumprank/trumpsuit?
				} else if (comparison == 0) {
					//We matched, compare against next play unit!
					List<Card> remove = new List<Card>();
					remove.Add(pairs[0]);
					remove.Add(getOtherCopy(pairs[0]));
					CppUtils.removeAll(cardsToConsider, remove);
					continue;
				} else {
					//Since we're sorted, we're screwed
					return false;
				}
			} else if (playUnit.getMode() == PlayUnit.SINGLE_MODE) {
				cardsToConsider.Sort(CardUtils.getComparer());
				cardsToConsider.Reverse();

				if (isLarger) {
					return true;
				}
				int singlesCompareLargest = CardUtils.Compare(cardsToConsider[0], playUnit.getHighestCard());
				if (singlesCompareLargest == 1) {
					return true;
				} else if (singlesCompareLargest == 0) {
					//We matched, compare against next play unit!
					cardsToConsider.RemoveAt(0);
					continue;
				} else {
					//Since we're sorted, we're screwed
					return false;
				}
			}
		}

		//We're equal... so we're smaller since we played 2nd.
		return isLarger;
	}
	public Card getOtherCopy(Card card) {
		return new Card(card.getRank(), card.getSuit());
	}
	
	//TODO this will fk up if > 2 copies
	public List<Card> findPairs(List<Card> cards) {

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
	public List<Card> findPairs() {
		return findPairs(new List<Card>(this.cardsPlayed));
	}
	public void sortPlayUnits() {
		//	std::sort(playUnits, playUnitComparator);
	}
	public int getSize() {
		return cardsPlayed.Count;
	}

	public List<Card> getCardsPlayed() {
		return cardsPlayed;
	}

	public int getNumCardsOfSuit(Suit s) {
		int cnt = 0;

		for (int i = 0; i < cardsPlayed.Count; i++) {
			if (CardUtils.getSuitFactorTrump(cardsPlayed[i]) == s) {
				cnt++;
			}
		}
		return cnt;
	}
	public Card getACard() {
		return playUnits[0].getHighestCard();
	}
	public Suit getSuit() {
		return CardUtils.getSuitFactorTrump(cardsPlayed[0]);
	}

	public bool isDisjoint() {
		//Suit mainSuit = SUIT_INVALID;
		//if (cardsPlayed.Count == 0 || cardsPlayed.Count == 1)
		//{
		//	return false;
		//}

		//if (cardsPlayed.Count % 2 != 0)
		//{
		//	return true;
		//}
		////TODO CONFIRM if cardsPlayed is sroted

		//mainSuit = CardUtils.getSuitFactorTrump(cardsPlayed[0]);
		//Card lastCard = cardsPlayed[0];
		//Card lastPair = NULL;

		//for (int i = 1; i < cardsPlayed.Count; i++)
		//{
		//	Card card = cardsPlayed[i];
		//	if (CardUtils.getSuitFactorTrump(card) != mainSuit)
		//	{
		//		return true;
		//	}
		//
		//	if ()

		//}
		//return false;
		return playUnits.Count > 1;
	}
	public int getPlayUnitType() {
		return playUnits[0].getPlayUnitType();
	}

// WDF WAS I THINKING
	public List<PlayUnit> splitIntoPlayUnits(List<Card> cards, bool granular = false) {
		List<PlayUnit> playUnits = new List<PlayUnit>();
		HashSet<Card> singlesSet = new HashSet<Card>();
		List<Card> pairedCards = new List<Card>();

		for (int i = 0; i < cards.Count; i++) {
			if (singlesSet.Contains(cards[i])) {
				pairedCards.Add(cards[i]);
				singlesSet.Remove(cards[i]);
			} else {
				singlesSet.Add(cards[i]);
			}
		}

		pairedCards.Sort(CardUtils.getComparer());
		pairedCards.Reverse();
		List<Card> singles = new List<Card>(singlesSet);
		singles.Sort(CardUtils.getComparer());
		singles.Reverse();


		//TODO
		//When i pass into play unit, i need to tell it what type!!! or i need to actually pass the pair!!!


		//Find all tractors. and pairs
		List<Card> pairedTractors = new List<Card>();
		foreach (Card pairs in pairedCards) {
			//Part of tractor.
			if (pairedTractors.Count > 0 && CardUtils.isAdjacentRank(pairedTractors[pairedTractors.Count - 1], pairs)) {
				pairedTractors.Add(pairs);
			} else {

				//Last tractor finished
				if (pairedTractors.Count > 1) {
					List<Card> duped = new List<Card>(pairedTractors);
					List<Card> duped2 = new List<Card>(pairedTractors);
					duped.AddRange(duped2);
					PlayUnit unit = new PlayUnit(duped, PlayUnit.TRACTOR_MODE);
					playUnits.Add(unit);
					pairedTractors.Clear();
					//Last was solo pair
				} else if (pairedTractors.Count == 1) {
					List<Card> duped = new List<Card>(pairedTractors);
					List<Card> duped2 = new List<Card>(pairedTractors);
					duped.AddRange(duped2);
					PlayUnit unit = new PlayUnit(duped, PlayUnit.PAIR_MODE);
					playUnits.Add(unit);
					pairedTractors.Clear();
				}
				pairedTractors.Add(pairs);
			}

			//yea im too lazy to think there. so i'm just going to add dupes in granular mode...
			if (granular) {
				List<Card> singleGran = new List<Card>();
				singleGran.Add(pairs);

				// TODO MAYBE FIXME - may need to dupe.
				PlayUnit unit = new PlayUnit(singleGran, PlayUnit.PAIR_MODE);
				playUnits.Add(unit);

				unit = new PlayUnit(singleGran, PlayUnit.SINGLE_MODE);
				playUnits.Add(unit);
			}
		}

//TODO CLEAN UP
		if (pairedTractors.Count > 1) {
			List<Card> duped = new List<Card>(pairedTractors);
			List<Card> duped2 = new List<Card>(pairedTractors);
			duped.AddRange(duped2);
			PlayUnit unit = new PlayUnit(duped, PlayUnit.TRACTOR_MODE);
			playUnits.Add(unit);
			pairedTractors.Clear();
		} else if (pairedTractors.Count == 1) {
			List<Card> duped = new List<Card>(pairedTractors);
			List<Card> duped2 = new List<Card>(pairedTractors);
			duped.AddRange(duped2);
			PlayUnit unit = new PlayUnit(duped, PlayUnit.PAIR_MODE);
			playUnits.Add(unit);
			pairedTractors.Clear();
		}

		//Handle singles
		foreach (Card single in singles) {
			List<Card> singleGran = new List<Card>();
			singleGran.Add(single);
			PlayUnit unit = new PlayUnit(singleGran, PlayUnit.SINGLE_MODE);
			playUnits.Add(unit);
		}


		return playUnits;
	}



	//Only use if disjoint?

	public bool hasLargerUnit(List<Card> cards) {
		List<PlayUnit> newPlayUnits = splitIntoPlayUnits(cards, true);
		//TODO
		//We need ot search other player's hands to see if they're legal.
		bool found = false;
		foreach (PlayUnit unit in getPlayUnits()) {
			foreach (PlayUnit contender in newPlayUnits) {
				if (isLargerPlayUnit(unit, contender)) {
					Logger.logMessage($"{unit.ToString()} is smaller than {contender.ToString()}");
					found = true;
					break;
				}
			}
			if (found) {
				break;
			}
		}

		//TODO WHAT IS THIS
		// foreach (PlayUnit playUnit in newPlayUnits)
		// {
		// 	playUnit = null;
		// }
		return found;
	}

	public List<PlayUnit> getPlayUnits() {
		return playUnits;
	}

	public PlayUnit getFirstUnit() {
		return playUnits[0];
	}

	// Returns true if 2nd is larger than first
	//false if equals or is smaller
	public bool isLargerPlayUnit(PlayUnit original, PlayUnit contender) {
		if (original.getLength() == contender.getLength() && original.getPlayUnitType() == contender.getPlayUnitType() && contender.isAllOfSameSuit()) {
			if (CardUtils.Compare(original.getHighestCard(), contender.getHighestCard()) < 0) {
				return true;
			}
		}
		return false;
	}

	public override string ToString() {
		string msg = "";//Cards: {CardUtils.getCardListString(cardsPlayed)} ";
		foreach (PlayUnit unit in playUnits) {
			msg += unit.ToString() + "\n";
		}
		return msg;
	}

	public const int PLAY_TYPE_SAME = 1;
	public const int PLAY_TYPE_DIFF = 0;
	private List<PlayUnit> playUnits = new List<PlayUnit>();
  
	private List<Card> cardsPlayed = new List<Card>();
}



