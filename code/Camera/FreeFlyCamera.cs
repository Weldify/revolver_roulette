namespace RevolverRoulette;

public interface ICameraMode
{
	public void Activated();
	public void Update();
}

internal class FreeFlyCamera : ICameraMode
{
	const float DefaultSpeed = 400f;
	const float FastSpeed = 700f;

	public void Activated()
	{
		var plr = Game.LocalPawn as Player;

		var pos = plr.EyePosition;
		var rot = plr.ViewAngles.ToRotation();

		if ( !plr.FirstTimeSpectator ) return;
		plr.FirstTimeSpectator = false;

		Camera.Position = pos;
		Camera.Rotation = rot;
		Camera.FirstPersonViewer = plr;
	}

	public void Update()
	{
		var plr = Game.LocalPawn as Player;

		var rot = plr.ViewAngles.ToRotation();
		
		var up = Convert.ToSingle( Input.Down( InputButton.Jump ) ) - Convert.ToSingle( Input.Down( InputButton.Duck ) );

		var dir = new Vector3( plr.InputDirection.x.Clamp( -1f, 1f ), plr.InputDirection.y.Clamp( -1f, 1f ), up ).Normal;
		dir *= rot;

		var speed = Input.Down( InputButton.Run ) ? FastSpeed : DefaultSpeed;

		Camera.Position += dir.Normal * speed * Time.Delta;
		Camera.Rotation = rot;
	}
}
