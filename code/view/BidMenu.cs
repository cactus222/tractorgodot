using Godot;
using System;

public interface BidMenuListener {
	void BidPressed(Suit s);
}

public partial class BidMenu : Node2D
{
	BidMenuListener listener;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetListener(BidMenuListener b) {
		this.listener = b;
	}
	
	private void _on_clubs_pressed()
	{
		if (this.listener != null) {
			listener.BidPressed(Suit.CLUBS);
		}
	}


	private void _on_diamonds_pressed()
	{
		
		if (this.listener != null) {
			listener.BidPressed(Suit.DIAMONDS);
		}
	}


	private void _on_hearts_pressed()
	{
		if (this.listener != null) {
			listener.BidPressed(Suit.HEARTS);
		}
	}

	private void _on_spades_pressed()
	{
		if (this.listener != null) {
			listener.BidPressed(Suit.SPADES);
		}
	}

	private void _on_no_trump_pressed()
	{
		if (this.listener != null) {
			listener.BidPressed(Suit.NO_TRUMP);
		}
	}
	
	private void _on_pass_pressed()
	{
		if (this.listener != null) {
			listener.BidPressed(Suit.SUIT_INVALID);
		}
	}

}
