namespace RevolverRoulette.UI;

public partial class GameStateIndicator
{
	public static GameStateIndicator Current { get; private set; }

	private static string FormattedGameState
	{
		get => GameManager.Current.GameState switch
		{
			GameState.WaitingForPlayers => "Waiting for players",
			GameState.Intermission => "Intermission",
			GameState.Ongoing or _ => "Ongoing",
		};
	}

	public GameStateIndicator()
	{
		Current = this;
	}

	public void StateChanged()
	{
		PlaySound( "ui.click" );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( GameManager.Current.GameState );
	}
}
