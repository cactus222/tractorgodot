
using System.Collections.Generic;

public class Trick {
	//Cards played by each player
	private List<Card>[] cards = new List<Card>[Constants.NUM_PLAYERS];
	//int winner;
	private int leader;
	private PlayType playType;
	Game game;

	public PlayType getPlayType() {
		return playType;
	}
	public List<Card> getPlay(int i) {
		return cards[i];
	}
	public void setPlay(int index, List<Card> cards) {
		if (isLead()) {
			leader = index;
		}
		//Logger::logMessage("WE SET THIS PLAYER " + std::to_string(index) + " " + std::to_string(cards.Count));
		this.cards[index] = new List<Card>(cards);
		// if (isDone()) {
		//     getWinner();
		// }
	}
	public void setPlayType(PlayType pt) {
		playType = pt;
	}
	public bool isLead() {
		for (int i = 0; i < cards.Length; i++) {
			List<Card> playersCards = cards[i];
			if (playersCards.Count > 0) {
				return false;
			}
		}
		return true;
	}

	// NOTE THIS CAN ONLY BE CALLED FOR NON LEADERS
	public int GetPlayLength() {
		for (int i = 0; i < cards.Length; i++) {
			List<Card> playersCards = cards[i];
			if (playersCards.Count > 0) {
				return playersCards.Count;
			}
		}
		return 0;
	}

	public bool IsLargerThanCurrentPlays(List<Card> play) {
	
		PlayType bestPlayType = playType;

		// find the current largest play, and then compare
		for (int i = 1; i < Constants.NUM_PLAYERS; i++) {
			int index = (i + leader) % Constants.NUM_PLAYERS;
			List<Card> playedCards = cards[index];
			//Player didnt play yet.
			if (playedCards.Count == 0) {
				continue;
			}

			if (bestPlayType.isBigger(playedCards)) {
				bestPlayType = new PlayType(playedCards, playType);
			}
		}
		
		return bestPlayType.isBigger(play);
	}

	public int getWinner() {
		int bestIndex = leader;
		//WE can ignore leader.
		PlayType bestPlayType = playType;

		for (int i = 1; i < Constants.NUM_PLAYERS; i++) {
			int index = (i + leader) % Constants.NUM_PLAYERS;
			List<Card> playedCards = cards[index];
			//Logger::logMessage("WHAT IS HAPPENING" + std::to_string(index));
			//Player didnt play yet.
			if (playedCards.Count == 0) {
				continue;
			}

			if (bestPlayType.isBigger(playedCards)) {
  //              Logger.logMessage($"THIS IS BIGGER {index} old {CardUtils.getCardListString(bestPlayType.getCardsPlayed())} new {CardUtils.getCardListString(playedCards)}");
				bestPlayType = new PlayType(playedCards, playType);
//                Logger.logMessage($"NEW PLAY TYPE {bestPlayType.ToString()}");
				bestIndex = index;
			}
		}
		

		return bestIndex;
	}

	public PlayType GetLargestPlayType() {
		PlayType bestPlayType = playType;
		for (int i = 1; i < Constants.NUM_PLAYERS; i++) {
			int index = (i + leader) % Constants.NUM_PLAYERS;
			List<Card> playedCards = cards[index];
			if (playedCards.Count == 0) {
				continue;
			}
			if (bestPlayType.isBigger(playedCards)) {
				bestPlayType = new PlayType(playedCards, playType);
			}
		}
		

		return bestPlayType;
	}
   
	public bool isDone() {
		for (int i = 0; i < cards.Length; i++) {
			List<Card> playersCards = cards[i];
			if (playersCards.Count == 0) {
				return false;
			}
		}
		return true;
	}
	public bool IsLastToPlay() {
		var missing = 0;
		for (int i = 0; i < cards.Length; i++) {
			List<Card> playersCards = cards[i];
			if (playersCards.Count == 0) {
				missing++;
			}
		}
		return missing == 1;
	}

	public List<Card>[] getCards() {
		return cards;
	}
	public Trick(Game game, int lead) {
		for (int i = 0; i < Constants.NUM_PLAYERS; i++) {
			cards[i] = new List<Card>();
		}
		this.leader = lead;
		this.game = game;
	}

	public int getTotalPoints() {
		int sum = 0;

		foreach (List<Card> playerCards in cards) {
			sum += CardUtils.getTotalPoints(playerCards);
		}

		return sum;
	}
	public override string ToString() {
		string str = "";
		for (int i = 0; i < Constants.NUM_PLAYERS; i++) {
			str = str + $"{game.getPlayer(i).getName()}:";
			foreach (Card cardsPlayed in cards[i]) {
				str = str + cardsPlayed.ToString() + " ";
			}
			if (i == leader) {
				str = str + " Lead ";
			}
			str += "\n";
		}
		if (playType!= null) {
			str += $"PlayType: {playType.ToString()}\n";
		}
		str = str + $"Value: {getTotalPoints()}\n";
		return str;
	}
	public int getLeader() {
		return leader;
	}

	public List<int> getPlayersLeftToPlay() {
		List<int> indices = new List<int>();
		for (int i = 0; i < Constants.NUM_PLAYERS; i++) {
			 List<Card> c = cards[i];
			if (c == null || c.Count == 0) {
				indices.Add(i);
			}
		}
		return indices;
	}
}
