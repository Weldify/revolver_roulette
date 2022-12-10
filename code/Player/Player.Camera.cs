namespace RevolverRoulette;

public partial class Player
{
	private ICameraMode _cameraMode;

	public ICameraMode CameraMode
	{
		get => _cameraMode;
		set
		{
			_cameraMode = value;
			value.Activated();
		}
	}
	
	private void UpdateCamera()
	{
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );
		
		CameraMode?.Update();
		
		if ( ActiveChild is BaseWeapon weapon )
		{
			weapon.UpdateViewmodelCamera();
			weapon.UpdateCamera();
		}
	}

	float walkBob = 0;
	float lean = 0;
	float fov = 0;

	private void AddCameraEffects()
	{
		var speed = Velocity.Length.LerpInverse( 0, 320 );
		var forwardspeed = Velocity.Normal.Dot( Camera.Rotation.Forward );

		var left = Camera.Rotation.Left;
		var up = Camera.Rotation.Up;

		if ( GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		Camera.Position += up * MathF.Sin( walkBob ) * speed * 2;
		Camera.Position += left * MathF.Sin( walkBob * 0.6f ) * speed * 1;

		// Camera lean
		lean = lean.LerpTo( Velocity.Dot( Camera.Rotation.Right ) * 0.01f, Time.Delta * 15.0f );

		var appliedLean = lean;
		appliedLean += MathF.Sin( walkBob ) * speed * 0.3f;
		Camera.Rotation *= Rotation.From( 0, 0, appliedLean );

		speed = (speed - 0.7f).Clamp( 0, 1 ) * 3.0f;

		fov = fov.LerpTo( speed * 20 * MathF.Abs( forwardspeed ), Time.Delta * 4.0f );

		Camera.FieldOfView += fov;
	}
}
