namespace RevolverRoulette;

public enum GameState
{
	WaitingForPlayers,
	Ongoing,
	Intermission,
}

public partial class Game
{
	[Net, Change]
	public GameState GameState { get; set; } = GameState.WaitingForPlayers;

	private TimeUntil stateTimer;

	private void RespawnPlayers()
	{
		foreach ( var plr in Entity.All.OfType<Player>().ToList() )
			plr.Respawn();
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
					var living = Entity.All.OfType<Player>().Where(
						p => p.LifeState == LifeState.Alive
					);

					if ( living.Count() <= 1 )
					{
						GameState = GameState.Intermission;
						stateTimer = 5f;

						Game.ClientTellWinner( To.Everyone, living.First() );

						continue;
					}

					break;
			}

			break;
		}
	}

	public void OnGameStateChanged(GameState _, GameState state)
	{
		GameStateIndicator.Current.StateChanged();
	}
}