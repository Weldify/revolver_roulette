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

	Sound music;
	string curMusicName;

	readonly List<string> musicList = new()
	{
		"music.digital_gunslinger",
		"music.rattlesnake_railroad",
		"music.three_kinds_of_suns",
	};

	void RandomizeMusic( string without )
	{
		var names = musicList.Where( s => s != without ).ToList();

		Rand.SetSeed( Time.Tick );
		var name = Rand.FromList( names );

		music = Sound.FromScreen( name );
		curMusicName = name;
	}

	void TickMusic()
	{
		var uninitialized = music.Equals( default( Sound ));
		if ( uninitialized || music.Finished )
			RandomizeMusic( curMusicName );

		music.SetVolume( MusicVolume );
	}
}