using Godot;

public class PlayerJump
{
	private CharacterBody3D player;
	private PlayerAnimation animation;

	private bool enabled = true;
	private float jumpHeight = 1.0f;
	private float timeToPeak = 0.5f;
	private float timeToDescent = 0.35f;
	private float gravityUp, gravityDown, jumpVelocity;
	private bool wasOnFloor = false;
	private bool skipNextLandingAnim = true;
	
	public bool IsLanding { get; private set; } = false;

	public PlayerJump(CharacterBody3D player, PlayerAnimation animation)
	{
		this.player = player;
		this.animation = animation;
		
		gravityUp = (2 * jumpHeight) / (timeToPeak * timeToPeak);
		gravityDown = (2 * jumpHeight) / (timeToDescent * timeToDescent);
		jumpVelocity = Mathf.Sqrt(2 * gravityUp * jumpHeight);
		
		wasOnFloor = player.IsOnFloor();
		this.animation.AnimationFinished += OnAnimationFinished;
	}

	public void ApplyGravity(ref Vector3 velocity, double delta)
	{
		if (!enabled)
			return;
		
		bool onFloor = player.IsOnFloor();

		if (!onFloor)
			velocity.Y -= (velocity.Y > 0 ? gravityUp : gravityDown) * (float)delta;

		if (!wasOnFloor && onFloor)
		{
			if (!skipNextLandingAnim)
			{
				IsLanding = true;
				PlayShortJumpLand();
			}
				
			skipNextLandingAnim = false;
		}
		
		wasOnFloor = onFloor;
	}

	public void HandleJump(ref Vector3 velocity)
	{
		if (!enabled)
			return;
		
		if (Input.IsActionJustPressed("jump") && player.IsOnFloor())
		{
			velocity.Y = jumpVelocity;
			animation?.Play("Jump_Start_3", isOverride: true);
		}
	}
	
	private void OnAnimationFinished(string animationName)
	{
		if (animationName == "Jump_Start_3")
		{
			animation?.Play("Jump_Idle", isOverride: true);
		}
		else if (animationName == "Jump_End")
		{
			IsLanding = false;
		}
	}
	
	private async void PlayShortJumpLand()
	{
		animation?.Play("Jump_End", isOverride: true);
		animation?.GetAnimationPlayer()?.Seek(0);

		await player.ToSignal(player.GetTree().CreateTimer(0.5), "timeout");

		animation?.GetAnimationPlayer()?.Stop();
		IsLanding = false;
	}
	
	public void SetEnabled(bool value)
	{
		enabled = value;
	}
}
