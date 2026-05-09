using Godot;
using System;

public interface ITalkable
{
	void ShowInteractionPrompt();
	void HideInteractionPrompt();
	void StartConversation();
}
