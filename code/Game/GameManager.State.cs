namespace RevolverRoulette;

public enum GameState
{
	WaitingForPlayers,
	Ongoing,
	Intermission,
}

public partial class GameManager
{
	[Net, Change]
	public GameState GameState { get; set; } = GameState.WaitingForPlayers;

	private TimeUntil stateTimer;

	private void RespawnPlayers()
	{
		var spawnPoints = All.OfType<SpawnPoint>()
			.OrderBy( x => Guid.NewGuid() );

		var spawnPointCount = spawnPoints.Count();

		var players = All.OfType<Player>();

		for ( int i = 0; i < players.Count(); i++ )
		{
			var plr = players.ElementAt( i );

			plr.Respawn();

			// Does the map have spawn points? 
			if ( spawnPointCount < 1 ) continue;

			var spawnPoint = spawnPoints.ElementAt( i % spawnPointCount );
			plr.Transform = spawnPoint.Transform;

			plr.ResetInterpolation();
		}
	}

	void DespawnPlayers()
	{
		var livePlayers = All.OfType<Player>().Where( p => p.LifeState == LifeState.Alive );

		foreach ( var plr in livePlayers )
		{
			plr.Despawn( true );
		}
	}

	public void TickState()
	{
		// STATE MACHINE!???
		while ( true )
		{
			switch ( GameState )
			{
				case GameState.Intermission:
					if ( stateTimer < 0f )
					{
						DespawnPlayers();

						GameState = GameState.WaitingForPlayers;
						continue;
					}

					break;
				case GameState.WaitingForPlayers:
					var players = Entity.All.OfType<Player>();

					if ( players.Count() >= 2 )
					{
						RespawnPlayers();
						GameState = GameState.Ongoing;
						continue;
					}

					break;
				case GameState.Ongoing:
					var living = Entity.All.OfType<Player>()
						.Where( p => p.LifeState == LifeState.Alive );

					if ( living.Count() <= 1 )
					{
						GameState = GameState.Intermission;
						stateTimer = 5f;

						ClientTellWinner( To.Everyone, living.First() );

						continue;
					}

					break;
			}

			break;
		}
	}

	public void OnGameStateChanged( GameState _, GameState state )
	{
		GameStateIndicator.Current.StateChanged();
	}
}
