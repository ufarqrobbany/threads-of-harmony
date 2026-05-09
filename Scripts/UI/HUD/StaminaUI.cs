using Godot;
using System;

public partial class StaminaUI : Control
{
	[Export] private Texture2D normalTexture;
	[Export] private Texture2D lowStaminaTexture;
	
	private TextureProgressBar staminaBar;
	private float visibleDuration = 1f;
	private float hideTimer = 0f;
	private bool isLowStamina = false;
	
	public override void _Ready()
	{
		staminaBar = GetNode<TextureProgressBar>("Node2D/StaminaBar");
		staminaBar.Visible = false;
		
		if (normalTexture != null)
			staminaBar.TextureProgress = normalTexture;
	}

	public void ShowStamina()
	{
		staminaBar.Visible = true;
		hideTimer = visibleDuration;
	}
	
	public void UpdateStamina(float current, float max)
	{
		staminaBar.MaxValue = max;
		staminaBar.Value = current;
		
		float percent = current / max;

		if (percent < 0.25f && !isLowStamina)
		{
			if (lowStaminaTexture != null)
				staminaBar.TextureProgress = lowStaminaTexture;

			isLowStamina = true;
		}
		else if (percent >= 0.25f && isLowStamina)
		{
			if (normalTexture != null)
				staminaBar.TextureProgress = normalTexture;

			isLowStamina = false;
		}
	}

	public override void _Process(double delta)
	{
		if (staminaBar.Visible)
		{
			hideTimer -= (float)delta;
			if (hideTimer <= 0f)
				staminaBar.Visible = false;
		}
	}
}
