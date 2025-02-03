using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    public AudioClip[] audioList;
    AudioSource source;
    public bool is_sound_on = true;

    private int fruit_slice_index = 0;
    private int bomb_index = 1;
    private int stab_index = 2;
    private int fruit_impact_index = 3;
    private int vicotry_index = 4;
    private int game_audio_index = 5;
    private int menu_audio_index = 6;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void FruitSliceSound()
    {
        if (!is_sound_on) {
            return;
        }
        source.PlayOneShot(audioList[fruit_slice_index]);
    }

    public void FruitImpactSound()
    {
        if (!is_sound_on) {
            return;
        }
        source.PlayOneShot(audioList[fruit_impact_index]);
    }

    public void BombSound()
    {
        if (!is_sound_on) {
            return;
        }
        source.PlayOneShot(audioList[bomb_index]);
    }
    
    public void StabSound()
    {
        if (!is_sound_on) {
            return;
        }
        source.PlayOneShot(audioList[stab_index]);
    }

    public void VictorySound()
    {
        if (!is_sound_on) {
            return;
        }
        source.PlayOneShot(audioList[vicotry_index]);
    }

    public void StartGameMusic()
    {
        source.clip = audioList[game_audio_index];
        source.Play();
    }

    public void StartMenuMusic()
    {
        source.clip = audioList[menu_audio_index];
        source.Play();
    }
}