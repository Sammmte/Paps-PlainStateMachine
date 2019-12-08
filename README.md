# Paps FSM 

A simple finite state machine made in c#. Developed in a Unity 3D Project.

```csharp
//Create your state machine using any type you want for state and trigger identifiers

var guardEnemyFSM = new FSM<State, Trigger>();

//State objects must implement IState interface

IState patrolState = new PatrolState();
IState searchTarget = new SearchTargetState();

guardEnemyFSM.AddState(State.Patrol, patrolState);
guardEnemyFSM.AddState(State.SearchTarget, searchTarget);

//You must specify a initial state before start using the state machine
//otherwise an exception will be thrown

guardEnemyFSM.SetInitialState(State.Patrol);

//Define transitions between states.

guardEnemyFSM.AddTransition(
    State.Patrol, 
    Trigger.PatrolFinished, 
    State.SearchTarget);

guardEnemyFSM.AddTransition(
    State.SearchTarget, 
    Trigger.StartPatrol, 
    State.Patrol);

//Start your state machine

guardEnemyFSM.Start();

guardEnemyFSM.IsInState(State.Patrol); //True

guardEnemyFSM.Trigger(Trigger.PatrolFinished);

guardEnemyFSM.IsInState(State.SearchTarget); //True
```

This project, as well as the example above, as well as this clarification and README, was inspired by [Stateless](https://github.com/dotnet-state-machine/stateless).

### Features

 * Generic state and trigger identifiers
 * Entry, update and exit events for states
 * Guard conditions (or clauses) for conditional transitions
 * Useful extensions
 * Event dispatching
 * Reentrant states

### Entry, Update and Exit Events

When a state machine is started or when a state becomes the current state the IState.Enter() function of the state object will be called.

The IState.Exit() function will be called when the current state gets replaced after a transition or when a state machine is stopped.

IState.Update() function gets called on the current state when you call the IFSM.Update() function of your state machine object.

### Guard Conditions

Add guard conditions to your transitions to wisely choose the next state. e.g.:

```csharp
//You can create your custom guard condition classes implementing 
//the IGuardCondition<TState, TTrigger> interface
IGuardCondition<State, Trigger> startPatrolGuard = new MyCustomGuardCondition();

guardEnemyFSM.AddGuardConditionTo(
    State.SearchTarget, 
    Trigger.StartPatrol, 
    State.Patrol, 
    startPatrolGuard);
```

Using lambda expressions

```csharp
//using Paps.FSM.Extensions

guardEnemyFSM.AddGuardConditionTo(
    State.SearchTarget, 
    Trigger.StartPatrol, 
    State.Patrol, 
    () => !HasTarget());
```

### Event Dispatching

The FSM class implements the IFSMEventSender. You can send events to state objects that implement IStateEventReceiver interface.
The type of event you send must match the type of IStateEventReceiver generic.

```csharp
IStateEventReceiver<string> patrolState = new PatrolState();

guardEnemyFSM.AddState(State.Patrol, patrolState);

guardEnemyFSM.SendEvent<string>("MyEvent");
```

### Useful Extensions

Use this extensions by adding "using Paps.FSM.Extensions"

Create timer states

```csharp
//Wait 2 seconds, then start patrol
guardEnemyFSM.AddTimerState(
    State.Waiting, 2000, 
    stateId => guardEnemyFSM.Trigger(Trigger.StartPatrol));
```

Create a state only using lambda expressions

```csharp
guardEnemyFSM.AddWithEvents(
    State.Patrol, 
    () => Debug.Log("Enter Patrol"), 
    () => Debug.Log("Update Patrol"), 
    () => Debug.Log("Exit Patrol"));
```

Generate transitions from any a source state to a specific target state

```csharp
//Any state can transition to Waiting. Including Waiting itself
guardEnemyFSM.FromAny(Trigger.Wait, State.Waiting);
```

Not want to have a reentrat state? Try the following

```csharp
//Any state can transition to Waiting. Except Waiting itself
guardEnemyFSM.FromAnyExceptTarget(Trigger.Wait, State.Waiting);
```

Obtain transitions related to a specific state...

```csharp
ITransition<State, Trigger>[] transitions = guardEnemyFSM.GetTransitionsRelatedTo(State.Patrol);
```

...or remove it

```csharp
guardEnemyFSM.RemoveAllTransitionsRelatedTo(State.Patrol);
```

### Including this package on your Unity Project

...