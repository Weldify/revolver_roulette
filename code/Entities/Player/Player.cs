namespace RevolverRoulette;

internal partial class Player : Sandbox.Player
{
	private TimeSince timeSinceDied = 0f;

	public Player()
	{
		Inventory = new BaseInventory( this );
	}

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen/citizen.vmdl" );

		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	public override void Respawn()
	{
		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
		CameraMode = new FirstPersonCamera();

		Inventory.Add( new Revolver(), true );

		EnableDrawing = true;
		EnableAllCollisions = true;

		base.Respawn();
	}

	public override void OnKilled()
	{
		timeSinceDied = 0f;

		Inventory.DeleteContents();

		EnableDrawing = false;
		EnableAllCollisions = false;

		base.OnKilled();
	}

	public override void Simulate( Client cl )
	{
		if ( LifeState != LifeState.Alive ) return;

		if ( Input.Pressed( InputButton.View ) )
		{
			CameraMode = CameraMode switch
			{
				FirstPersonCamera => new ThirdPersonCamera(),
				ThirdPersonCamera or _ => new FirstPersonCamera(),
			};
		}

		TickPlayerUse();
		SimulateActiveChild( cl, ActiveChild );

		var controller = GetActiveController();
		controller?.Simulate( cl, this, GetActiveAnimator() );
	}

	[Event.Tick.Server]
	public void OnTick()
	{
		if ( LifeState == LifeState.Dead && timeSinceDied > 3f )
		{
			Respawn();
			return;
		}
	}
}