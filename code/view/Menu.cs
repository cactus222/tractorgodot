using Godot;
using System;

public partial class Menu : Control
{
	LineEdit seedInput;
	LineEdit bidDelay;
	LineEdit trickDelay;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		seedInput = GetNode<LineEdit>("MarginContainer/VBoxContainer/SeedInput");
		bidDelay = GetNode<LineEdit>("MarginContainer/VBoxContainer/BidDelay");
		trickDelay = GetNode<LineEdit>("MarginContainer/VBoxContainer/TrickDelay");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_play_pressed()
	{
		var inputString = seedInput.Text;
		
		if (Int32.TryParse(inputString, out int numValue)) {
			// GD.Print($"used seed {inputString}");
			CppUtils.SetSeed(numValue);
		} else {
			CppUtils.SetSeed(Guid.NewGuid().GetHashCode());
		}
		
		var bidDelayInputString = bidDelay.Text;
		if (float.TryParse(bidDelayInputString, out float bidDelayValue)) {
			ViewSettings.SetBidDelay(bidDelayValue);
		}

		var trickDelayInputString = trickDelay.Text;
		if (float.TryParse(trickDelayInputString, out float trickDelayValue)) {
			ViewSettings.SetTrickDelay(trickDelayValue);
		}

		GetTree().ChangeSceneToFile("res://gameview/MainView.tscn");
	}

	private void _on_quit_pressed()
	{
		GetTree().Quit();
	}

}


