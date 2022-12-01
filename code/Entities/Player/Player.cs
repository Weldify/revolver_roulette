namespace RevolverRoulette;

public partial class Player : Sandbox.Player
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

		// Try to start spectating a player

		var validPlayers = All.OfType<Player>().Where( p => p.LifeState == LifeState.Alive );
		var randPlayer = Rand.FromList( validPlayers.ToList() );

		if ( randPlayer.IsValid() )
		{
			TryBeginSpectating( To.Single( Client ), EyePosition, true );
			return;
		}

		// Fallback to spawnpoint if no player is found

		var spawnPoints = All.OfType<SpawnPoint>();
		var randSpawnPoint = Rand.FromList( spawnPoints.ToList() );

		if ( randSpawnPoint.IsValid() )
		{
			TryBeginSpectating( To.Single( Client ), EyePosition, true );
			return;
		}

		TryBeginSpectating( To.Single( Client ), EyePosition, true );
	}

	public void Despawn( bool spectate = false )
	{
		Inventory?.DeleteContents();

		LifeState = LifeState.Respawnable;

		SetVisibility( false );

		if ( spectate )
		{
			TryBeginSpectating( To.Single( Client ), EyePosition, true );
		}
	}

	public override void Respawn()
	{
		Controller = new WalkController
		{
			SprintSpeed = 200f,
			WalkSpeed = 150f,
			DefaultSpeed = 200f,
		};

		Animator = new StandardPlayerAnimator();
		CameraMode = new FirstPersonCamera();

		Inventory.DeleteContents();
		Inventory.Add( new Revolver(), true );

		TakeBullet();

		UpdateClothes();
		Dress();

		base.Respawn();

		SetVisibility( true );
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
		CameraMode = new SpectateRagdollCamera();

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

	public void SetVisibility( bool visible )
	{
		EnableDrawing = visible;
		EnableAllCollisions = visible;
	}

	[Event.Tick.Client]
	public void OnClientTick()
	{
		TryBeginSpectating( EyePosition );
	}
}
