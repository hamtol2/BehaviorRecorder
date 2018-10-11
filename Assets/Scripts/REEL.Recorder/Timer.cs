namespace REEL.Recorder
{
    [System.Serializable]
    public class Timer
    {
        public delegate void IntervalDelegate();
        public event IntervalDelegate IntervalEvent;

        private float interval = 0f;
        private Timer intervalTimer;
        [UnityEngine.SerializeField] private float elapsedTime = 0f;

        // Constructors.
        public Timer() { }
        public Timer(float interval, IntervalDelegate intervalEvent = null)
        {
            this.interval = interval;
            intervalTimer = new Timer();
            if (intervalEvent != null) SubsribeIntervalEvent(intervalEvent);
        }

        public void SetTimer(float interval, IntervalDelegate intervalEvent = null)
        {
            this.interval = interval;
            intervalTimer = new Timer();
            if (intervalEvent != null) SubsribeIntervalEvent(intervalEvent);
        }

        // Timer Update.
        public void Update(float deltaTime)
        {
            elapsedTime += deltaTime;

            if (intervalTimer != null)
            {
                intervalTimer.Update(deltaTime);
                if (intervalTimer.GetElapsedTime > interval)
                {
                    if (IntervalEvent != null) IntervalEvent();
                    intervalTimer.Reset();
                }
            }
        }

        public void SubsribeIntervalEvent(IntervalDelegate intervalDele)
        {
            IntervalEvent += intervalDele;
        }

        public void UnsubscribeIntervalEvent(IntervalDelegate intervalDele)
        {
            if (IntervalEvent != null)
                IntervalEvent -= intervalDele;
        }

        public float GetElapsedTime
        {
            get { return elapsedTime; }
        }

        // Reset Timer.
        public void Reset()
        {
            elapsedTime = 0f;
            if (intervalTimer != null)
                intervalTimer.Reset();
            //IntervalEvent = null;
        }
    }
}