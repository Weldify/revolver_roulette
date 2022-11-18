namespace RevolverRoulette;

internal partial class Player : Sandbox.Player
{
	private DamageInfo lastDamage;
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

		UpdateClothes();
		Dress();

		EnableDrawing = true;
		EnableAllCollisions = true;

		base.Respawn();
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( LifeState == LifeState.Dead )
			return;

		lastDamage = info;
		base.TakeDamage( info );
	}

	public override void OnKilled()
	{
		timeSinceDied = 0f;

		// So we don't hog the bullet while we're dead
		// or even worse, duplicate it when we spawn in!
		TakeBullet();

		Inventory.DeleteContents();

		BecomeRagdollOnClient( To.Everyone, lastDamage.Force, lastDamage.BoneIndex );
		CameraMode = new SpectateRagdollCamera();

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