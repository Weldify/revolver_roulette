namespace RevolverRoulette;

public partial class Player
{
	public bool HasBullet
    {
        get => Game.Current.BulletOwner == this;
    }

	public bool TakeBullet()
	{
		if ( HasBullet )
		{
			Game.Current.RerollBullet();
			return true;
		}

		return false;
	}
}