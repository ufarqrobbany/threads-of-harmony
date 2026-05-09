using Godot;
using System;

public partial class GameManager : Node3D
{
	private Player player;
	private GameOverUI gameOverUI;
	private PauseMenu pauseMenuUI;
	
	private bool gameOverTriggered = false;
	private bool isPaused = false;
	
	public override void _Ready()
	{
		player = GetNode<Player>("/root/Game/Entities/Player");
		gameOverUI = GetNode<GameOverUI>("/root/Game/UI/Popup/GameOverUI");
		pauseMenuUI = GetNode<PauseMenu>("/root/Game/UI/Popup/PauseMenu");
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("pause") && !gameOverTriggered)
		{
			TogglePause();
		}
	}
	
	private void TogglePause()
	{
		isPaused = true;
		GetTree().Paused = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;

		pauseMenuUI.ShowWithFade();
	}
	
	public void ResumeGame()
	{
		isPaused = false;
		GetTree().Paused = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;

		pauseMenuUI.HideWithFade();
	}
	
	public void GameOver()
	{
		if (gameOverTriggered) return;

		gameOverTriggered = true;

		Input.MouseMode = Input.MouseModeEnum.Visible;

		var timer = new Timer();
		timer.WaitTime = 2.0f;
		timer.OneShot = true;
		timer.Timeout += () =>
		{
			gameOverUI.ShowWithFade();
		};
		AddChild(timer);
		timer.Start();
	}
}
