using Godot;
using System;

public partial class HealthUI : Control
{
	[Export] private Texture2D normalTexture;
	[Export] private Texture2D lowHealthTexture;
	
	private TextureProgressBar healthBar;
	private Label healthText;
	private bool isLowHealth = false;
	
	private float displayedHealth;
	private float targetHealth;

	private const float LERP_SPEED = 5.0f;
	
	public override void _Ready()
	{
		healthBar = GetNode<TextureProgressBar>("Node2D/HealthBar");
		healthText = GetNode<Label>("Node2D/HealthText");
		
		if (normalTexture != null)
			healthBar.TextureProgress = normalTexture;
	}
	
	public override void _Process(double delta)
	{
		displayedHealth = Mathf.Lerp(displayedHealth, targetHealth, (float)delta * LERP_SPEED);
		healthBar.Value = displayedHealth;
		
		float percent = (float)displayedHealth / (float)healthBar.MaxValue;

		if (percent < 0.25f && !isLowHealth)
		{
			if (lowHealthTexture != null)
				healthBar.TextureProgress = lowHealthTexture;

			isLowHealth = true;
		}
		else if (percent >= 0.25f && isLowHealth)
		{
			if (normalTexture != null)
				healthBar.TextureProgress = normalTexture;

			isLowHealth = false;
		}
	}
	
	public void UpdateHealth(float current, float max)
	{
		healthBar.MaxValue = max;
		targetHealth = current;
				
		if (healthText != null)
			healthText.Text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
	}
}
