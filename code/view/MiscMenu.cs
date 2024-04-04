using Godot;
using System;

public interface MiscMenuListener {
	void BuryPressed();
	void PlayPressed();
	void NextPressed();
	void NewGamePressed();
}
public partial class MiscMenu : Node2D
{
	MiscMenuListener listener;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetListener(MiscMenuListener l) {
		this.listener = l;
	}
	
	private void _on_bury_pressed()
	{
		if (this.listener != null) {
			listener.BuryPressed();
		}
	}


	private void _on_play_pressed()
	{
		if (this.listener != null) {
			listener.PlayPressed();
		}
	}
	
	private void _on_next_round_pressed()
	{
		if (this.listener != null) {
			listener.NextPressed();
		}
	}

	private void _on_new_game_pressed()
	{
		if (this.listener != null) {
			listener.NewGamePressed();
		}
	}


}



