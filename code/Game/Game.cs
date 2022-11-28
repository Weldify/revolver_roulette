global using Sandbox;
global using Sandbox.UI;
global using System;
global using System.Linq;
global using System.Collections;
global using System.Collections.Generic;
global using RevolverRoulette.UI;

namespace RevolverRoulette;

public partial class Game : Sandbox.Game
{
	new public static Game Current { get; private set; }

	[Net, Change]
	public Player BulletOwner { get; private set; }

	private Player prevBulletOwner;

	private TimeSince timeSinceBulletReroll;

	public Game()
	{
		Current = this;

		if ( IsClient )
		{
			_ = new Hud();
		}
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		var plr = new Player();
		cl.Pawn = plr;
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

		Rand.SetSeed( Time.Tick );
		BulletOwner = Rand.FromList( eligible.ToList() );
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
}
