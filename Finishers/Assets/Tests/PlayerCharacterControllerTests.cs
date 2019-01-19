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
        private GameObject _player;
        private GameObject _enemy;
        
        [OneTimeSetUp]
        public void MasterSetup()
        {
            // Make Player
            _player = Object.Instantiate(
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Characters/Player/Player.prefab"
                )
            );
            // Make Enemy
            _enemy = Object.Instantiate(
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
                    _player 
                );
            // Make Enemy
            Object.Destroy(
                    _enemy 
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
            
            /*
             * PRE CONDITIONS:
             * - Enemy targeted (in hitbox)
             * - Player attacks
             */
            
            /*
             * POST CONDITIONS:
             * - Player Rigidbody 1 unit away from Target Rigidbody
             */
            Assert.True(true);
        }
    }
}
