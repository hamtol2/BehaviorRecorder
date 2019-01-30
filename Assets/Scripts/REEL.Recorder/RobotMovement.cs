using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Recorder
{
    public class RobotMovement : MonoBehaviour
    {
        public enum State
        {
            Idle, Move, Clap, No
        }

        private enum WaypointEnum
        {
            Origin, Right, Left
        }

        [SerializeField] private State state = State.Idle;
        public State GetRobotState { get { return state; } }

        [SerializeField] private Transform waypoint;
        [SerializeField] private Transform[] waypoints;

        [SerializeField] private float waitTimeMin = 1.5f;
        [SerializeField] private float waitTimeMax = 5f;

        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float turnSpeed = 360f;

        [SerializeField] private Vector3 targetPos;
        [SerializeField] private bool isSimulation = true;

        private float timerTarget;
        private float elapsedTime;

        [SerializeField] private WaypointEnum currentPosition;

        private Timer returnToOriginTimer;
        private float returnTime = 1.5f;

        private Queue<WaypointEnum> nextWaypoint = new Queue<WaypointEnum>();

        private float WaitTime
        {
            get { return Random.Range(waitTimeMin, waitTimeMax); }
        }

        //private void Awake()
        private void Start()
        {
            waypoints = waypoint.GetComponentsInChildren<Transform>();
            ResetTimer();
        }

        private void Update()
        {
            //if (!isSimulation) return;

            if (returnToOriginTimer != null)
                returnToOriginTimer.Update(Time.deltaTime);

            FSM();
        }

        private void FSM()
        {
            switch (state)
            {
                case State.Idle: Idle(); break;
                case State.Move: Move(); break;
                case State.Clap: Clap(); break;
                case State.No: No(); break;

                default: break;
            }
        }

        float GetRandomTime(float maxValue)
        {
            float time = Random.Range(maxValue * 0.9f, maxValue * 1.2f);
            //Debug.LogWarning("Robot Return Time: " + time);
            return time;
        }

        public void MoveRight()
        {
            currentPosition = WaypointEnum.Right;
            nextWaypoint.Enqueue(WaypointEnum.Left);
            nextWaypoint.Enqueue(WaypointEnum.Origin);

            targetPos = waypoints[(int)currentPosition].position;
            SetState(State.Move);

            //float time = GetRandomTime(returnTime);
            //returnToOriginTimer = new Timer(time, ReturnToOrigin);
        }

        public void MoveLeft()
        {
            currentPosition = WaypointEnum.Left;
            nextWaypoint.Enqueue(WaypointEnum.Right);
            nextWaypoint.Enqueue(WaypointEnum.Origin);

            targetPos = waypoints[(int)currentPosition].position;
            SetState(State.Move);

            //float time = GetRandomTime(returnTime);
            //returnToOriginTimer = new Timer(time, ReturnToOrigin);
        }

        public void ReturnToOrigin()
        {
            currentPosition = WaypointEnum.Origin;
            targetPos = waypoints[(int)currentPosition].position;
            SetState(State.Move);
        }

        private void Idle()
        {
            //elapsedTime += Time.deltaTime;
            //if (elapsedTime > timerTarget)
            //{
            //    ResetTimer();
            //    targetPos = waypoints[Random.Range(0, waypoints.Length)].position;

            //    SetState(State.Move);
            //}
        }

        public void SetState(State newState)
        {
            this.state = newState;
        }

        private void Move()
        {
            RotateToward(targetPos);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) == 0f)
            {
                if (nextWaypoint.Count > 0)
                {
                    Invoke("GetNextWaypoint", GetRandomTime(returnTime));
                }

                SetState(State.Idle);
            }
        }

        private void GetNextWaypoint()
        {
            WaypointEnum nextPoint = nextWaypoint.Dequeue();
            targetPos = waypoints[(int)nextPoint].position;

            SetState(State.Move);
        }

        private void Clap()
        {

        }

        private void No()
        {

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

                RotateToward(targetPos);
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