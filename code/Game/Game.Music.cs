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

	bool musicInitialized = false;
	void TickMusic()
	{
		// Terrorizes me when testing the game
		if ( Local.Client.IsBot ) return;

		if ( !musicInitialized || music.Finished )
			musicInitialized = true;
			RandomizeMusic( curMusicName );

		music.SetVolume( MusicVolume );
	}
}
