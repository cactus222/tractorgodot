using System.Collections.Generic;

public class Computer : Player {

	ComputerStrategy strat;

	public Computer(Game game) : base(game) {
		//.
		strat = new IDKComputerStrategy();
	}
	
	public void setStrategy(ComputerStrategy s) {
		this.strat = s;
	}

	public override void respondKitty(int size) {
		game.receiveBury(this.strat.generateKitty(this.game, this, size));
	}
	public override void requestMove() {
		game.receivePlay(this, this.strat.generateMove(this.game, this));
	}
	public override void requestPlayerBid() {
		game.receiveBid(this, this.strat.generateBid(this.game, this));
	}


	public override int getType() {
		return Player.COMPUTER;
	}
}


