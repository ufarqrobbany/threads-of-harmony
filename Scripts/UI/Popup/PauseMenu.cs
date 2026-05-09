using Godot;
using System;

public partial class PauseMenu : Control
{
	private Control panel;
	private AnimationPlayer animation;
	private TextureButton resumeButton;
	private TextureButton returnButton;

	[Export] public string ReturnScenePath { get; set; } = "res://Scenes/MainMenu.tscn";

	public override void _Ready()
	{
		panel = GetNode<Control>("FadeOverlay");
		animation = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		resumeButton = GetNode<TextureButton>("FadeOverlay/Buttons/ResumeButton");
		returnButton = GetNode<TextureButton>("FadeOverlay/Buttons/ExitButton");

		panel.Visible = false;
		this.Visible = false;
		SetProcessMode(ProcessModeEnum.Always);

		resumeButton.Pressed += OnResumePressed;
		returnButton.Pressed += OnReturnPressed;

		if (animation != null)
		{
			animation.AnimationFinished += OnAnimationFinished;
		}
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

	public void HideWithFade()
	{
		if (animation != null)
		{
			animation.Play("fade_out");
		}
		else
		{
			panel.Visible = false;
			this.Visible = false;
			GetTree().Paused = false;
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName == "fade_out")
		{
			panel.Visible = false;
			GetTree().Paused = false;
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	private void OnResumePressed()
	{
		if (GetTree().Root.GetNodeOrNull<GameManager>("/root/Game") is GameManager gm)
		{
			gm.ResumeGame();
		}
	}

	private void OnReturnPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile(ReturnScenePath);
	}
}
