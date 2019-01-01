using UnityEngine;

using Finisher.Cameras;

namespace Finisher.Characters {
    public class FinisherSystem : MonoBehaviour
    {
        private Animator animator;
        private PlayerCharacterController character;
        private FreeLookCam freeLookCam;
        private Transform grabTarget;

        public delegate void StartGrabbingTarget();
        public event StartGrabbingTarget OnStartGrabbingTarget;
        public delegate void StopGrabbingTarget();
        public event StopGrabbingTarget OnStopGrabbingTarget;

        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<PlayerCharacterController>();
            freeLookCam = FindObjectOfType<FreeLookCam>();
            OnStartGrabbingTarget += startGrab;
            OnStopGrabbingTarget += stopGrab;
        }

        void Update()
        {
            if(character.Dying)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                animator.SetBool(AnimContstants.Parameters.FINISHERMODE_BOOL, !character.FinisherModeActive);
            }
            if (character.FinisherModeActive)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    if (grabTarget)
                    {
                        OnStopGrabbingTarget();
                    }
                    else
                    {
                        if (character.CombatTarget != null)
                        {
                            OnStartGrabbingTarget();
                        }
                    }
                }
            }

            if (grabTarget)
            {
                if (grabTarget.GetComponent<CharacterMotor>().Dying)
                {
                    OnStopGrabbingTarget();
                }
                else
                {
                    transform.position = grabTarget.position + grabTarget.forward;

                    Vector3 rot = freeLookCam.transform.rotation.eulerAngles;
                    rot = new Vector3(rot.x, rot.y + 180, rot.z);
                    grabTarget.rotation = Quaternion.Euler(rot);
                }
            }
        }

        private void startGrab()
        {
            grabTarget = character.CombatTarget;
            freeLookCam.NewFollowTarget = grabTarget;
            character.Grabbing = true;
            grabTarget.GetComponent<CharacterMotor>().Staggered = true;
        }

        private void stopGrab()
        {
            grabTarget.GetComponent<CharacterMotor>().Staggered = false;
            grabTarget = null;
            freeLookCam.NewFollowTarget = null;
            character.Grabbing = false;
        }
    }
}
