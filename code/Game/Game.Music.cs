namespace RevolverRoulette;

public partial class Game
{
	static float musicVolume = 0.1f;

	[ConVar.Client( "music_volume" )]
	public static float MusicVolume
	{
		get => musicVolume;
		set
		{
			musicVolume = Math.Clamp( value, 0f, 1f );
		}
	}

	readonly List<string> musicList = new()
	{
		"music.digital_gunslinger",
		"music.rattlesnake_railroad",
		"music.three_kinds_of_suns",
	};

	Sound music;
	string lastMusicName;

	void SwitchMusic()
	{
		var names = musicList.Where( s => s != lastMusicName ).ToList();

		Rand.SetSeed( Time.Tick );
		var name = Rand.FromList( names );

		music = Sound.FromScreen( name );
		lastMusicName = name;

		musicFinished = false;
	}

	bool musicFinished = true;
	void TickMusic()
	{
		if ( musicFinished )
			SwitchMusic();

		music.SetVolume( MusicVolume );

		if ( music.Finished && music.ElapsedTime > 1f )
			musicFinished = true;
	}
}
