using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour 
{ 
    // adding public here helped...
	static SoundManager Instance = null;

    public AudioMixer masterMixer;
    public AudioMixerGroup sfxGroup;


    public enum SoundType
	{
		GenericSuccess,
		GoldBar,
		Gem,
		Null,
	};

	[Range(0,1)]
	public float soundVolume = 1.0f;
	public ChordComponent chordComponent;
    public FootstepComponent footstepComponent;
	public ChordComponent goldComponent;
	public ChordComponent gemComponent;

	void Awake()
	{
        Debug.Assert(Instance == null);
		Instance = this;
	}

	void PlayRandomSound(SoundType type)
	{
		switch (type)
		{
		case SoundType.GoldBar:
			goldComponent.PlayRandom();
			break;
		case SoundType.Gem:
			gemComponent.PlayRandom();
			break;
		case SoundType.GenericSuccess:
		default:
			// hand this off to our chord component
			chordComponent.PlayRandom();
			break;
		case SoundType.Null:
			//don't play anything on collect
			break;
		
		}
	}

	public static void PlaySoundType(SoundType st)
	{
		if (Instance != null)
			Instance.PlayRandomSound(st);
	}

    static public void ChangeState(FootstepComponent.GroundMaterial idx)
    {
        Instance.footstepComponent.changeState(idx);
    }

    static public void PlayStep(int idx)
    {
        Instance.footstepComponent.playStep(idx);
    }
}