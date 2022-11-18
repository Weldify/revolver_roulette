namespace RevolverRoulette;

internal partial class Player
{
	private readonly ClothingContainer clothing = new();

	public void UpdateClothes()
	{
		clothing.LoadFromClient( Client );
	}

	public void Dress()
	{
		clothing.DressEntity( this );
	}
}