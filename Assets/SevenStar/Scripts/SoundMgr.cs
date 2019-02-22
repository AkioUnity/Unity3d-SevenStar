using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtil;


public enum SoundFXType
{
    RoomIn=0,
    RoomOut,
    GameStart,
    GameEnd,
    ChargeChip,
    WinChip,
    MyTurn,
    PlayerTurn,
    Check,
    Fold,
    CardDrop,
    DealCard,
    CardOpen,
    Capture,
    Bankrupt,
    ButtonClick,
    PlayerCardOpen,
    WinnerMyself,
    Voice_M_Call,
    Voice_F_Call,
    Voice_M_Bet,
    Voice_F_Bet,
    Voice_M_Fold,
    Voice_F_Fold,
    Voice_M_Check,
    Voice_F_Check,
    BettingChip
}

public enum VoiceFXType
{
    Call,
    Bet,
    Fold,
    Check
}

[RequireComponent(typeof(AudioSource))]
public class SoundMgr : UtilHalfSingleton<SoundMgr>
{
    public AudioSource m_Audio;
    public AudioSource m_AlertAudio;
    public AudioClip m_AlertBeep;
    public AudioClip[] m_Clips;
    public AudioClip m_BettingChip1;
    public AudioClip m_BettingChip2;
    public AudioClip m_BettingChip3;
    private bool m_FXDelay = false;

    public bool m_IsAlert = false;

    private void Start()
    {
        if (m_Audio == null)
            m_Audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (m_IsAlert)
        {
            if (m_AlertAudio.isPlaying == false)
                m_AlertAudio.Play();
        }
        else
        {
            if (m_AlertAudio.isPlaying)
                m_AlertAudio.Stop();
        }
    }

    public void PlaySoundFx(SoundFXType type)
    {
        AudioClip useClip = GetClip(type);
        if (useClip != null)
            m_Audio.PlayOneShot(useClip);
    }

    public void PlaySoundFx(SoundFXType type, float delay)
    {
        AudioClip useClip = GetClip(type);
        if (useClip != null && m_FXDelay == false)
            StartCoroutine(PlaySoundFXDelay(useClip, delay));
    }

    public void PlayVoiceSound(VoiceFXType type, int gender)
    {
        if (gender < 0)
            return;
        SoundFXType soundType = SoundFXType.BettingChip;
        switch (type)
        {
            case VoiceFXType.Call:
                if (gender == 0)
                    soundType = SoundFXType.Voice_M_Call;
                else
                    soundType = SoundFXType.Voice_F_Call;
                break;
            case VoiceFXType.Bet:
                if (gender == 0)
                    soundType = SoundFXType.Voice_M_Bet;
                else
                    soundType = SoundFXType.Voice_F_Bet;
                break;
            case VoiceFXType.Fold:
                if (gender == 0)
                    soundType = SoundFXType.Voice_M_Fold;
                else
                    soundType = SoundFXType.Voice_F_Fold;
                break;
            case VoiceFXType.Check:
                if (gender == 0)
                    soundType = SoundFXType.Voice_M_Check;
                else
                    soundType = SoundFXType.Voice_F_Check;
                break;
        }

        AudioClip useClip = GetClip(soundType);
        if (useClip != null)
            m_Audio.PlayOneShot(useClip);
    }

    private IEnumerator PlaySoundFXDelay(AudioClip clip, float delay)
    {
        m_FXDelay = true;
        m_Audio.PlayOneShot(clip);
        yield return new WaitForSeconds(delay);
        m_FXDelay = false;
    }

    private AudioClip GetClip(SoundFXType type)
    {
        AudioClip useClip = null;
        if (type == SoundFXType.BettingChip)
        {
            int randChipSound = Random.Range(0, 3);
            switch (randChipSound)
            {
                case 0:
                    useClip = m_BettingChip1;
                    break;
                case 1:
                    useClip = m_BettingChip2;
                    break;
                case 2:
                    useClip = m_BettingChip3;
                    break;
            }
        }
        else
        {
            useClip = m_Clips[(int)type];
        }
        return useClip;
    }


}
