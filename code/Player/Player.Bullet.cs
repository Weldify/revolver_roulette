namespace RevolverRoulette;

public partial class Player
{
	public bool HasBullet
    {
        get => GameManager.Current.BulletOwner == this;
    }

	public bool TakeBullet()
	{
		if ( HasBullet )
		{
			GameManager.Current.RerollBullet();
			return true;
		}

		return false;
	}
}
