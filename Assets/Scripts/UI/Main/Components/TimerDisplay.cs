using Events;
using Extensions.Unity.MonoHelper;
using Extensions.Unity.Utils;
using Settings;
using UnityEngine;
using Zenject;

namespace UI.Main.Components
{
    public class TimerDisplay : UITMP
    {
        [Inject] private LevelEvents LevelEvents{get;set;}
        [SerializeField] private float _minutes;
        private float _timeRemaining;
        private bool _timerRunning;
        private RoutineHelper _timerRoutine;

        private void Awake()
        {
            _timerRoutine = new RoutineHelper(this, null, TimerUpdate);
        }

        private void TimerUpdate()
        {
            if (_timerRunning)
            {
                if (_timeRemaining > 0)
                {
                    _timeRemaining -= Time.deltaTime;

                    int minutesLeft = Mathf.FloorToInt(_timeRemaining / 60);
                    int secondsLeft = Mathf.FloorToInt(_timeRemaining % 60);

                    RenderTxt(minutesLeft, secondsLeft);
                }
                else
                {
                    _timerRunning = false;
                    _timeRemaining = 0;

                    RenderTxt(0,0);

                    Debug.LogWarning("FailCond: Time Out");
                    LevelEvents.TimeOut?.Invoke();
                    _timerRoutine.StopCoroutine();
                }
            }
        }

        private void RenderTxt(int minutesLeft, int secondsLeft)
        {
            _myTMP.text = $"Timer: {minutesLeft:00}:{secondsLeft:00}";
        }

        private void StartTimer(float startMinutes)
        {
            _minutes = startMinutes;
            _timeRemaining = _minutes * 60;
            _timerRunning = true;
            
            _timerRoutine.StartCoroutine();
        }

        protected override void RegisterEvents()
        {
            LevelEvents.LevelLoaded += OnLevelLoaded;
        }

        private void OnLevelLoaded(LevelData arg0)
        {
            StartTimer(arg0.LevelTimerMins);
        }

        protected override void UnRegisterEvents()
        {
            LevelEvents.LevelLoaded -= OnLevelLoaded;
        }
    }
}