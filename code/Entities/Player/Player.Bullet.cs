namespace RevolverRoulette;

public partial class Player
{
	[Net]
	public bool HasBullet { get; set; } = false;

	public bool TakeBullet()
	{
		if ( !HasBullet ) return false;
        HasBullet = false;

        return true;
	}
}