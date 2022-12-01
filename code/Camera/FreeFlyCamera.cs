namespace RevolverRoulette;

internal class FreeFlyCamera : CameraMode
{
	const float DEFAULT_SPEED = 400f;
	const float FAST_SPEED = 700f;

	public override void Activated()
	{
		var plr = Local.Pawn as Player;
		if ( plr == null ) return;

		Position = plr.EyePosition;
		Rotation = Input.Rotation;

		if ( !plr.FirstTimeSpectator ) return;
		plr.FirstTimeSpectator = false;

		Position = plr.SpectatorOrigin;
	}

	public override void Update()
	{
		var pawn = Local.Pawn;
		if ( !pawn.IsValid() ) return;

		Rotation = Input.Rotation;

		var up = Convert.ToSingle( Input.Down( InputButton.Jump ) ) - Convert.ToSingle( Input.Down( InputButton.Duck ) );
		var dir = Rotation.Forward * Input.Forward + Rotation.Right * -Input.Left + Rotation.Up * up;

		var speed = Input.Down( InputButton.Run ) ? FAST_SPEED : DEFAULT_SPEED;

		Position += dir.Normal * speed * Time.Delta;
	}
}
