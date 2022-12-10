namespace RevolverRoulette;

public partial class Player : SandboxPlayer
{
	private DamageInfo lastDamage;

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

		// Freshly spawned pawns are Alive,
		// which causes the game to think they're participating in the match.
		Despawn();
	}

	public void Despawn( bool spectate = false )
	{
		Inventory?.DeleteContents();

		LifeState = LifeState.Respawnable;

		SetVisibility( false );

		IsSpectating = true;
	}

	public override void Respawn()
	{
		Controller = new MovementController
		{
			SprintSpeed = 200f,
			WalkSpeed = 150f,
			DefaultSpeed = 200f,
		};

		Inventory.DeleteContents();
		Inventory.Add( new Revolver(), true );

		TakeBullet();

		UpdateClothes();
		Dress();

		base.Respawn();

		SetVisibility( true );
		IsSpectating = false;
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

		Despawn();

		BecomeRagdollOnClient( To.Everyone, lastDamage.Force, lastDamage.BoneIndex );

		base.OnKilled();
	}

	public override void Simulate( IClient cl )
	{
		if ( LifeState != LifeState.Alive ) return;

		TickPlayerUse();
		DoPlayerAnimation();
		SimulateActiveChild( cl, ActiveChild );

		var controller = GetActiveController();
		controller?.Simulate( cl, this );
	}

	[Event.Tick.Client]
	public void OnClientTick()
	{
		ResolveCamera();
	}

	public override void FrameSimulate( IClient cl )
	{
		UpdateCamera();
	}

	public void SetVisibility( bool visible )
	{
		EnableDrawing = visible;
		EnableAllCollisions = visible;
	}
}
