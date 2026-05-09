using Godot;
using System;

public partial class Settings : Node
{
	[Export] public TextureButton BackButton;
	[Export] public string MainMenu = "res://Scenes/MainMenu.tscn";

	public override void _Ready()
	{
		BackButton.Pressed += OnBackButtonPressed;
	}

	private void OnBackButtonPressed()
	{
		GetTree().ChangeSceneToFile(MainMenu);
	}
}
