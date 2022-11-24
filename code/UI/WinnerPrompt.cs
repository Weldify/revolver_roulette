namespace RevolverRoulette.UI;

public partial class WinnerPrompt
{
	public static WinnerPrompt Current { get; private set; }

	public Label WinnerLabel { get; set; }
	public Player Winner;

	private TimeSince timeSinceShown = 0f;

	public WinnerPrompt()
	{
		Current = this;
	}

	public void PlayerWon( Player plr )
	{
		Winner = plr;
		timeSinceShown = 0f;

		WinnerLabel.AddClass( "visible" );
	}

	public override void Tick()
	{
		if ( timeSinceShown < 4f || !WinnerLabel.HasClass( "visible" ) ) return;
		WinnerLabel.RemoveClass( "visible" );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Winner?.Client.Id );
	}
}