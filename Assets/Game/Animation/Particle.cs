using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Particle : MonoBehaviour {

    /* --- Data Structures --- */
    [System.Serializable]
    public struct ParticleData {
        [SerializeField, ReadOnly] public Sprite[] animation;
        [SerializeField, ReadOnly] public int length;
        [SerializeField, ReadOnly] public float timeInterval;

        public ParticleData(Sprite[] animation, int length) {
            this.animation = animation;
            this.length = animation.Length;
            this.timeInterval = 0f;
        }
    }

    /* --- Components --- */
    private SpriteRenderer spriteRenderer;

    /* --- Parameters --- */
    [SerializeField] private Sprite[] particle;
    [SerializeField] private bool loop;

    /* --- Properties --- */
    [SerializeField] private ParticleData particleData;

    /* --- Unity --- */
    // Runs once before the first frame.
    private void Start() {
        Init();
    }

    // Runs once every frame.
    private void Update() {
        float deltaTime = Time.deltaTime;
        Animate(deltaTime);
    }

    /* --- Methods --- */
    private void Init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particleData = new ParticleData(particle, particle.Length);
    }

    private void Animate(float deltaTime) {
        // Set the current frame.
        particleData.timeInterval += deltaTime;
        int index = (int)Mathf.Floor(particleData.timeInterval * GameRules.FrameRate);
        if (!loop && index >= particleData.length) {
            Destroy(gameObject);
        }
        index = index % particleData.length;
        spriteRenderer.sprite = particleData.animation[index];
    }

}
