using Godot;
using System;

public partial class PlayerCameraController : SpringArm3D
{
	[Export] public NodePath PlayerPath { get; set; }
	[Export] public float SmoothSpeed = 10.0f;
	[Export] public float OrbitSensitivity = 0.005f;
	
	private bool enabled = true;

	private Node3D _player = null;
	private Vector3 initialOffset = new Vector3(0, 3, 0);
	private float yaw = 0f;
	private float pitch = 0f;
	private float distance;
	private const float minZoom = 1.5f;
	private const float maxZoom = 10.0f;
	private const float zoomStep = 0.5f;
	private const float minVerticalAngleDeg = 0f;
	private float maxVerticalAngleDeg = 60f;
	private bool isAltHeld = false;

	public bool IsCameraControlEnabled { get; private set; } = true;

	public override void _Ready()
	{
		if (PlayerPath != null)
			_player = GetNode<Node3D>(PlayerPath);
		
		SetCameraControlEnabled(true);
		InitializeRotationAnddistance();
	}

	public override void _Process(double delta)
	{
		if (_player == null || !enabled)
			return;

		Vector3 offset = CalculateOffset();
		Vector3 desiredPosition = _player.GlobalPosition + offset;
		
		if (SmoothSpeed > 0f)
		{
			GlobalPosition = GlobalPosition.Lerp(desiredPosition, SmoothSpeed * (float)delta);
		}
		else 
		{
			GlobalPosition = desiredPosition;
		}
		LookAt(_player.GlobalPosition + Vector3.Up * 1.0f);
	}

	public override void _Input(InputEvent @event)
	{
		if (!enabled)
			return;
		
		if (@event is InputEventKey keyEvent)
			HandleAltKey(keyEvent);
			
		if (!IsCameraControlEnabled)
			return;

		if (@event is InputEventMouseButton mouseButton)
		{
			HandleZoom(mouseButton);
		}

		if (@event is InputEventMouseMotion mouseMotion)
		{
			UpdateRotation(mouseMotion.Relative);
		}
	}

	private void InitializeRotationAnddistance()
	{
		yaw = 0f; 
		pitch = Mathf.DegToRad(20f);

		distance = SpringLength;

		Clamppitch();
	}

	private void Clamppitch()
	{
		float minpitch = Mathf.DegToRad(minVerticalAngleDeg);
		float maxpitch = Mathf.DegToRad(maxVerticalAngleDeg);
		pitch = Mathf.Clamp(pitch, minpitch, maxpitch);
	}

	private Vector3 CalculateOffset()
	{
		float x = distance * Mathf.Sin(yaw) * Mathf.Cos(pitch);
		float y = distance * Mathf.Sin(pitch);
		float z = distance * Mathf.Cos(yaw) * Mathf.Cos(pitch);

		return new Vector3(x, y, z);
	}

	private void UpdateRotation(Vector2 relativeMotion)
	{
		yaw -= relativeMotion.X * OrbitSensitivity;
		pitch += relativeMotion.Y * OrbitSensitivity;
		
		Clamppitch();
	}

	private void HandleZoom(InputEventMouseButton mouseButton)
	{
		if (mouseButton.ButtonIndex == MouseButton.WheelUp && mouseButton.Pressed)
		{
			distance = Mathf.Max(distance - zoomStep, minZoom);
		}
		else if (mouseButton.ButtonIndex == MouseButton.WheelDown && mouseButton.Pressed)
		{
			distance = Mathf.Min(distance + zoomStep, maxZoom);
		}
		
		if (distance <= 2f)
		{
			maxVerticalAngleDeg = 70f;
		}
		else
		{
			maxVerticalAngleDeg = 60f;
		}
	}
	
	public void SetCameraControlEnabled(bool enabled)
	{
		IsCameraControlEnabled = enabled;
		Input.MouseMode = enabled ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
		
		if (IsCameraControlEnabled)
		{
			Vector2 center = GetViewport().GetVisibleRect().Size / 2;
			Input.WarpMouse(center);
		}
	}
	
	private void HandleAltKey(InputEventKey keyEvent)
	{
		if (keyEvent.Keycode != Key.Alt)
			return;

		if (keyEvent.Pressed && !keyEvent.Echo)
		{
			isAltHeld = true;
			SetCameraControlEnabled(false);
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else if (!keyEvent.Pressed)
		{
			isAltHeld = false;
			SetCameraControlEnabled(true);
		}
	}
	
	public void SetEnabled(bool value)
	{
		enabled = value;
	}
}
