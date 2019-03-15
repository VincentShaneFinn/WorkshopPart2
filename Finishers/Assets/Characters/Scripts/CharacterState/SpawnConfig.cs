using Finisher.Characters.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Finisher.Characters
{
    public class SpawnConfig
    {
        protected Scene scene;
        protected Dictionary<int, Vector3> savedPositions = new Dictionary<int, Vector3>();
        protected Dictionary<int, Quaternion> savedRotations = new Dictionary<int, Quaternion>();
        protected Dictionary<int, bool> savedDestroyed = new Dictionary<int, bool>();
        protected Dictionary<int, bool> savedInteractions = new Dictionary<int, bool>();
        public SpawnConfig()
        {
            this.scene = SceneManager.GetActiveScene();
            GameObject[] objects = scene.GetRootGameObjects();
            foreach (GameObject obj in objects)
            {
                PositionSaveable[] positionSaveables = obj.GetComponentsInChildren<PositionSaveable>();
                foreach (PositionSaveable po in positionSaveables)
                {
                    GameObject go = po.gameObject;
                    savedPositions.Add(po.uniqueId, go.transform.position);
                    savedRotations.Add(po.uniqueId, go.transform.rotation);
                }
                DestroyedSaveable[] destroyedSavables = obj.GetComponentsInChildren<DestroyedSaveable>();
                foreach (DestroyedSaveable ds in destroyedSavables)
                {
                    GameObject go = ds.gameObject;
                    savedDestroyed.Add(ds.uniqueId, false);
                }
                InteractionSaveable[] interactionSaveables = obj.GetComponentsInChildren<InteractionSaveable>();
                foreach (InteractionSaveable i in interactionSaveables)
                {
                    savedInteractions.Add(i.uniqueId, i.interacted);
                }
            }
        }

        public void runConfig()
        {
            SceneManager.sceneLoaded += setStates;
            SceneManager.LoadScene(scene.buildIndex);
        }

        private void setStates(Scene scene, LoadSceneMode mode)
        {
            Time.timeScale = 0;
            GameObject[] objects = scene.GetRootGameObjects();
            Dictionary<int, GameObject> positionSaved = new Dictionary<int, GameObject>();
            Dictionary<int, GameObject> destroyedSaved = new Dictionary<int, GameObject>();
            Dictionary<int, InteractionSaveable> interactionSaved = new Dictionary<int, InteractionSaveable>();

            foreach (GameObject obj in objects)
            {
                PositionSaveable[] positionSaveables = obj.GetComponentsInChildren<PositionSaveable>();
                foreach (PositionSaveable po in positionSaveables) {
                    GameObject go = po.gameObject;
                    positionSaved.Add(po.uniqueId, go);
                    Debug.Log(go.GetInstanceID());
                }
                DestroyedSaveable[] destroyedSavables = obj.GetComponentsInChildren<DestroyedSaveable>();
                foreach (DestroyedSaveable ds in destroyedSavables)
                {
                    GameObject go = ds.gameObject;
                    destroyedSaved.Add(ds.uniqueId, go);
                }
                InteractionSaveable[] interactionSaveables = obj.GetComponentsInChildren<InteractionSaveable>();
                foreach (InteractionSaveable i in interactionSaveables)
                {
                    interactionSaved.Add(i.uniqueId, i);
                }
            }
            foreach (int key in positionSaved.Keys)
            {
                positionSaved[key].transform.position = savedPositions[key];
                positionSaved[key].transform.rotation = savedRotations[key];
            }
            foreach (int key in destroyedSaved.Keys)
            {
                if (!savedDestroyed.ContainsKey(key))
                {
                    GameObject.Destroy(destroyedSaved[key]);
                }
            }
            foreach (int key in interactionSaved.Keys)
            {
                if (savedInteractions[key])
                {
                    interactionSaved[key].runInteraction();
                }
            }
            Time.timeScale = 1;
            SceneManager.sceneLoaded -= setStates;
        }
    }
}