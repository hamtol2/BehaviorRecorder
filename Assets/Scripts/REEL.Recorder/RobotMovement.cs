using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
	public class RobotMovement : MonoBehaviour
	{
        public enum State
        {
            Idle, Move
        }

        [SerializeField] private State state = State.Idle;

        [SerializeField] private Transform waypoint;
        [SerializeField] private Transform[] waypoints;

        [SerializeField] private float waitTimeMin = 1.5f;
        [SerializeField] private float waitTimeMax = 5f;

        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float turnSpeed = 360f;

        [SerializeField] private Vector3 targetPos;
        [SerializeField] private bool isSimulation = true;

        private float timerTarget;
        private float elapsedTime;

        private float WaitTime
        {
            get { return Random.Range(waitTimeMin, waitTimeMax); }
        }

        private void Awake()
        {
            waypoints = waypoint.GetComponentsInChildren<Transform>();
            ResetTimer();
        }

        private void Update()
        {
            if (!isSimulation) return;

            FSM();
        }

        private void FSM()
        {
            switch (state)
            {
                case State.Idle: Idle(); break;
                case State.Move: Move(); break;
                default: break;
            }
        }

        private void Idle()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > timerTarget)
            {
                ResetTimer();
                targetPos = waypoints[Random.Range(0, waypoints.Length)].position;

                SetState(State.Move);
            }
        }

        void SetState(State newState)
        {
            this.state = newState;
        }

        private void Move()
        {
            //RotateToward(targetPos);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) == 0f)
            {
                SetState(State.Idle);
            }
        }

        public void MoveToOrigin()
        {
            targetPos = Vector3.zero;
            SetState(State.Move);
            //SetSimulation(true);
            StartCoroutine("MoveToOriginCoroutine");
        }

        private IEnumerator MoveToOriginCoroutine()
        {
            while (true)
            {
                if (Vector3.Distance(transform.position, targetPos) == 0f)
                    break;

                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        public void SetSimulation(bool isSimulation)
        {
            this.isSimulation = isSimulation;
        }

        private void RotateToward(Vector3 targetPos)
        {
            Vector3 relativePosition = targetPos - transform.position;
            relativePosition.y = 0f;

            if (relativePosition == Vector3.zero) return;

            Quaternion targetRot = Quaternion.LookRotation(relativePosition);
            Quaternion frameRot = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            transform.rotation = frameRot;
        }

        private void ResetTimer()
        {
            elapsedTime = 0;
            timerTarget = WaitTime;
        }
    }
}