namespace RevolverRoulette.UI;

public partial class MagIndicator
{
    public static MagIndicator Current;
    public Panel Mag { get; set; }

    float rotation = 0f;
    float impulse = 0f;

    public MagIndicator()
    {
        Current = this;
    }

    public void Spin()
    {
        impulse = 1;
        PlaySound("revolver.cock");
    }

	public override void Tick()
	{
        rotation = (rotation + Time.Delta * impulse * 1000f) % 180f;
		Mag.Style.Set("transform", $"rotate({rotation})");

        impulse = MathF.Max(impulse - Time.Delta, 0f);
	}
}