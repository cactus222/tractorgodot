
public class Human : Player {
	public Human(Game game) : base(game) {
	}
	public override void requestMove() {
	}
	public override void requestPlayerBid() {
	}
	public override int getType() {
		return Player.HUMAN;
	}
	public override void respondKitty(int size) {
	}
}
