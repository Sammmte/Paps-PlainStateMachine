using NSubstitute;
using NUnit.Framework;
using Paps.StateMachines;
using Paps.StateMachines.Extensions;

namespace Tests.WithStructs
{
    public class EventDispatcherStateMachineShould
    {
        [Test]
        public void Add_And_Remove_Event_Handler_To_States()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmpty(1);

            IStateEventHandler eventHandler = Substitute.For<IStateEventHandler>();

            fsm.SubscribeEventHandlerTo(1, eventHandler);

            Assert.IsTrue(fsm.HasEventHandlerOn(1, eventHandler));

            Assert.IsTrue(fsm.UnsubscribeEventHandlerFrom(1, eventHandler));

            Assert.IsFalse(fsm.HasAnyEventHandlerOn(1));
            Assert.IsFalse(fsm.HasEventHandlerOn(1, eventHandler));
        }

        [Test]
        public void Send_Event_To_Event_Receivers()
        {
            var fsm = new PlainStateMachine<int, int>();

            IState state1 = Substitute.For<IState>();
            IState state2 = Substitute.For<IState>();

            IStateEventHandler eventHandler1 = Substitute.For<IStateEventHandler>();
            IStateEventHandler eventHandler2 = Substitute.For<IStateEventHandler>();

            IEvent stateEvent = Substitute.For<IEvent>();

            stateEvent.GetEventData().Returns(10);

            eventHandler1.HandleEvent(stateEvent).Returns(true);

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.SubscribeEventHandlerTo(1, eventHandler1);
            fsm.SubscribeEventHandlerTo(2, eventHandler2);

            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.InitialState = 1;

            fsm.Start();

            Assert.IsTrue(fsm.SendEvent(stateEvent));

            eventHandler1.Received().HandleEvent(stateEvent);

            fsm.Trigger(0);

            Assert.IsFalse(fsm.SendEvent(stateEvent));

            eventHandler2.Received().HandleEvent(stateEvent);
        }

        [Test]
        public void Send_Events_To_Event_Receivers_And_Stop_When_Any_Returns_True()
        {
            var fsm = new PlainStateMachine<int, int>();

            IState state1 = Substitute.For<IState>();

            IStateEventHandler eventHandler1 = Substitute.For<IStateEventHandler>();
            IStateEventHandler eventHandler2 = Substitute.For<IStateEventHandler>();
            IStateEventHandler eventHandler3 = Substitute.For<IStateEventHandler>();

            IEvent stateEvent = Substitute.For<IEvent>();

            stateEvent.GetEventData().Returns(10);

            eventHandler1.HandleEvent(stateEvent).Returns(false);
            eventHandler2.HandleEvent(stateEvent).Returns(true);
            eventHandler3.HandleEvent(stateEvent).Returns(false);

            fsm.AddState(1, state1);

            fsm.SubscribeEventHandlerTo(1, eventHandler1);
            fsm.SubscribeEventHandlerTo(1, eventHandler2);
            fsm.SubscribeEventHandlerTo(1, eventHandler3);

            fsm.InitialState = 1;

            fsm.Start();

            fsm.SendEvent(stateEvent);

            eventHandler1.Received(1).HandleEvent(stateEvent);
            eventHandler2.Received(1).HandleEvent(stateEvent);
            eventHandler3.DidNotReceive().HandleEvent(stateEvent);
        }

        [Test]
        public void Return_Event_Handlers_Of_A_Specific_State()
        {
            var fsm = new PlainStateMachine<int, int>();

            var stateId1 = 1;
            var stateId2 = 2;

            var stateObj = Substitute.For<IState>();

            var eventHandler1 = Substitute.For<IStateEventHandler>();
            var eventHandler2 = Substitute.For<IStateEventHandler>();
            var eventHandler3 = Substitute.For<IStateEventHandler>();

            fsm.AddState(stateId1, stateObj);
            fsm.AddState(stateId2, stateObj);

            fsm.SubscribeEventHandlerTo(stateId1, eventHandler1);
            fsm.SubscribeEventHandlerTo(stateId1, eventHandler2);

            fsm.SubscribeEventHandlerTo(stateId2, eventHandler3);

            var eventHandlers = fsm.GetEventHandlersOf(stateId1);

            Assert.Contains(eventHandler1, eventHandlers);
            Assert.Contains(eventHandler2, eventHandlers);
            AssertExtensions.DoesNotContains(eventHandler3, eventHandlers);
        }
    }
}