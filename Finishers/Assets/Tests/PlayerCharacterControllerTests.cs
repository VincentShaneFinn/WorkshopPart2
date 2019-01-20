using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Environments;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = System.Diagnostics.Debug;

namespace Tests
{
    [SetUpFixture]
    public class Master
    {
        public static GameObject Player;
        public static GameObject Enemy;
        
        [OneTimeSetUp]
        public void MasterSetup()
        {
            // Make Player
            Player = Object.Instantiate(
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Characters/Player/Player.prefab"
                )
            );
            // Make Enemy
            Enemy = Object.Instantiate(
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Characters/Enemies/Enemy.prefab"
                    )
                );
        }

        [OneTimeTearDown]
        public void MasterTeardown()
        {
            // Destroy Player
            Object.Destroy(
                    Player 
                );
            // Destroy Enemy
            Object.Destroy(
                    Enemy 
                );        
        }
    }

    public class PlayerCharacterControllerTests
    {
       // A Test behaves as an ordinary method
        [Test]
        public void CharacterPositionalSnapping()
        {
            /*
             * TESTED ATTRIBUTE:
             * - Player Rigidbody Location
             */

            var playerlocation = Master.Player.GetComponent<Rigidbody>().position;
            var enemylocation = Master.Enemy.GetComponent<Rigidbody>().position;
            
            /*
             * PRE CONDITIONS:
             * - Enemy targeted (in hitbox)
             * - Player attacks
             */

            Master.Player.GetComponent<>();
            
            /*
             * POST CONDITIONS:
             * - Player Rigidbody 1 unit away from Target Rigidbody
             */
            Assert.True(true);
        }
    }
}
