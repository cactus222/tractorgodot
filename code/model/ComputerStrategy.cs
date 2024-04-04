using System.Collections.Generic;

public interface ComputerStrategy {
	List<Card> generateKitty(Game game, Computer comp, int size);
	List<Card> generateMove(Game game, Computer comp);
	Suit generateBid(Game game, Computer comp);
}
