using Godot;
using System;

public partial class GameOverUI : Control
{
	private Control panel;
	private AnimationPlayer animation;
	private TextureButton continueButton;
	private TextureButton returnButton;

	[Export] public string ReturnScenePath { get; set; } = "res://Scenes/MainMenu.tscn";

	public override void _Ready()
	{
		panel = GetNode<Control>("FadeOverlay");
		animation = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		continueButton = GetNode<TextureButton>("FadeOverlay/Container/Buttons/ContinueButton");
		returnButton = GetNode<TextureButton>("FadeOverlay/Container/Buttons/ReturnButton");

		panel.Visible = false;
		this.Visible = false;
		SetProcessMode(ProcessModeEnum.Always);

		continueButton.Pressed += OnContinuePressed;
		returnButton.Pressed += OnReturnPressed;
	}

	public void ShowWithFade()
	{
		panel.Visible = true;
		this.Visible = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetTree().Paused = true;

		if (animation != null)
			animation.Play("fade_in");
		else
			Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1.0f);
	}

	private void OnContinuePressed()
	{
		GD.Print("Continue Pressed");
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	private void OnReturnPressed()
	{
		GD.Print("Return Pressed");
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile(ReturnScenePath);
	}
}
