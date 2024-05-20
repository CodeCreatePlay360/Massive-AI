using System.Reflection;


namespace MassiveAI
{
    public abstract class StateBase<T>
    {
        public readonly string label;
        public readonly short Priority = -1;
        public readonly T Owner;

        public bool FORCE_TRIGGER_TO_GLOBAL_STATE = false;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner"></param>
        public StateBase(string label, T owner)
        {
            this.label = label;
            this.Owner = owner;

            FetchMethods();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner"></param>
        public StateBase(string label, T owner, short priority)
        {
            this.label = label;
            this.Owner = owner;
            this.Priority = priority;

            FetchMethods();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner"></param>
        public StateBase(string label, T owner, System.Func<bool> enter, System.Action update, System.Func<bool> exit)
        {
            this.label = label;
            this.Owner = owner;

            this.Enter = enter;
            this.Update = update;
            this.Exit = exit;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner"></param>
        public StateBase(string label, T owner, short priority, System.Func<bool> enter, System.Action update, System.Func<bool> exit)
        {
            this.label = label;
            this.Owner = owner;
            this.Priority = priority;

            this.Enter = enter ?? this.OnEnter;
            this.Update = update ?? this.OnUpdate;
            this.Exit = exit ?? this.OnExit;
        }

        private void FetchMethods()
        {
            System.Type type = this.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            MethodInfo mInfo = type.GetMethod("OnEnter", flags);
            if (mInfo != null && mInfo.ReturnType == typeof(bool))
                this.Enter = System.Linq.Expressions.Expression.Lambda<System.Func<bool>>
                    (System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(this), mInfo)).Compile();
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogFormat("User defined implementation of 'OnEnter' not found for state '{0}'," +
                    " using default implementation now.",
                    label);
#endif
                this.Enter = OnEnter;
            }

            mInfo = type.GetMethod("OnUpdate", flags);
            if (mInfo != null && mInfo.ReturnType == typeof(void))
                this.Update = System.Linq.Expressions.Expression.Lambda<System.Action>(System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(this), mInfo)).Compile();
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogFormat("User defined implementation of 'OnUpdate' not found for state '{0}'," +
                    " using default implementation now.",
                    label);
#endif
                this.Update = OnUpdate;
            }

            mInfo = type.GetMethod("OnExit", flags);
            if (mInfo != null && mInfo.ReturnType == typeof(bool))
                this.Exit = System.Linq.Expressions.Expression.Lambda<System.Func<bool>>
                    (System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(this), mInfo)).Compile();
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogFormat("User defined implementation of 'OnExit' not found for state '{0}'," +
                    " using default implementation now.",
                    label);
#endif
                this.Exit = OnExit;
            }
        }

        // ---------------------------------------------------------------------------------------------------------------------- //
        // default implementations of OnEnter, OnUpdate and OnExit methods, if the user has not provided any custom implementations
        private bool OnEnter()
        {
            return true;
        }

        private void OnUpdate()
        {
        }

        private bool OnExit()
        {
            return true;
        }
        // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ //

        public System.Func<bool> Enter { get; private set; } = null;
        public System.Action Update { get; private set; } = null;
        public System.Func<bool> Exit { get; private set; } = null;
    }
}
