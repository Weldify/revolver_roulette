namespace RevolverRoulette;

public class FirstPersonCamera : ICameraMode
{
	public void Activated()
	{
		
	}

	public void Update()
	{
		var plr = Game.LocalPawn as Player;

		Camera.Position = plr.EyePosition;
		Camera.Rotation = plr.ViewAngles.ToRotation();
		Camera.FirstPersonViewer = plr;

		Camera.Main.SetViewModelCamera( 66f );
		AddCameraEffects();
	}

	float walkBob = 0;
	float lean = 0;
	float fov = 0;

	private void AddCameraEffects()
	{
		var plr = Game.LocalPawn as Player;
		
		var speed = plr.Velocity.Length.LerpInverse( 0, 320 );
		var forwardspeed = plr.Velocity.Normal.Dot( Camera.Rotation.Forward );

		var left = Camera.Rotation.Left;
		var up = Camera.Rotation.Up;

		if ( plr.GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		Camera.Position += up * MathF.Sin( walkBob ) * speed * 2;
		Camera.Position += left * MathF.Sin( walkBob * 0.6f ) * speed * 1;

		// Camera lean
		lean = lean.LerpTo( plr.Velocity.Dot( Camera.Rotation.Right ) * 0.01f, Time.Delta * 15.0f );

		var appliedLean = lean;
		appliedLean += MathF.Sin( walkBob ) * speed * 0.3f;
		Camera.Rotation *= Rotation.From( 0, 0, appliedLean );

		speed = (speed - 0.7f).Clamp( 0, 1 ) * 3.0f;

		fov = fov.LerpTo( speed * 20 * MathF.Abs( forwardspeed ), Time.Delta * 4.0f );

		Camera.FieldOfView += fov;
	}
}
