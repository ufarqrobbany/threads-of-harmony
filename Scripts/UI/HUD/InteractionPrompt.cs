using Godot;
using System;

public partial class InteractionPrompt : Control
{
	private Label keyLabel;
	private Label promptLabel;
		
	public override void _Ready()
	{
		keyLabel = GetNode<Label>("HBoxContainer/LabelKey");
		promptLabel = GetNode<Label>("HBoxContainer/LabelText");
		Visible = false;
	}

	public void ShowPrompt(string key, string text)
	{
		keyLabel.Text = key;
		promptLabel.Text = text;
		Visible = true;
	}

	public void HidePrompt()
	{
		Visible = false;
	}
}
