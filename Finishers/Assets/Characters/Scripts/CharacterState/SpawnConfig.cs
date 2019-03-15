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
        protected Dictionary<string, Vector3> savedPositions = new Dictionary<string, Vector3>();
        protected Dictionary<string, Quaternion> savedRotations = new Dictionary<string, Quaternion>();
        protected Dictionary<string, bool> savedDestroyed = new Dictionary<string, bool>();
        protected Dictionary<string, bool> savedInteractions = new Dictionary<string, bool>();
        public SpawnConfig()
        {
            this.scene = SceneManager.GetActiveScene();
            GameObject[] objects = scene.GetRootGameObjects();
            foreach (GameObject obj in objects)
            {
                Saveable[] saveables = obj.GetComponentsInChildren<Saveable>();
                foreach (Saveable s in saveables)
                {
                    GameObject go = s.gameObject;
                    string fullName = GetFullName(go);
                    if (s is PositionSaveable)
                    {
                        savedPositions.Add(fullName, go.transform.position);
                        savedRotations.Add(fullName, go.transform.rotation);
                    } else if (s is DestroyedSaveable)
                    {
                        savedDestroyed.Add(fullName, false);
                    }
                    else if (s is InteractionSaveable)
                    {
                        savedInteractions.Add(fullName, ((InteractionSaveable) s).interacted);
                    }
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

            foreach (GameObject obj in objects)
            {
                Saveable[] saveables = obj.GetComponentsInChildren<Saveable>();
                foreach (Saveable s in saveables)
                {
                    GameObject go = s.gameObject;
                    string fullName = GetFullName(go);
                    if (s is PositionSaveable)
                    {
                        go.transform.position = savedPositions[fullName];
                        go.transform.rotation = savedRotations[fullName];
                    }
                    else if (s is DestroyedSaveable)
                    {
                        if (!savedDestroyed.ContainsKey(fullName))
                        {
                            GameObject.Destroy(go);
                        }
                    }
                    else if (s is InteractionSaveable)
                    {
                        if (savedInteractions[fullName])
                        {
                            ((InteractionSaveable) s).runInteraction();
                        }
                    }
                }
            }
            Time.timeScale = 1;
            SceneManager.sceneLoaded -= setStates;
        }

        public string GetFullName(GameObject go)
        {
            string name = go.name;
            while (go.transform.parent != null)
            {

                go = go.transform.parent.gameObject;
                name = go.name + "/" + name;
            }
            return name;
        }
    }
}