using System.Collections.Generic;
public class PlayerHandData {
    public void update(Trick trick) {
        PlayType playType = trick.getPlayType();
        Suit leadSuit = playType.getSuit();
        List<Card>[] playedCards = trick.getCards();
        for (int playerIndex = trick.getLeader(); playerIndex < Constants.NUM_PLAYERS + trick.getLeader(); playerIndex++) {
            int realIndex = playerIndex % Constants.NUM_PLAYERS;
            List<Card> playerCardsPlayed = playedCards[realIndex];

            for (int cardIndex = 0; cardIndex < playerCardsPlayed.Count; cardIndex++) {
                Card playedCard = playerCardsPlayed[cardIndex];
                if (!numCardsPlayedByCopy.ContainsKey(playedCard)) {
                    numCardsPlayedByCopy[playedCard] = 1;
                } else {
                    numCardsPlayedByCopy[playedCard] = 2;
                }

                if (CardUtils.getSuitFactorTrump(playedCard) != leadSuit) {
                    voidSuits[playerIndex][leadSuit] = true;
                }
            }
        }
    }

    private Dictionary<Card, int> numCardsPlayedByCopy = new Dictionary<Card, int>();
    private List<Dictionary<Suit, bool>> voidSuits = new List<Dictionary<Suit, bool>>();
    private List<Dictionary<Suit, bool>> pairSuits = new List<Dictionary<Suit, bool>>();

}