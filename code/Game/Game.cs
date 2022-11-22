global using Sandbox;
global using Sandbox.UI;
global using System;
global using System.Linq;
global using System.Collections;
global using System.Collections.Generic;
global using RevolverRoulette.UI;

namespace RevolverRoulette;

internal partial class Game : Sandbox.Game
{
	new public static Game Current;

	private Player prevBulletOwner;

	private Hud hud;

	public Game()
	{
		Current = this;

		if ( IsClient )
			hud = new Hud();
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		var plr = new Player();
		cl.Pawn = plr;
	}

	private Player GetNextBulletOwner( IEnumerable<Player> players )
	{
		Rand.SetSeed( Time.Tick );

		var set = players.ToHashSet();
		set.Remove( prevBulletOwner );

		var pos = Rand.Int( set.Count );
		if ( pos >= set.Count ) return null;

		return set.ElementAt( pos );
	}

	public void DistributeBullet()
	{
		var eligiblePlayers = Entity.All.OfType<Player>().Where(
			p => p.IsValid() && p.LifeState == LifeState.Alive
		);

		var noBullet = eligiblePlayers.All( p => !p.HasBullet );
		if ( !noBullet ) return;

		Player plr;

		plr = eligiblePlayers.Count() switch
		{
			1 => eligiblePlayers.First(),
			_ => GetNextBulletOwner( eligiblePlayers ),
		};

		if ( plr == null ) return;

		plr.HasBullet = true;
		prevBulletOwner = plr;

		BulletRerollNotifyClient( To.Everyone );
	}

	[Event.Tick.Server]
	public void OnTick()
	{
		TickState();
		DistributeBullet();
	}

	[ClientRpc]
	public static void BulletRerollNotifyClient()
	{
		MagIndicator.Current?.Spin();
	}
}
