using System.Collections;
using System.Collections.Generic;

public interface ModelSub {

	void GameEventOccurred(string gameEvent);
	void GameStateChanged(GameState gameState);
	void NotifyHumanInput(string humanInputEvent);
}
