namespace RevolverRoulette;

internal partial class Player : Sandbox.Player
{
	private DamageInfo lastDamage;

	public Player()
	{
		Inventory = new BaseInventory( this );
	}

	public override void Spawn()
	{
		base.Spawn();

		// Freshly spawned pawns are Alive,
		// which causes the game to think they're participating in the match.
		LifeState = LifeState.Respawnable;

		SetModel( "models/citizen/citizen.vmdl" );

		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	public override void Respawn()
	{
		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
		CameraMode = new FirstPersonCamera();

		Inventory.DeleteContents();
		Inventory.Add( new Revolver(), true );

		TakeBullet();

		//UpdateClothes();
		//Dress();

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

		TickPlayerUse();
		SimulateActiveChild( cl, ActiveChild );

		var controller = GetActiveController();
		controller?.Simulate( cl, this, GetActiveAnimator() );
	}

	[Event.Tick.Client]
	public void OnClientTick()
	{
		SwapRagdollCamera();
	}
}