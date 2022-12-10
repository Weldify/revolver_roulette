global using Sandbox;
global using Sandbox.UI;
global using System;
global using System.Linq;
global using System.Collections;
global using System.Collections.Generic;
global using RevolverRoulette.UI;

namespace RevolverRoulette;

public partial class GameManager : Sandbox.GameManager
{
	public new static GameManager Current { get; private set; }

	[Net, Change] public Player BulletOwner { get; private set; }

	private Player prevBulletOwner;

	private TimeSince timeSinceBulletReroll;

	public GameManager()
	{
		Current = this;

		if ( IsClient )
		{
			_ = new Hud();
		}
	}

	public override void ClientJoined( IClient cl )
	{
		base.ClientJoined( cl );

		var plr = new Player();
		cl.Pawn = plr;

		plr.SpectatorOrigin = GetRandomSpectatorOrigin();
	}

	public void RerollBullet()
	{
		prevBulletOwner = BulletOwner;
		BulletOwner = null;
	}

	public void EnsureBullet()
	{
		// So that a player who is AFK/trolling can't hog the bullet forever
		if ( BulletOwner.IsValid() && timeSinceBulletReroll < 20f ) return;
		if ( GameState == GameState.WaitingForPlayers ) return;

		timeSinceBulletReroll = 0f;

		// Normally this is set in RerollBullet, but we need to
		// do this here too if we want to take a bullet away forcefully
		if ( BulletOwner.IsValid() )
			prevBulletOwner = BulletOwner;

		var eligible = All.OfType<Player>().Where( p => p != prevBulletOwner && p.LifeState == LifeState.Alive );

		Game.SetRandomSeed( Time.Tick );
		BulletOwner = Game.Random.FromList( eligible.ToList() );
	}

	[Event.Tick.Server]
	public void OnServerTick()
	{
		TickState();
		EnsureBullet();
	}

	[Event.Tick.Client]
	public void OnClientTick()
	{
		TickMusic();
	}

	public void OnBulletOwnerChanged( Player _, Player owner )
	{
		MagIndicator.Current?.Spin();
	}

	[ClientRpc]
	public static void ClientTellWinner( Player plr )
	{
		WinnerPrompt.Current?.PlayerWon( plr );
	}

	// Where to put the spectator camera
	// The first time they join the game
	Vector3 GetRandomSpectatorOrigin()
	{
		var spawnPoints = All.OfType<SpawnPoint>();
		if ( spawnPoints.Count() < 1 )
			return Vector3.Zero;

		var randSpawn = spawnPoints
			.OrderBy( x => Guid.NewGuid() )
			.First();

		return randSpawn.Position + Vector3.Up * 80f;
	}
}
