namespace RevolverRoulette;

partial class ViewModel : BaseViewModel
{
	float walkBob = 0;

	public override void PlaceViewmodel()
	{
		// nothing
	}

	public void UpdateCamera()
	{
		var rotationDistance = Rotation.Distance( Camera.Rotation );

		Position = Camera.Position;
		Rotation = Camera.Rotation;

		if ( Game.LocalPawn.LifeState == LifeState.Dead )
			return;

		//
		// Bob up and down based on our walk movement
		//
		var speed = Game.LocalPawn.Velocity.Length.LerpInverse( 0, 400 );
		var left = Camera.Rotation.Left;
		var up = Camera.Rotation.Up;

		if ( Game.LocalPawn.GroundEntity != null )
		{
			walkBob += Time.Delta * 30.0f * speed;
		}

		Position += up * MathF.Sin( walkBob ) * speed * -0.6f;
		Position += left * MathF.Sin( walkBob * 0.5f ) * speed * -0.3f;
	}
}
