namespace RevolverRoulette;

internal class FreeFlyCamera : CameraMode
{
	const float DEFAULT_SPEED = 400f;
	const float FAST_SPEED = 700f;

	public override void Activated()
	{
		var plr = Local.Pawn as Player;

		Position = plr.EyePosition;
		Rotation = plr.ViewAngles.ToRotation();

		if ( !plr.FirstTimeSpectator ) return;
		plr.FirstTimeSpectator = false;

		Position = plr.SpectatorOrigin;
	}

	public override void Update()
	{
		var plr = Local.Pawn as Player;

		Rotation = plr.ViewAngles.ToRotation();

		var up = Convert.ToSingle( Input.Down( InputButton.Jump ) ) - Convert.ToSingle( Input.Down( InputButton.Duck ) );

		var dir = new Vector3( plr.InputDirection.x.Clamp( -1f, 1f ), plr.InputDirection.y.Clamp( -1f, 1f ), up ).Normal;
		dir *= Rotation;

		var speed = Input.Down( InputButton.Run ) ? FAST_SPEED : DEFAULT_SPEED;

		Position += dir.Normal * speed * Time.Delta;
	}
}
