using Godot;
using System;

public partial class MainMenu : Node
{
	[Export] public TextureButton NewGameButton;
	[Export] public TextureButton ContinueButton;
	[Export] public TextureButton SettingsButton;
	[Export] public TextureButton CreditsButton;
	[Export] public TextureButton ExitButton;
	[Export] public string Prolog = "res://Scenes/Prolog.tscn";
	[Export] public string Continue = "res://Scenes/World/Dungeon.tscn";
	[Export] public string Settings = "res://Scenes/Settings.tscn";
	[Export] public string Credits = "res://Scenes/Credits.tscn";

	public override void _Ready()
	{
		NewGameButton.Pressed += OnNewGameButtonPressed;
		ContinueButton.Pressed += OnContinueButtonPressed;
		SettingsButton.Pressed += OnSettingsButtonPressed;
		CreditsButton.Pressed += OnCreditsButtonPressed;
		ExitButton.Pressed += OnExitButtonPressed;
	}

	private void OnNewGameButtonPressed()
	{
		GetTree().ChangeSceneToFile(Prolog);
	}
	
	private void OnContinueButtonPressed()
	{
		GetTree().ChangeSceneToFile(Continue);
	}
	
	private void OnSettingsButtonPressed()
	{
		GetTree().ChangeSceneToFile(Settings);
	}
	
	private void OnCreditsButtonPressed()
	{
		GetTree().ChangeSceneToFile(Credits);
	}
	
	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}

}
