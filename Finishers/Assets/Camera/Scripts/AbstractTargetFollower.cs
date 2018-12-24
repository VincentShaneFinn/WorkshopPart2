using System;
using UnityEngine;

namespace Finisher.Cameras
{
    [DisallowMultipleComponent]
    public abstract class AbstractTargetFollower : MonoBehaviour
    {
        public enum UpdateType // The available methods of updating are:
        {
            FixedUpdate, // Update in FixedUpdate (for tracking rigidbodies).
            LateUpdate, // Update in LateUpdate. (for tracking objects that are moved in Update)
            ManualUpdate, // user must call to update camera
        }
        [Tooltip("Will target Player at start")]
        [SerializeField] protected Transform followTarget;            // The target object to follow
        [SerializeField] private bool autoTargetPlayer = true;  // Whether the rig should automatically target the player.
        [SerializeField] private UpdateType updateType;         // stores the selected update type

        protected Rigidbody targetRigidbody;


        protected virtual void Start()
        {
            // if auto targeting is used, find the object tagged "Player"
            // any class inheriting from this should call base.Start() to perform this action!
            if (autoTargetPlayer)
            {
                FindAndTargetPlayer();
            }
            if (followTarget == null) return;
            targetRigidbody = followTarget.GetComponent<Rigidbody>();
        }


        private void FixedUpdate()
        {
            // we update from here if updatetype is set to Fixed, or in auto mode,
            // if the target has a rigidbody, and isn't kinematic.
            if (autoTargetPlayer && (followTarget == null || !followTarget.gameObject.activeSelf))
            {
                FindAndTargetPlayer();
            }
            if (updateType == UpdateType.FixedUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }


        private void LateUpdate()
        {
            // we update from here if updatetype is set to Late, or in auto mode,
            // if the target does not have a rigidbody, or - does have a rigidbody but is set to kinematic.
            if (autoTargetPlayer && (followTarget == null || !followTarget.gameObject.activeSelf))
            {
                FindAndTargetPlayer();
            }
            if (updateType == UpdateType.LateUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }


        public void ManualUpdate()
        {
            // we update from here if updatetype is set to Late, or in auto mode,
            // if the target does not have a rigidbody, or - does have a rigidbody but is set to kinematic.
            if (autoTargetPlayer && (followTarget == null || !followTarget.gameObject.activeSelf))
            {
                FindAndTargetPlayer();
            }
            if (updateType == UpdateType.ManualUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        protected abstract void FollowTarget(float deltaTime);

        public void FindAndTargetPlayer()
        {
            // auto target an object tagged player, if no target has been assigned
            var targetObj = GameObject.FindGameObjectWithTag("Player");
            if (targetObj)
            {
                SetFollowTarget(targetObj.transform);
            }
        }

        public virtual void SetFollowTarget(Transform newTransform)
        {
            followTarget = newTransform;
        }

        public Transform Target
        {
            get { return followTarget; }
        }
    }
}
