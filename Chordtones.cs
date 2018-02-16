using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ChordComponent : MonoBehaviour 
{
    

    public enum Chord
	{
		C_Major,
		//C_Minor,
		C_Aug,
		//C_Dim,
		//B_Major,
		//B_Minor,
		//B_Aug,
		//B_Dim,
		Bb_ASharp_Major,
		//Bb_ASharp_Minor,
		//Bb_ASharp_Aug,
		//Bb_ASharp_Dim,
		A_Major,
		A_Minor,
		//A_Aug,
		//A_Dim,
		Ab_GSharp_Major,
		//Ab_GSharp_Minor,
		//Ab_GSharp_Aug,
		//Ab_GSharp_Dim,
		G_Major,
		//G_Minor,
		//G_Aug,
		//G_Dim,
		//Gb_FSharp_Major,
		//Gb_FSharp_Minor,
		//Gb_FSharp_Aug,
		//Gb_FSharp_Dim,
		F_Major,
		F_Minor,
		//F_Aug,
		//F_Dim,
		E_Major,
		//E_Minor,
		//E_Aug,
		//E_Dim,
		Eb_DSharp_Major,
		//Eb_DSharp_Minor,
		//Eb_DSharp_Aug,
		//Eb_DSharp_Dim,
		D_Major,
		D_Minor,
		//D_Aug,
		//D_Dim,
		//Db_CSharp_Major,
		//Db_CSharp_Minor,
		//Db_CSharp_Aug,
		//Db_CSharp_Dim,

		_MAX,		
	};


    [System.Serializable]
    public class ChordTones
    {
        public AudioMixerGroup ctGroup;

        //public AudioMixerGroup sfxGroup;
        //List<AudioMixerGroup> groupList = new List<AudioMixerGroup>();


        public Chord chord;
        public List<AudioClip> clips;

        float lastYPos = 32; //player height constant at ground level
        float lastXPos = 0;
        float ydiff = 0;
        float xdiff = 0;
        float diff = 0;
        int lastIdx = 0;
        //float pitchshift = 1;
        //float vol = 1;

        List<AudioSource> sources = new List<AudioSource>();


        public void Init(GameObject parent)
        {

            foreach (AudioClip ac in clips)
            {
                AudioSource auso = parent.AddComponent<AudioSource>();
                auso.playOnAwake = false;
                auso.clip = ac;
                //auso.outputAudioMixerGroup = sfxGroup;
                auso.outputAudioMixerGroup = ctGroup;
                auso.bypassListenerEffects = true;

                sources.Add(auso);
            }
        }

        int getHeightIndx()
        {
            const float HEIGHT_THRESH = .2f;

            PlayerManager mainPlayer = GameManager.GetLeadPlayer();
            float yPos = mainPlayer.transform.position.y;
            float xPos = mainPlayer.transform.position.x;
            ydiff = yPos - lastYPos;
            xdiff = xPos - lastXPos;

            ydiff = normalizeY(ydiff);
            xdiff = checkX(xdiff);

            diff = ydiff / xdiff;

            //print("lastIdx: " + lastIdx + " mainPlayer height: " + yPos + " - Last height: " + lastYPos + " - x = " + xPos + " - and last x was: " + lastXPos + " - Diff: " + diff + " -YDiff: " + ydiff + " - XDiff: " + xdiff); //debug
            lastYPos = yPos;
            lastXPos = xPos;

            int currentIdx = getNextIdx(diff, HEIGHT_THRESH, lastIdx); // test and may reorganize 
            if (currentIdx == lastIdx) {
                currentIdx = Random.Range(0, sources.Count - 1); // reshuffle 
            }
            return lastIdx = currentIdx;
        }

        float normalizeY(float ydiff)
        {

            if (ydiff > 0) // all we care about ydiff is its sign
            {
                return 1;
            }
            else if (ydiff < 0)
            {
                return -1;
            }
            else          // ydiff == 0
            {
                return 0;
            }
        }
        float checkX(float xdiff)
        {
            if (xdiff == 0) // check to avoid divide by 0
            {
                return 1;
            }
            else if (xdiff < 0) // reset!  
            {
                return 0;
            }
            return xdiff;
        }

        int getNextIdx(float diff, float HEIGHT_THRESH, int lastIdx)
        {
            int sourcesMaxIdx = sources.Count - 1;
            int sourcesHalfIdx = sourcesMaxIdx / 2;
            // careful -- random.range() is INCLUSIVE and sources.Count is not playable.

            if (diff > HEIGHT_THRESH)   // ascend 
            {

                lastIdx += 1; // might throw error if less than 0;
                //pitchshift = 1;
                //vol = 1;
                //print("Ascend!"); //debug
                if (lastIdx >= sources.Count)
                {
                    //print("Hitting max in ascend!"); //debug
                    //pitchshift = 2;
                    //vol = .75f;
                    return lastIdx = Random.Range(sourcesHalfIdx, sourcesMaxIdx);


                }
                return lastIdx = Random.Range(lastIdx, sourcesMaxIdx);

            }
            else if (diff < HEIGHT_THRESH && diff > 0) // raise but not sure if sequence 
            {
                //print("Potential lift off?"); //debug
                //pitchshift = 1;
                //vol = 1;
                lastIdx = Random.Range(0, 1);
                return lastIdx;

            }
            else if (diff < -HEIGHT_THRESH)
            {
                lastIdx -= 1; // might throw error if less than 0;
                //pitchshift = 1;
                //vol = 1;
                //print("Descend!"); // debug
                if (lastIdx < 0)
                {
                    //print("Hitting max in descend!"); // debug
                    //pitchshift = .5f;
                    //vol = 1;
                    lastIdx = Random.Range(0, sourcesHalfIdx);
                }
                return lastIdx = Random.Range(0, lastIdx);

            }
            else if (diff > -HEIGHT_THRESH && diff < 0)
            {
                //print("Potential drop down?"); //debug
                //pitchshift = 1;
                //vol = 1;
                lastIdx = Random.Range(sourcesHalfIdx, sourcesMaxIdx);
                return lastIdx;
            }
            else if (diff == 0)
            {
                //print("Truly random"); // debug
                //pitchshift = 1;
                //vol = 1;
                return Random.Range(0, sourcesMaxIdx);
            }
            //pitchshift = 1;
            //vol = 1;
            return 0; // should not get here ever...
        }


        public void PlayRandom()
        {
            
            int currentIdx = getHeightIndx();

            //print("Played index is = " + currentIdx); //debug
            //sources[currentIdx].pitch = pitchshift;
            //sources[currentIdx].volume = vol;
            sources[currentIdx].Play();

        }

        public void Play(int idx) // test
        {
            sources[idx].Play();
        }

        
    };

	public List<ChordTones> tones;

	List<ChordTones> fullChordArray = new List<ChordTones>((int)Chord._MAX);

	void Awake()
	{
		for (int i = 0; i < fullChordArray.Capacity; i++)
			fullChordArray.Add(null);

		foreach (ChordTones ct in tones)
		{
			ct.Init(this.gameObject);

			// now, store this one in an easy-reference array
			fullChordArray[(int)ct.chord] = ct;
		}
	}

	public void PlayRandom()
	{
        //lowerMusicVol(); 
        //turnOnReverb();
		Chord c = MetronomeManager.GetCurrentChord();
		ChordTones ct = fullChordArray[(int)c];
		if (ct != null)
			ct.PlayRandom();
	}

    public void PlayIdx(int idx) // test
    {    
        Chord c = MetronomeManager.GetCurrentChord();
        ChordTones ct = fullChordArray[(int)c];
        if (ct != null) {
            ct.Play(idx);
        }
    }
    

    public void lowerMusicVol() // test
    {
        AudioMixerController.SetMusicLevel (-80f);
    }

    public void turnOnReverb() // test
    {
        AudioMixerController.SetReverbLevel(0);
    }
   
}
