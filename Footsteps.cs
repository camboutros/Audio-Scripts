using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FootstepComponent : MonoBehaviour
{
   
    public enum GroundMaterial
    {
        Dirt,
        Glass,
        Grass,
        Metal,
        Sand,
        Wood,

        _MAX,
    };
    public GroundMaterial currentGround;

    [System.Serializable]
    public class FootstepSounds
    {
        public AudioMixer masterMixer;
        public AudioMixerGroup sfxGroup;

        public GroundMaterial groundMaterial;
        public List<AudioClip> clips;

        public List<AudioSource> sources = new List<AudioSource>();

        public void Init(GameObject parent)
        {
            int count = 0; //test
            foreach (AudioClip ac in clips)
            {
                AudioSource auso = parent.AddComponent<AudioSource>();
                auso.playOnAwake = false;
                auso.clip = ac;
                auso.outputAudioMixerGroup = sfxGroup;

                sources.Add(auso);
                count = count + 1; //test
            }
            
        }

    };

    public List<FootstepSounds> footstepSounds;

    List<FootstepSounds> fullFootstepArray = new List<FootstepSounds>((int)GroundMaterial._MAX);

    void Awake()
    {
        for (int i = 0; i < fullFootstepArray.Capacity; i++)
            fullFootstepArray.Add(null);

        foreach (FootstepSounds ft in footstepSounds)
        {
            ft.Init(this.gameObject);

            // now, store this one in an easy-reference array
            fullFootstepArray[(int)ft.groundMaterial] = ft;
        }
        
    }

    public void changeState(GroundMaterial state)
    {
        currentGround = state;
    }


     void playStepOne()
    {
        FootstepSounds fts = fullFootstepArray[(int)currentGround];
    
        fts.sources[0].Play();
    }

    void playStepTwo()
    {
        FootstepSounds fts = fullFootstepArray[(int)currentGround];
        fts.sources[1].Play();
    }

    public void playStep(int idx)
    {
        switch (idx)
        {
            case 0:
                playStepOne();
                break;
            case 1:
                playStepTwo();
                break;
            default:
                break;
        }
    }

}
