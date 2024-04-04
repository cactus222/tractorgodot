using Godot;
using System;
using System.Collections.Generic;

public partial class TrickView : Node2D
{
	HandView[] handViews = new HandView[4];
	bool isAnimating = false;
	Trick queuedTrick;
	Timer trickTimer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		for (var i = 0; i < 4; i++) {
			var handScene = GD.Load<PackedScene>("res://gameview/HandView.tscn");
			var instance = handScene.Instantiate();
			AddChild(instance);
			var handView = (HandView)instance;
			handViews[i] = handView;
		}
		
		trickTimer = new Timer();
		AddChild(trickTimer);
		trickTimer.Timeout += clearTrick;
		trickTimer.OneShot = true;
		trickTimer.WaitTime = ViewSettings.GetTrickDelay();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetTrick(Trick t) {
		queuedTrick = t;
		processQueue(); 
	}
	
	private void processQueue() {
		if (!isAnimating) {
			List<Card>[] cards = queuedTrick.getCards();
			for (int i = 0; i < 4; i++) {
				handViews[i].SetCards(cards[i]);
			}
		}
	}

	public async void EndTrick(Trick t) {
		List<Card>[] cards = t.getCards();
		for (int i = 0; i < 4; i++) {
			handViews[i].SetCards(cards[i]);
		}
		isAnimating = true;

		trickTimer.Start();
		// // this works... kinda, it pauses this thread which is not what i want
		// await ToSignal(GetTree().CreateTimer(trickDelaySeconds), SceneTreeTimer.SignalName.Timeout);
		// clearTrick();
	}
	
	private void clearTrick() {
		for (int i = 0; i < 4; i++) {
			handViews[i].SetCards(new List<Card>());
		}
		isAnimating = false;
		processQueue();
	}
	
	public void SetXOffset(int offset) {
		var viewSizeWidth = (int)GetViewport().GetVisibleRect().Size.X;
		var viewSizeHeight = (int)GetViewport().GetVisibleRect().Size.Y;
		Vector2[] randomCoords = new Vector2[4];
		var deltaHeightY = 200;
		var topGapY = 20;
		randomCoords[0] = new Vector2(offset,2 * deltaHeightY + topGapY); // S
		randomCoords[1] = new Vector2(-2*offset,deltaHeightY + topGapY); // W
		randomCoords[2] = new Vector2(offset, topGapY); // N
		randomCoords[3] = new Vector2(viewSizeWidth/5*2 - offset,deltaHeightY + topGapY); // E
		for (var i = 0; i < handViews.Length; i++) {
			var handView = handViews[i];
			handViews[i].Position = randomCoords[i];
		}
	}
}
