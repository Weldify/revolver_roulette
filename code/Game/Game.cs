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

	public Game()
	{
		Current = this;

		if ( IsClient )
			_ = new Hud();
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
		if ( BulletOwner.IsValid() || GameState == GameState.WaitingForPlayers ) return;

		var eligible = All.OfType<Player>().Where( p => p != prevBulletOwner && p.LifeState == LifeState.Alive );

		Rand.SetSeed( Time.Tick );
		BulletOwner = Rand.FromList( eligible.ToList() );
	}

	[Event.Tick.Server]
	public void OnTick()
	{
		TickState();
		EnsureBullet();
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
