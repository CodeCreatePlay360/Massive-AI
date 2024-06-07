<h4 align="left">
Massive AI is your ultimate library of decision-making algorithms and logic-design tools, perfect for creating realistic and immersive AI characters in games and simulations. It also includes tools for communications, spatial querying, and navigation. Designed with ease of use and flexibility in mind, our systems can function independently or be seamlessly integrated.

Currently, Massive AI is available exclusively for the Unity engine. However, we are actively working on porting Massive AI to C++, which will expand its usability across other game engines.
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
3. [Behavior Design Tools](https://github.com/CodeCreatePlay360/Massive-AI)
   - [StateMachine](https://github.com/CodeCreatePlay360/Massive-AI)
   - [Action Sequence](https://github.com/CodeCreatePlay360/Massive-AI)
4. [Decision-Making Algorithms](https://github.com/CodeCreatePlay360/Massive-AI)
   - [Fuzzy Logic](https://github.com/CodeCreatePlay360/Massive-AI)


> ⭐ It takes a considerable amount of time and effort and maintain Massive AI and keeping it updated with new features and tools, so if you find it useful for your projects, then consider giving it a star on GitHub, it will help Massive AI reach more audience. Also check out the demo projects in 'Demos' folder, showcasing various tools and features of this library.


***

### 🛠️ Install
Download this repository and import it in root of your Unity project, you can safely put it in your existing projects, all code is contained in a separate namespace so zero conflicts with other systems is guaranteed.

<h2 align="center">🔷 Behavior Design Tools</h2>

### 🟦 State Machine

Generic 'StateMachine' implementation with a logical separation of state's initialization and disposal logic from regular update processes. The execution of state's 'OnEnter' and 'OnExit' methods can extend over multiple frames if necessary, allowing for complex initialization or clean-up procedures. After initialization, update process runs every frame, ensuring smooth execution of the state's main functionality. When a state transition occurs, the exit method of the current state is invoked, followed by the start method of the new state, this separation of start and exit logic from the regular update process provides flexibility and efficiency, accommodating diverse state behaviors and ensuring seamless state transitions in dynamic applications.

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

There are two methods for defining an AI character’s ‘State’: one involves specifying separate ‘OnEnter,’ ‘OnExit,’ and ‘Update’ methods and then using the ‘CreateState’ method of ‘StateMachine’, which creates and returns the new ‘State’ object, alternatively you can define a state directly as a separate ‘State’ object.

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

To address specific situations, such as a character’s death, that must be acknowledged regardless of the character’s current state, a ‘GlobalState’ and a ‘GlobalStateTrigger’ can be defined, ‘GlobalStateTrigger’ sends the request to switch to ‘GlobalState’, the default is that ‘StateMachine’ waits for the ‘OnExit’ method of current executing state to finish executing before switching to ‘GlobalState’, however you can override this behavior by setting ForceTriggerToGlobalState to ‘True’ for all individual state where you want to override this behavior, in this case ‘StateMachine’ will switch to 'OnEnter' of ‘GlobalState’ without waiting for ‘OnExit’ method for current executing state to finish.

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
**Introduction:-** An 'Action Sequence' provide a structured way to manage complex sequences of actions, offering modularity, flexibility, and ease of use in organizing gameplay logic. It could be particularly useful in scenarios where you have NPCs or player characters performing sequences of actions in response to different events or conditions, in short the 'ActionSequence' aims to address the following four issues.

1. **Modularity and Reusability**: The Action Sequence approach promotes modularity by encapsulating each action as a separate class. This can make it easier to reuse and maintain individual actions, as well as compose them into different sequences. If-else statements might become cumbersome and harder to maintain as the number of actions and conditions grows.

2. **Readability and Understandability**: Action sequences can make the code more readable and understandable by providing a clear structure for defining and managing sequences of actions. On the other hand, complex if-else statements might make the code harder to follow, especially if there are many branching conditions.

3. **Flexibility and Extensibility**: The Action Sequence approach can offer more flexibility and extensibility in managing the flow of actions. It allows for easy addition, removal, or modification of actions within a sequence without affecting other parts of the code. If-else statements, especially when deeply nested, can become rigid and difficult to extend or modify.

4. **Debugging and Error Handling**: Action sequences might offer better debugging capabilities by isolating the logic of each action into separate classes. This can make it easier to identify and fix issues related to specific actions. If-else statements can sometimes lead to spaghetti code and make debugging more challenging.

**Technical details:-** A 'GameAction' in an 'ActionSequence' is similar to the 'State' objects in the 'StateMachine' implementation, it has separate methods (OnStart, OnUpdate, OnExit) for initialization, update and exit logic implementations, to address situations where enter and exit logic require execution over multiple frame both 'OnStart' and 'OnExit' methods can continue to be executed over multiple frames. The code below shows a basic 'GameAction' implementation.

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

In contrast to 'OnStart' and 'OnEnd' methods, which return a Boolean to communicate their exit status with 'ActionSequence' manager, the 'Update' method returns the 'ExecutionResult' enumeration to signal the 'Action-Sequence' manager to take appropriate action based on the returned value.

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

The following code sample below shows a complete initialization and usage of an 'ActionSequence'.

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

Although, you can directly stop a sequence by stop updating it, however there is an elegant way to stop the sequence using a 'StopProcedure' enumeration, which triggers the sequence to halt gracefully, responding to the specified value within the enum.

```
// 'StopProcedure' enum has following 3 values.
// 1. StopAbruptly:     stop updating immediately
// 2. StopAfterCurrent: stop after the current action is finished.
// 3. StopAfterAll:     stop after all actions have finished. 

// here is a basic usage in code
actionsSeq.SetStopProcedure(ActionsSeq<AICharacter>.StopProcedure.StopAfterCurrent);

// use the 'IsStopped' method to check if any action in the sequence is updating.
actionsSeq.IsStopped()
```

<h2 align="center">🔷 Decision-Making Algorithms</h2>

### 🟦 Fuzzy Logic

Fuzzy logic is a form of logic that allows reasoning to be approximate rather than fixed and exact. It is particularly useful in applications where human-like reasoning is needed, as it can handle the concept of partial truth—truth values between "completely true" and "completely false." This is in contrast to classical logic, where every statement must be either true or false.

**Key Concepts of Fuzzy Logic**  
- **Fuzzy Sets** In classical set theory, an element either belongs to a set or does not (binary logic: 0 or 1). In fuzzy sets, an element can partially belong to a set with a degree of membership ranging from 0 to 1. For example, consider the set of 'tall people.' Instead of defining a strict height threshold, fuzzy logic allows for varying degrees of 'tallness.'

- **Membership Functions** These functions define how each point in the input space is mapped to a degree of membership between 0 and 1. Common shapes for membership functions include 'Triangular', 'Trapezoidal', and 'Gaussian'.

- **Fuzzy Rules** Fuzzy logic systems use 'if-then' rules to model human reasoning. For example, 'If the distance to the enemy is close, then the threat level is high.' These rules are defined using linguistic variables, which are described using fuzzy sets.

- **Fuzzification** The process of transforming crisp (exact) inputs into fuzzy values using membership functions.
For example, converting the exact distance of 30 meters into a fuzzy value that indicates 'close,' 'medium,' or 'far.'

- **Inference** The fuzzy inference process applies fuzzy rules to the fuzzified inputs to derive fuzzy outputs. There are several methods for inference, with the most common being 'Mamdani' and 'Sugeno' methods.

- **Defuzzification:**
The process of converting fuzzy output values back into crisp values. Common defuzzification methods include the 'Centroid' method (finding the center of gravity of the fuzzy set) and the 'Maximum Membership' method.

The code below demonstrates a basic usage of 'FuzzyLogic' in your game. For more comprehensive examples, see the code included in the demos folder.

```
// Initialize a constant for the current amount of ammunition
const int ammoAmount = 55;

// Define constants representing fuzzy sets for ammunition levels
const int low = 0;
const int medium = 1;
const int high = 2;

// Create a FuzzyInput to represent the ammunition status
FuzzyInput ammoStatus = new(() => ammoAmount);
ammoStatus.Set(low, new Triangle(0, 25, 50));
ammoStatus.Set(medium, new Triangle(25, 50, 75));
ammoStatus.Set(high, new Triangle(50, 75, 100));

// Create a FuzzyOutput to represent the reload necessity
FuzzyOutput shouldReload = new();
shouldReload.Set(low, new Triangle(-0.5, 0, 0.5));
shouldReload.Set(medium, new Triangle(0, 0.5, 1));
shouldReload.Set(high, new Triangle(0.5, 1, 1.5));

// Define fuzzy rules to determine the reload necessity based on ammo status
FuzzyRule.If(ammoStatus.Is(high)).Then(shouldReload.Is(low));
FuzzyRule.If(ammoStatus.Is(medium)).Then(shouldReload.Is(medium));
FuzzyRule.If(ammoStatus.Is(low)).Then(shouldReload.Is(high));

// Evaluate the fuzzy output for the reload necessity and log the result
string result = $"Reload desirability when (ammoStatus: {ammoStatus.Value}) = {shouldReload.Evaluate()}";
UnityEngine.Debug.Log(result);
```
