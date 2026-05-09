using Godot;

public class PlayerStamina
{
	private float maxStamina = 10f;
	private float stamina = 10f;
	private float drain = 5f;
	private float regen = 2f;
	private float regenDelay = 1.5f;
	private float regenTimer = 0f;
	private bool empty = false;

	public bool IsSprinting() => Input.IsActionPressed("sprint");
	public float GetCurrentStamina() => stamina;
	public float GetMaxStamina() => maxStamina;

	public void HandleStamina(double delta, bool isMoving)
	{
		if (Input.IsActionPressed("sprint") && isMoving)
		{
			stamina = Mathf.Max(stamina - drain * (float)delta, 0);
			if (stamina <= 0 && !empty)
			{
				empty = true;
				regenTimer = 0;
			}
		}
		else
		{
			if (empty)
			{
				regenTimer += (float)delta;
				if (regenTimer >= regenDelay)
					empty = false;
			}

			if (!empty)
				stamina = Mathf.Min(stamina + regen * (float)delta, maxStamina);
		}
	}
}
