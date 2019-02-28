using Finisher.Characters.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Finisher.Characters
{
    public class SpawnConfig
    {
        protected Vector3 spawnPosition;
        protected Quaternion spawnRotation;
        protected Scene scene;

        public SpawnConfig(Transform spawnTransform)
        {
            this.spawnPosition = spawnTransform.position;
            this.spawnRotation = spawnTransform.rotation;
            this.scene = SceneManager.GetActiveScene();
        }

        public void runConfig()
        {
            SceneManager.sceneLoaded += setStates;
            SceneManager.LoadScene(scene.buildIndex);
        }

        private void setStates(Scene scene, LoadSceneMode mode)
        {
            GameObject[] objects = scene.GetRootGameObjects();
            GameObject player = null;
            foreach (GameObject obj in objects)
            {
                PlayerCharacterController controller = obj.GetComponentInChildren<PlayerCharacterController>();
                if (controller != null)
                {
                    player = controller.gameObject;
                }
            }
            Assert.IsNotNull(player);

            player.transform.position = spawnPosition;
            player.transform.rotation = spawnRotation;
            SceneManager.sceneLoaded -= setStates;
        }
    }
}