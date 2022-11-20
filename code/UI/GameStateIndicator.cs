namespace RevolverRoulette.UI;

public partial class GameStateIndicator
{
	private string FormattedGameState
	{
		get => Game.Current.GameState switch
		{
			GameState.WaitingForPlayers => "Waiting for players",
            GameState.Intermission => "Intermission",
            GameState.Ongoing or _ => "Ongoing",
		};
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Game.Current.GameState );
	}
}