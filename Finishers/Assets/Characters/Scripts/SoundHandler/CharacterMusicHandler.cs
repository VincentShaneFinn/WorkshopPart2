using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Finisher.Characters.Systems;

namespace Finisher.Core
{
    public class CharacterMusicHandler : MonoBehaviour
    {
        protected EnemyManager enemyManager;
        private List<AudioClip> combatMusic;
        private List<AudioClip> explorationMusic;
        private AudioSource musicSource;
        private float timer = 0.0f;
        private int volumeSteps = 10;
        private float timeBetweenStep = .1f;
        private float maxVolume = 1;

        //to be removed once a proper config is set up for this
        public AudioClip Adventurer;
        public AudioClip AdventurerAlt;
        public AudioClip Mystic;
        public AudioClip Ominous;
        public AudioClip Fight;

        void Awake()
        {
            combatMusic = new List<AudioClip> { Fight };
            explorationMusic = new List<AudioClip> { Adventurer, AdventurerAlt, Mystic, Ominous };

            enemyManager = FindObjectOfType<EnemyManager>();
            musicSource = gameObject.AddComponent<AudioSource>();

            musicSource.clip = explorationMusic[Random.Range(0, explorationMusic.Count - 1)];
            musicSource.Play(0);
            subscribeToDelegates();
        }

        private void subscribeToDelegates()
        {
            enemyManager.OnPlayerInCombatChanged += ToggleCombatMusic;
        }

        void ToggleCombatMusic(bool inCombat)
        {
            StartCoroutine(switchTrack(inCombat));
        }

        //fades out of current track, switches track, then fades back in
        private IEnumerator switchTrack(bool inCombatPass)
        {
            //fade out
            for(int i=0; i<= volumeSteps; i++ )
            {
                musicSource.volume -= maxVolume / volumeSteps;
                yield return new WaitForSeconds(timeBetweenStep);
            }
            //switch tracks on context
            if (inCombatPass)
            {
                musicSource.clip = combatMusic[Random.Range(0, combatMusic.Count-1)];
            }
            else
            {
                musicSource.clip = explorationMusic[Random.Range(0, explorationMusic.Count - 1)];
            }
            //start new track
            musicSource.Play(0);
            //fade volume back up
            for (int i = 0; i <= volumeSteps; i++)
            {
                musicSource.volume += maxVolume / volumeSteps;
                yield return new WaitForSeconds(timeBetweenStep);
            }
        }
    }
}