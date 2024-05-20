
/// <summary>
/// 
/// </summary>
namespace MassiveAI
{
    [System.Serializable]
    public class ActionsSeq<T>
    {
        public struct CurrentActnInfo
        {
            public GameAction<T> actn;
            public int execMethodID;


            public CurrentActnInfo(GameAction<T> actn, int execMethodID)
            {
                this.actn = actn;
                this.execMethodID = execMethodID;
            }
        }

        public enum StopProcedure
        {
            StopAbruptly,
            StopAfterCurrent,
            StopAfterAll,
        }


        private GameAction<T>[] actionsArray;          // list of actions, as set by user

        private readonly T owner;                      // the owner of this action list
        private int idx = -1;                          // index of current executing action
        private GameAction<T> currentAction;           // current action which is executing
        private GameAction<T> lastActn;                // last executing action

        private bool isInTransition;                   // 
        private StopProcedure stopProcedure;
        private bool shouldStop;                       //
        private bool isStopped;                        // 

        public CurrentActnInfo currentActnInfo;        //
        public bool currentActnInfoUpdated;            //

        // cache
        private GameAction<T>.ExecutionResult successStatus;


        // constructor
        public ActionsSeq(T _owner, GameAction<T>[] actions=default)
        {
            owner = _owner;
            actionsArray = actions;

            idx = -1;

            stopProcedure = StopProcedure.StopAfterCurrent;
            shouldStop = false;
            isStopped  = false;

            currentActnInfoUpdated = false;
        }

        public void Update()
        {
            if (actionsArray.Length == 0)
                return;

            // update stop procedure first
            if (shouldStop)
            {
                switch(stopProcedure)
                {
                    case StopProcedure.StopAbruptly:
                        isStopped = true;
                        break;

                    // ** other two procedures are checked in UpdateCurrentActnInfo method
                }
            }
            // ---------------------------

            if (isStopped)
                return;

            if (currentAction == null)
                MoveToFirst(); 

            if(currentAction != null)
                currentAction.InternalUpdate();

            if (isInTransition)
            {
                Transition(lastActn);
                return;
            }

            successStatus = currentAction.Update();

            if (successStatus == GameAction<T>.ExecutionResult.Success)
                Step(previous: false);

            else
            {
                switch(successStatus)
                {
                    case GameAction<T>.ExecutionResult.ResetFromStart:
                        MoveToFirst();
                        break;

                    case GameAction<T>.ExecutionResult.ResetFromLast:
                        Step(previous: true);
                        break;

                    case GameAction<T>.ExecutionResult.ResetFromCurrent:
                        currentAction.OnStart();
                        break;
                }
            }
        }

        /// <summary>
        /// Move one step forward or backward the actions list.
        /// </summary>
        private void Step(bool previous=false)
        {
            lastActn = currentAction;

            if (previous)
                idx--;
            else
                idx++;

            idx = UnityEngine.Mathf.Clamp(idx, 0, actionsArray.Length - 1);

            isInTransition = true;
            _currentActionExited = false;
        }

        private void MoveToFirst()
        {
            lastActn = currentAction;
            idx = 0;

            isInTransition = true;
            _currentActionExited = false;
        }


        private bool _currentActionExited = false;
        private void Transition(GameAction<T> lastActn)
        {
            currentAction = actionsArray[idx];

            if (lastActn != null)
            {
                if (!_currentActionExited && !lastActn.OnEnd())
                { UpdateCurrentActnInfo(GameAction<T>.END_METHOD_ID); return; }

                if (!_currentActionExited)
                { _currentActionExited = true; lastActn.Reset(); }

                UpdateCurrentActnInfo(GameAction<T>.START_METHOD_ID);
                if (!isStopped && !currentAction.OnStart())
                    return;
            }
            else
            {
                if (!_currentActionExited)
                { _currentActionExited = true; }

                UpdateCurrentActnInfo(GameAction<T>.START_METHOD_ID);
                if (!isStopped && !currentAction.OnStart())
                    return;
            }

            isInTransition = false;
            UpdateCurrentActnInfo(GameAction<T>.UPDATE_METHOD_ID);
        }


        private int _currentExecMethodID = -1;
        private int _lastExecMethodID = -1;
        private void UpdateCurrentActnInfo(int newExecutingMethodID)
        {
            if(_currentExecMethodID != newExecutingMethodID)
            {
                // UnityEngine.Debug.LogFormat("Updated {0}", currentExecutingMethod);
                currentActnInfo      = new CurrentActnInfo(currentAction, newExecutingMethodID);
                _lastExecMethodID    = _currentExecMethodID;
                _currentExecMethodID = newExecutingMethodID;
            }


            if (shouldStop)
            {
                switch (stopProcedure)
                {
                    case StopProcedure.StopAfterCurrent:
                        if (_lastExecMethodID == GameAction<T>.END_METHOD_ID)
                            isStopped = true;
                        break;

                    case StopProcedure.StopAfterAll:
                        if (idx == actionsArray.Length - 1 && _currentExecMethodID == GameAction<T>.END_METHOD_ID)
                            isStopped = true;
                        break;
                }
            }
        }

        public void Start()
        {
            this.isStopped = false;
            this.shouldStop = false;
        }

        public void SetStopProcedure(StopProcedure stopProcedure)
        {
            this.shouldStop = true;
            this.stopProcedure = stopProcedure;
        }

        public CurrentActnInfo GetCurrentActnInfo() => currentActnInfo;

        public T GetOwner() => owner;

        public bool IsOK() => isStopped;

        public bool IsStopped() => false;
    }
}
