using Godot;
using System;

public interface CardObserver {
	void NotifyClicked(CardView c);
}
