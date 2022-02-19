/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using Movement = Controller.Movement;
using Direction = Controller.Direction;
using Airborne = Controller.Airborne;
using ActionState = Controller.ActionState;

/// <summary>
/// Handles the collision framework and animation
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class NoiseMesh : MonoBehaviour {

    public Controller controller;
    public AudioSource audioSource;

    public AudioClip movementAudio;

    // Start is called before the first frame update
    void Start() {
        Init();
    }

    private void Init() {
        controller = transform.parent.GetComponent<Controller>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        GetAudio();
    }

    /* --- Sub-Methods --- */
    private void GetAudio() {

        if (controller.movementFlag != Movement.Idle) {
            AudioMovement();
        }
        else {
            AudioIdle();
        }

    }

    private void AudioMovement() {
        audioSource.clip = movementAudio;
        if (!audioSource.isPlaying) {
            audioSource.Play();
        }
    }

    private void AudioIdle() {
        audioSource.clip = null;
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
    }

}
