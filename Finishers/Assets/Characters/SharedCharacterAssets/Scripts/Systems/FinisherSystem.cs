using UnityEngine;

using Finisher.Cameras;

namespace Finisher.Characters {
    public class FinisherSystem : MonoBehaviour
    {
        private Animator animator;
        private PlayerCharacterController playerChar;
        private FreeLookCam freeLookCam;
        private Transform grabTarget;

        public bool FinisherModeActive { get { return animator.GetBool(AnimContstants.Parameters.FINISHERMODE_BOOL); } }

        public delegate void StartGrabbingTarget();
        public event StartGrabbingTarget OnStartGrabbingTarget;
        public delegate void StopGrabbingTarget();
        public event StopGrabbingTarget OnStopGrabbingTarget;

        void Start()
        {
            animator = GetComponent<Animator>();
            playerChar = GetComponent<PlayerCharacterController>();
            freeLookCam = FindObjectOfType<FreeLookCam>();
            OnStartGrabbingTarget += startGrab;
            OnStopGrabbingTarget += stopGrab;
        }

        void Update()
        {
            if(playerChar.Dying)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                animator.SetBool(AnimContstants.Parameters.FINISHERMODE_BOOL, !FinisherModeActive);
            }
            if (FinisherModeActive)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    if (grabTarget)
                    {
                        OnStopGrabbingTarget();
                    }
                    else
                    {
                        if (playerChar.CombatTarget != null)
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
            grabTarget = playerChar.CombatTarget;
            freeLookCam.NewFollowTarget = grabTarget;
            playerChar.Grabbing = true;
            grabTarget.GetComponent<CharacterMotor>().Staggered = true;
        }

        private void stopGrab()
        {
            grabTarget.GetComponent<CharacterMotor>().Staggered = false;
            grabTarget = null;
            freeLookCam.NewFollowTarget = null;
            playerChar.Grabbing = false;
        }
    }
}
