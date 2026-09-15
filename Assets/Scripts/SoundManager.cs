using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _footstep_sound;
    [SerializeField] private AudioSource sound1;


    private void OnEnable()
    {
        MyInputManager.OnAttackPressed += PlayAudio;
        MyInputManager.OnMovePressed += CheckForMovement;
    }

    private void OnDisable()
    {
        MyInputManager.OnAttackPressed -= PlayAudio;
        MyInputManager.OnMovePressed -= CheckForMovement;
    }

    public void playShoot()
    {
        sound1.pitch = 0.5f;
        sound1.PlayOneShot(sound1.clip);
    }

    public void playHit()
    {
        // sound1.pitch = 0.5f;
        // sound1.PlayOneShot(sound1.clip);
    }

    private void PlayAudio(bool isPressed)
    {
        // if (!sound1.isPlaying)
        // {
        //     sound1.pitch = 0.5f;
        //     sound1.PlayOneShot(sound1.clip);
        //     return;
        // }
        // // sound.Stop();
        // // sound.PlayOneShot(sound.clip);
    }

    private void PlayFootStepAudio(bool isPlaying)
    {
        if (!_footstep_sound.isPlaying && isPlaying)
        {
            _footstep_sound.Play();
            return;
        }
        // sound.Stop();
        // sound.PlayOneShot(sound.clip);
        if (_footstep_sound.isPlaying && !isPlaying)
        {
            _footstep_sound.Stop();
            return;
        }

    }

    private void CheckForMovement(Vector2 input)
    {
        bool toPlay = input.sqrMagnitude >= 0.2f;
        PlayFootStepAudio(toPlay);
    }
}
