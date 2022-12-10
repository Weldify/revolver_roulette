namespace RevolverRoulette;

public partial class Player
{
	[Net]
	public bool IsSpectating { get; set; } = true;
	[Net]
	public Vector3 SpectatorOrigin { get; set; }
	public bool FirstTimeSpectator = true;

	void ResolveCamera()
	{
		if ( IsSpectating && CameraMode is not FreeFlyCamera )
		{
			CameraMode = new FreeFlyCamera();
			return;
		}

		if ( !IsSpectating && CameraMode is not FirstPersonCamera )
		{
			CameraMode = new FirstPersonCamera();
		}
	}
}
