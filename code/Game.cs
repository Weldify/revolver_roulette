global using Sandbox;
global using Sandbox.UI;
global using System;
global using System.Linq;
global using RevolverRoulette.UI;

namespace RevolverRoulette;

internal partial class Game : Sandbox.Game
{
	new public static Game Current;

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

		plr.Respawn();
	}

	public static void RerollBullet()
	{
		foreach ( var plr in Entity.All.OfType<Player>() )
		{
			if ( !plr.IsValid() ) continue;

			//TODO Give bullet to random player
			plr.HasBullet = true;
		}

		BulletRerolled();
	}

	[ClientRpc]
	public static void BulletRerolled()
	{
		MagIndicator.Current?.Spin();
	}
}
