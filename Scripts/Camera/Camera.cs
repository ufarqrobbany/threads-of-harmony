using Godot;
using System;

public partial class Camera : Camera3D
{
	[Export]
	public NodePath TargetNodePath { get; set; }
	
	[Export]
	public Vector3 Offset = new Vector3(0, 3, 5);
	
	[Export]
	public float SmoothSpeed = 5.0f;
	
	[Export]
	public float MinZoom = 2.0f;
	
	[Export]
	public float MaxZoom = 10.0f;
	
	[Export]
	public float ZoomStep = 0.5f;
	
	[Export]
	public float OrbitSensitivity = 0.01f; // radians per pixel moved
	
	private Node3D _target = null;
	
	public bool IsCameraControlEnabled { get; private set; } = true;
	
	public override void _Ready()
	{
		if (TargetNodePath != null)
			_target = GetNode<Node3D>(TargetNodePath);
		
		SetCameraControlEnabled(true);
	}

	public override void _Process(double delta)
	{
		if (_target != null)
		{
			Vector3 desiredPosition = _target.GlobalPosition + Offset;
			GlobalPosition = GlobalPosition.Lerp(desiredPosition, SmoothSpeed * (float)delta);
			LookAt(_target.GlobalPosition + Vector3.Up * 1.0f);

			Vector3 cameraForward = -GlobalTransform.Basis.Z;
			cameraForward.Y = 0;
			cameraForward = cameraForward.Normalized();

			if (cameraForward.LengthSquared() > 0.01f)
			{
				_target.LookAt(_target.GlobalPosition + cameraForward, Vector3.Up);
			}
		}
		
		Vector2 center = GetViewport().GetVisibleRect().Size / 2;
		Input.WarpMouse(center);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!IsCameraControlEnabled)
			return;
			
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.WheelUp && mouseButton.Pressed)
			{
				Offset.Z = Mathf.Max(Offset.Z - ZoomStep, MinZoom);
			}
			else if (mouseButton.ButtonIndex == MouseButton.WheelDown && mouseButton.Pressed)
			{
				Offset.Z = Mathf.Min(Offset.Z + ZoomStep, MaxZoom);
			}
		}

		if (@event is InputEventMouseMotion mouseMotion)
		{
			float angleDelta = -mouseMotion.Relative.X * OrbitSensitivity;
			Offset = Offset.Rotated(Vector3.Up, angleDelta);
		}
	}
	
	public void SetCameraControlEnabled(bool enabled)
	{
		IsCameraControlEnabled = enabled;
		Input.MouseMode = enabled ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
	}
}
