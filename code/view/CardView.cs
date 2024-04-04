using Godot;
using System;

public partial class CardView : TextureRect
{
	Card card;
	bool active;
	CardObserver observer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//active = false;
		//updateView();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetObserver(CardObserver c) {
		this.observer = c;
	}
	
	public void Deactivate() {
		this.active = false;
		updateView();
	}	

	public void SetCard(Card card) {
		this.card = card;
		this.active = true;
		updateView();
	}
	
	public Card GetCard() {
		return this.card;
	}
	
	private void updateView() {
		if (this.active) {
			// GD.Print(this.card);
			this.Texture = ImageLibrary.GetCardImage(this.card);
			//this.MouseFilter = MouseFilterEnum.MOUSE_FILTER_STOP;
			//this.MouseFilter = (MouseFilterEnum)0;
			this.Visible = true;
		} else {
			this.Visible = false;
			// NEVERMIND ALL THIS WAS FIXED WHEN I STOPPED USING CONTROL/MARGIN/VBOX CONTAINERS
			// VISIBLE TRUE/FALSE BREAKS POSITIONING FOR SOME REASON. I DONT KNOW WHY
			// IDK HOW TO USE THE ACTUAL ENUMS, INTELLISENSE DOESNT WORK FOR BEANS
			//this.MouseFilter = (MouseFilterEnum)2;
			//this.MouseFilter = MouseFilterEnum.MOUSE_FILTER_IGNORE;
			
		}
	}
	
	public int GetCardSize() {
		// GD.Print(this.Texture.GetWidth());
		return this.Texture.GetWidth();
	}
	
	private void _on_gui_input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventButton) {
			if (eventButton.Pressed) {
				if (this.observer != null) {
					observer.NotifyClicked(this);
				}
			}
		}
	}
}


