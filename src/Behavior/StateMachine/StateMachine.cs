using System.Linq.Expressions;
using System.Reflection;


namespace MassiveAI
{
    public class StateMachine<T>
    {
        private System.Collections.Generic.Dictionary<string, StateBase<T>> statesmap;

        private StateBase<T> globalState;
        private StateBase<T> currentState = null;
        private StateBase<T> nextState = null;
        private StateBase<T> lastState = null;

        private bool isGlobalState = false;
        private StateBase<T> preGlobalState = null;          // the last state before global state
        private System.Func<bool> globalStateTrigger = null; // global state condition callback, sm will switch
                                                             // to global state as long as this will return true

        private bool needTransition = false;

        public StateBase<T> CurrentState { get { return currentState; } }
        public string CurrentStateLabel { get { return currentState?.label;  } }


        public T Owner 
        { 
            get; private set;
        }

        public System.Collections.Generic.Dictionary<string, StateBase<T>> StatesMap
        {
            get 
            {
                if (statesmap == null) statesmap = new System.Collections.Generic.Dictionary<string, StateBase<T>>();
                return statesmap;
            }
        }

        public StateMachine(T owner, UnityEngine.MonoBehaviour mono)
        {
            this.Owner = owner;
        }

        public StateMachine(T owner, StateBase<T> globalState)
        {
            this.Owner = owner;
            this.globalState = globalState;
        }

        public void AddState(StateBase<T> newState)
        {
            if (!CanAddState(state: newState, label:newState.label))
                return;

            StatesMap[newState.label] = newState;
        }

        public StateBase<T> CreateState(string label, System.Action updateMethod, System.Func<bool> enterMethod=null, System.Func<bool> exitMethod=null,
            short statepriority=-1)
        {
            PlaceholderState placeHolderState = new PlaceholderState(label, Owner, statepriority, enterMethod, updateMethod, exitMethod);
            return placeHolderState;
        }

        public void AddGlobalState(StateBase<T> state, System.Func<bool> trigger)
        {
            globalState = state;
            this.globalStateTrigger = trigger;
        }

        public StateBase<T> AddGlobalState(string statename, System.Action update, System.Func<bool> enter = null, System.Func<bool> exit = null)
        {
            if (!CanAddState(label:statename))
                return null;

            PlaceholderState placeHolderState = new PlaceholderState(statename, Owner, -1, enter, update, exit);
            globalState = placeHolderState;
            return globalState;
        }

        public bool CanAddState(StateBase<T> state = null, string label = null)
        {
            if (state != null && StatesMap.ContainsValue(state))
            { UnityEngine.Debug.LogFormat("Unable to add state '{0}' value already exists.", state.label); return false; }

            if (label != null && StatesMap.ContainsKey(label))
            { UnityEngine.Debug.LogFormat("Unable to add state '{0}' key already exists.", state.label); return false; }

            return true;
        }

        public bool CanSwitchToLastState()
        {
            return this.lastState != null;
        }

        public bool IsOK()
        {
            return UnityEngine.Application.isPlaying;
        }

        public void SwitchState(StateBase<T> newState = null)
        {
            if (currentState == newState)
                return;

            if(globalState != null && newState == globalState && currentState.FORCE_TRIGGER_TO_GLOBAL_STATE)
            { _currentStateExited = true; }

            StateBase<T> nextState;

            if (newState != null)
                nextState = newState;
            else
            {
                UnityEngine.Debug.LogErrorFormat("[DemonAI - StateMachine] Unable to switch state; state {0} does not exits.!", newState);
                return;
            }

            needTransition = true;
            this.nextState = nextState;
        }

        /// <summary>
        /// Switches to the last state if it exists, it is up to the user to
        /// make sure last state exists using 'CanSwitchToLastState' function call.
        /// </summary>
        public void SwitchToLastState()
        {
            if (lastState != null)
                SwitchState(lastState);
        }

        private bool _currentStateExited = false;
        public void Transition()
        {
            if (IsOK() && currentState != null && !_currentStateExited && !currentState.Exit())
                return;

            _currentStateExited = true;

            if (IsOK() && nextState != null && !nextState.Enter())
                return;

            currentState = nextState;
            _currentStateExited = false;
            needTransition = false;
        }

        public void Update()
        {
            if (globalState != null && globalStateTrigger != null && globalStateTrigger())
            {
                if(!isGlobalState)
                {
                    isGlobalState = true;
                    preGlobalState = currentState;
                    SwitchState(globalState);
                }
            }

            if (currentState != null && !needTransition)
                currentState.Update();
            else
                Transition();
        }

        /// <summary>
        /// 
        /// </summary>
        public class PlaceholderState : StateBase<T>
        {
            public PlaceholderState(string label, T owner, short priority, System.Func<bool> enter, System.Action update,
                System.Func<bool> exit) :
                base(label, owner, priority, enter, update, exit)
            {
            }
        }
    }
}
