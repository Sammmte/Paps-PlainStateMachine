# Paps Plain State Machine

A simple plain state machine made in c#. Developed in a Unity 3D Project.

```csharp
//Create your state machine using any type you want for state and trigger identifiers
//using Paps.StateMachines

var guardEnemyFSM = new PlainStateMachine<State, Trigger>();

//State objects must implement IState interface

IState patrolState = new PatrolState();
IState searchTarget = new SearchTargetState();

guardEnemyFSM.AddState(State.Patrol, patrolState);
guardEnemyFSM.AddState(State.SearchTarget, searchTarget);

//You must specify a initial state before start using the state machine
//otherwise an exception will be thrown

guardEnemyFSM.InitialState = State.Patrol;

//Define transitions between states.

var transition1 = new Transition<State, Trigger>(
                        State.Patrol, 
                        Trigger.PatrolFinished, 
                        State.SearchTarget);

var transition2 = new Transition<State, Trigger>(
                        State.SearchTarget, 
                        Trigger.StartPatrol, 
                        State.Patrol);

guardEnemyFSM.AddTransition(transition1);
guardEnemyFSM.AddTransition(transition2);

//Start your state machine

guardEnemyFSM.Start();

//guardEnemyFSM.CurrentState == State.Patrol

guardEnemyFSM.Trigger(Trigger.PatrolFinished);

//guardEnemyFSM.CurrentState == State.SearchTarget
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

IState.Update() function gets called on the current state when you call the PlainStateMachine.Update() function of your state machine object.

### Guard Conditions

Add guard conditions to your transitions to wisely choose the next state. e.g.:

```csharp
//You can create your custom guard condition classes implementing 
//the IGuardCondition interface
IGuardCondition startPatrolGuard = new MyCustomGuardCondition();

guardEnemyFSM.AddGuardConditionTo(transition, startPatrolGuard);
```

Using lambda expressions

```csharp
//using Paps.StateMachines.Extensions

guardEnemyFSM.AddGuardConditionTo(transition, () => !HasTarget());
```

### Event Dispatching

You can send events to state objects. You need add IStateEventHandler objects to states.

```csharp
IState patrolState = new PatrolState();

guardEnemyFSM.AddState(State.Patrol, patrolState);

guardEnemyFSM.SubscribeEventHandlerTo(State.Patrol, 
    someEvent => 
    {
        Event<string> myEvent = (Event<string>)someEvent;
        Debug.Log("Handling event " + myEvent.GetEventData())
    });

IEvent myEvent = new Event<string>("MyEvent");

guardEnemyFSM.SendEvent(myEvent);

//Output: Handling event MyEvent
```

Event handlers returns a boolean on its HandleEvent function. If you have more than one event handler, the event instance will be sent to each one until one returns "true" or all returns "false".

### Useful Extensions

Use this extensions by adding "using Paps.StateMachines.Extensions"

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
Transition<State, Trigger>[] transitions = guardEnemyFSM.GetTransitionsRelatedTo(State.Patrol);
```

...or remove it

```csharp
guardEnemyFSM.RemoveAllTransitionsRelatedTo(State.Patrol);
```

### Including this package on your Unity Project

Open the "Packages" folder inside your project folder. Open the manifest.json, you should have something like this:

```json
{
   "dependencies": {
        "com.unity.some-package" : "1.0.0",
        "com.unity.some-other-package" : "1.0.0"
        //other packages...
    }
}
```

This "dependencies" json object represents the package key values (key: package name, value: package version) of all packages your project depends on.

First you need to add a "scopedRegistries" json object with the following data:

```json
"scopedRegistries": [
    {
      "name": "Any name should fit",
      "url": "https://registry.npmjs.org/",
      "scopes": [
        "paps"
      ]
    }
  ]
```

The "url" field represents the url where Paps Plain State Machine packages is hosted.

the "scopes" field represents the permitted package "namespaces". For example if you want to use two packages with names "pepe.awesome-feature" and "pepe.fantastic-feature", you could write both names in the "scopes" object, or simply write "pepe".

For more information about adding custom packages to your unity project, [read unity's documentation here](https://docs.unity3d.com/Manual/CustomPackages.html). Aaaand [here](https://docs.unity3d.com/Manual/upm-manifestPkg.html)

Finally you must add this package dependency to your project in the "dependencies" object:

```json
{
    "dependencies": {
        "com.unity.some-package" : "1.0.0",
        "com.unity.some-other-package" : "1.0.0",
        "paps.plain-state-machine" : "0.0.1-unity" //or the version you want
      }
}
```

Your manifest.json file should look like this:

```json
{
  "scopedRegistries": [
    {
      "name": "Any name should fit",
      "url": "https://registry.npmjs.org/",
      "scopes": [
        "paps"
      ]
    }
  ],

  "dependencies": {
    "com.unity.some-package" : "1.0.0",
    "com.unity.some-other-package" : "1.0.0",
    "paps.plain-state-machine": "0.0.1-unity" //or the version you want
  }
}
```