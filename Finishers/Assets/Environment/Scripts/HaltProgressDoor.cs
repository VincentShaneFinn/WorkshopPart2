using Finisher.Characters.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Environment {
    public class HaltProgressDoor : MonoBehaviour
    {

        [SerializeField] List<SquadManager> squadManagers;

        // Start is called before the first frame update
        void Start()
        {
            foreach (var squadManager in squadManagers)
            {
                squadManager.door = this;
            }
        }

        public void RemoveSquad(SquadManager squadManager)
        {
            squadManagers.Remove(squadManager);
            if(squadManagers.Count <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
