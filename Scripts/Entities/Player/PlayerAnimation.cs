using Godot;
using System;

public class PlayerAnimation
{
	private AnimationPlayer animationPlayer;
	private string currentAnimation = "";
	private string overrideAnimation = "";
	private bool isOverriding = false;
	
	public event Action<string> AnimationFinished;
	
	public PlayerAnimation(AnimationPlayer animationPlayer)
	{
		this.animationPlayer = animationPlayer;
		this.animationPlayer.AnimationFinished += OnAnimationFinished;
	}
	
	public AnimationPlayer GetAnimationPlayer()
	{
		return animationPlayer;
	}

	private void OnAnimationFinished(StringName animationName)
	{
		AnimationFinished?.Invoke(animationName);
	}

	public void Play(string animationName, bool isOverride = false)
	{
		if (isOverride)
		{
			if (overrideAnimation == animationName) return;
			overrideAnimation = animationName;
			isOverriding = true;
			animationPlayer?.Play(animationName);
		}
		else
		{
			if (currentAnimation == animationName || isOverriding) return;
			currentAnimation = animationName;
			animationPlayer?.Play(animationName);
		}
	}

	public void ClearOverride()
	{
		overrideAnimation = "";
		isOverriding = false;
	}

	public void Update(float delta)
	{
		if (isOverriding && !animationPlayer.IsPlaying())
		{
			ClearOverride();
			if (!string.IsNullOrEmpty(currentAnimation))
				animationPlayer.Play(currentAnimation);
		}
	}
}
