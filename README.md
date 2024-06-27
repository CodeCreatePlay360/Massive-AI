<h4 align="left">
MassiveAI is a library of AI programming and logic-design tools for creating autonomous agents and logic driven systems. It also includes tools for communication, spatial querying, and navigation. Designed with ease of use and flexibility in mind, all components of this library can function independently or can be integrated seamlessly. Currently, most of the MassiveAI components are written in C#. However, the long-term goal is to port it to C++, expanding its usability across different game engines and software platforms.
</h4>

<h3 align="center">Community</h3>

<p align="center">
<a href='https://discord.gg/WZ3GZCvVtg' target="_blank"><img alt='Discord' src='https://img.shields.io/badge/Discord-5865F2?style=plastic&logo=discord&logoColor=white'/></a>
<a href='https://www.patreon.com/massiveai' target="_blank"><img alt='Patreon' src='https://img.shields.io/badge/Patreon-F96854?style=plastic&logo=patreon&logoColor=white'/></a>
<a href='https://github.com/CodeCreatePlay360/Massive-AI' target="_blank"><img alt='Unity' src='https://img.shields.io/badge/Reddit-FF4500?style=plastic&logo=reddit&logoColor=white'/></a>
<a href='https://github.com/CodeCreatePlay360/Massive-AI' target="_blank"><img alt='Unity' src='https://img.shields.io/badge/YouTube-FF0000?style=plastic&logo=youtube&logoColor=white'/></a>
</p>

### Table of contents
1. [Install](https://github.com/CodeCreatePlay360/Massive-AI)
2. [Getting Started](https://github.com/CodeCreatePlay360/Massive-AI)
3. **Behavior Design Tools:** [StateMachine](https://github.com/CodeCreatePlay360/Massive-AI) - [Action Sequence](https://github.com/CodeCreatePlay360/Massive-AI)
4. **Decision-Making:** [FuzzyLogic](https://github.com/CodeCreatePlay360/Massive-AI)
5. **Communication:** [Messaging]() - [Events]()
6. **Tools for game AI characters:** [Campsite]()

> 💫 It takes a considerable amount of time and effort and maintain Massive AI and keeping it updated with new features and tools, so if you find it useful for your projects, then consider giving it a star on GitHub, it will help Massive AI reach more audience. Also check out the demo projects in 'Demos' folder, showcasing various tools and features of this library.

🛠️ **Install:-** Download this repository and import it in root of your Unity project, you can safely put it in your existing projects, all code is contained in a separate namespace so zero conflicts with other systems is guaranteed.

<h2 align="center">🟦 Behavior Design Tools</h2>

### 🟦 State Machine

Generic StateMachine implementation with a logical separation of a state initialization and disposal logic from regular update processes. The execution of a state's OnEnter and OnExit methods can extend over multiple frames if necessary, allowing for complex initialization or clean-up procedures. After initialization, update process runs every frame, ensuring smooth execution of the state's main functionality. When a state transition occurs, the exit method of the current state is invoked, followed by the start method of the new state, this separation of start and exit logic from the regular update process provides flexibility and efficiency, accommodating diverse state behaviors and ensuring seamless state transitions in dynamic applications.

```
class State : StateBase<Agent>
{
    public Chase(Agent owner) : base("ChaseState", owner)
    {
    }

    private bool OnEnter()
    {
        // the method is executed every frame until it returns true
        if (!someCondition)
            return false;

        return true;
    }

    public void OnUpdate()
    {
        // this method is executed every frame after 'OnEnter' has finished execution
    }

    private bool OnExit()
    {
        // the method is executed every frame until it returns true
        if (!someCondition)
            return false;

        return true;
    }
}
```

There are two methods for defining a state: one involves specifying separate OnEnter, OnExit and Update methods and then using the CreateState method of StateMachine, which creates and returns the new state object, alternatively you can define a state directly as a separate 'State' object.

```
// character states
private StateBase<Agent> idleState; 
private Chase chaseState;


void Start()
{
    // initialize state machine
    stateMachine = new StateMachine<Agent>(this);

    // initialize character states
    idleState = stateMachine.CreateState(label:"IdleState", enterMethod: IdleEnter, updateMethod: IdleUpdate, exitMethod:null);
    chaseState = new Chase(this);

    // add states to state machine
    stateMachine.AddState(idleState);
    stateMachine.AddState(chaseState);

    // switch to idle state, on start
    stateMachine.SwitchState(state: idleState);
}
```

To address specific situations, such as a character's death, that must be acknowledged regardless of the character's current state, a GlobalState and a GlobalStateTrigger can be defined, GlobalStateTrigger sends the request to switch to GlobalState, the default is that StateMachine waits for the OnExit method of current executing state to finish executing before switching to GlobalState, however you can override this behavior by setting ForceTriggerGlobalState to True for all individual state where you want to override this behavior, in this case StateMachine will switch to OnEnter of GlobalState without waiting for OnExit method for current executing state to finish.

```
void Start()
{
    stateMachine.AddGlobalState(state:new DeathState(agent), trigger: IsDead);
}

public bool IsDead()
{
    if (health <= 0)
        return true;

    return false;
}
```

### 🟦 Action Sequence
**Introduction:-** An ActionSequence provide a structured way to manage complex sequences of actions, offering modularity, flexibility, and ease of use in organizing actions. It could be particularly useful in scenarios where autonomous agents or systems has to perform a sequences of actions in response to different events or conditions, in short the ActionSequence aims to address the following four issues.

1. Modularity and Reusability
2. Readability and Understandability
3. Flexibility and Extensibility
4. Debugging and Error Handling

**Technical details:-** A GameAction in an ActionSequence is similar to a State objects in a StateMachine, it has separate methods (OnStart, OnUpdate, OnExit) for initialization, update and exit logic implementations. To address situations where enter and exit logic require execution over multiple frames both OnStart and OnExit methods can continue to be executed over multiple frames. The code below shows a basic GameAction implementation.

```
public class TestAction : GameAction<AICharacter>
{
    // constructor
    public TestAction(AICharacter owner, float duration) : base(owner, duration)
    {
    }

    public override bool OnStart()
    {
        return true;
    }

    public override ExecutionResult Update()
    {
        return ExecutionResult.Running;
    }

    public override bool OnEnd()
    {
        return true;
    }
}
```

In contrast to OnStart and OnEnd methods, which return a Boolean to communicate their exit status with ActionSequence manager, the Update method returns the ExecutionResult enumeration to signal the ActionSequence-Manager to take appropriate action based on the returned value.

```
public override ExecutionResult Update()
{
   // ExecutionResult enum has four values
   // 1. ExecutionResult.Running: keep running
   // 2. ExecutionResult.Success: go to next action
   // 3. ExecutionResult.ResetFromCurrent: restart current action
   // 4. ExecutionResult.ResetFromPreceding: restart from action preceding this action
   // 5. ExecutionResult.ResetFromStart: restart from first action in the stack
              
    return ExecutionResult.Running;
}
```

The following code sample below shows a complete initialization and usage of an ActionSequence.

```
public class ActionSeqTest : MonoBehaviour
{
    private ActionsSeq<AICharacter> actionsSeq;
    private AICharacter owner;


    void Start()
    {
        // instantiate or get the owner
        owner = new();

        // instantiate actions
        ActnFly  testActn1 = new(owner);
        ActnSwim testActn2 = new(owner);
        ActnWalk actnWalk3 = new(owner);

        // create actions list
        GameAction<AICharacter>[] actionsList = new GameAction<AICharacter>[]
        {
            testActn1,
            testActn2,
            actnWalk3,
        };

        // initialize the ActionSeq
        actionsSeq = new(owner, actions:actionsList);
    }

    void Update()
    {
        actionsSeq.Update();
    }
}
```

Although, you can directly and abruptly end a sequence by stop updating it, however there is an elegant way to stop the sequence using a StopProcedure enumeration, which triggers the sequence to halt gracefully, responding to the specified value within the enum.

```
// StopProcedure enum has following 3 values.
// 1. StopAbruptly:     stop updating immediately
// 2. StopAfterCurrent: stop after the current action is finished.
// 3. StopAfterAll:     stop after all actions have finished. 

// here is a basic usage in code
actionsSeq.SetStopProcedure(ActionsSeq<AICharacter>.StopProcedure.StopAfterCurrent);

// use the 'IsStopped' method to check if any action in the sequence is updating.
actionsSeq.IsStopped()
```

<h2 align="center">🟦 Decision Tools</h2>

### 🟦 Fuzzy Logic

Fuzzy logic is a form of logic that allows reasoning to be approximate rather than fixed and exact. It is particularly useful in applications where human-like reasoning is needed, as it can handle the concept of partial truth—truth values between completely true and completely false. This is in contrast to classical logic, where every statement must be either true or false.  

To learn more about FuzzyLogic, it is highly recommended to see chapter #10 of 'Programming Game AI by Example' by 'Mat Buckland'. The code below demonstrates a basic usage of FuzzyLogic using the current implementation. See the comments along code for detailed explanation.

```
using UnityEngine;
using System.Collections.Generic;
using MassiveAI.Fuzzy.MemberFunctions;


namespace MassiveAI.Fuzzy
{
    /// <summary>
    /// Basic class demonstrates the use of fuzzy logic in game AI.
    /// It evaluates the health status of a character and determines
    /// whether the character should flee. The fuzzy logic system uses
    /// linguistic variables to map health levels and make decisions.
    /// </summary>
    public class FuzzyBasic : UnityEngine.MonoBehaviour
    {
        // Public field to set the health value via the Unity Inspector
        [Range(0, 100)]
        public int healthValue = 0;

        // Private fields for fuzzy input and output
        private FuzzyInput healthStatus;  // Fuzzy input for health status
        private FuzzyOutput shouldFlee;   // Fuzzy output for flee decision

        // Constants representing different health levels
		// (fuzzy linguistic variables)
        private const int low = 0;
        private const int medium = 1;
        private const int high = 2;

        // Cached value to store the last health value
        private int lastHealthVal;


        public void Start()
        {
            // Initialize fuzzy input and output
            healthStatus = new FuzzyInput(() => healthValue);
            shouldFlee = new FuzzyOutput();

            // Map health levels to fuzzy sets (membership functions)
            healthStatus.Set(low, new LeftShoulder(0, 15, 30));
            healthStatus.Set(medium, new Triangle(15, 45, 60));
            healthStatus.Set(high, new RightShoulder(45, 70, 100));

            // Map flee decision levels to fuzzy sets
            shouldFlee.Set(low, new Triangle(-0.5, 0.0, 0.5));
            shouldFlee.Set(medium, new Trapezoidal(0, 0.3, 0.7, 1));
            shouldFlee.Set(high, new Triangle(0.55, 1, 1.5));

            // Create fuzzy rules for decision making
            FuzzyRule.If(healthStatus.Is(high)).Then(shouldFlee.Is(low));
            FuzzyRule.If(healthStatus.Is(medium)).Then(shouldFlee.Is(medium));
            FuzzyRule.If(healthStatus.Is(low)).Then(shouldFlee.Is(high));

            // Cache the initial health value
            lastHealthVal = healthValue;
        }

        private void Update()
        {
            // Check if the health value has changed
            if (lastHealthVal != healthValue)
            {
                UnityEngine.Debug.Log($"Flee(Health: {healthStatus.Value}) = {shouldFlee.Evaluate()}");

                // Update the cached health value
                lastHealthVal = healthValue;
            }
        }
    }
}
```

It is also possible to use and combine conditional 'if, else, else-if' statements and logical 'and, or' operators on fuzzy inputs (antecedents) and outputs (consequents).

```
FuzzyRule.If(condition01.Is(low)).And(condition02.Is(high)).Then(consequence01.Is(low)).And(consequence02.Is(high));
```

See "AdvancedFuzzy.cs" for an advanced usage of FuzzyLogic, in your projects.
