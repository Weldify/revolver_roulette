namespace RevolverRoulette;

internal class FreeFlyCamera : CameraMode
{
	public override void Activated()
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		Position = pawn.EyePosition;
		Rotation = pawn.EyeRotation;
	}

	public override void Update()
	{
		var pawn = Local.Pawn;
		if ( !pawn.IsValid() ) return;

		Rotation = pawn.EyeRotation;

		var dir = Rotation.Forward * Input.Forward - Rotation.Right * Input.Left;

		var helper = new MoveHelper( Position, dir.Normal * 600f );
		helper.Trace.Radius( 16f );

		if ( helper.TryMove( Time.Delta ) > 0f )
		{
			Position = helper.Position;
		}
	}
}