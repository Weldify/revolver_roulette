namespace RevolverRoulette;

internal partial class Player
{
	[Net]
	public bool HasBullet { get; set; } = true;

	public bool TakeBullet()
	{
		if ( !HasBullet ) return false;
        
        HasBullet = false;
        Game.RerollBullet();

        return true;
	}
}