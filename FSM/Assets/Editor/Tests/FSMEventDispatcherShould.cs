using NSubstitute;
using NUnit.Framework;
using Paps.FSM;
using Paps.FSM.Extensions;

namespace Tests
{
    public class FSMEventDispatcherShould
    {
        [Test]
        public void AddAndRemoveEventHandlerToStates()
        {
            var fsm = new FSM<int, int>();

            fsm.AddEmpty(1);

            IStateEventHandler eventHandler = Substitute.For<IStateEventHandler>();

            fsm.SubscribeEventHandlerTo(1, eventHandler);

            Assert.IsTrue(fsm.HasEventHandler(1, eventHandler));

            fsm.UnsubscribeEventHandlerFrom(1, eventHandler);

            Assert.IsFalse(fsm.HasEventListener(1));
            Assert.IsFalse(fsm.HasEventHandler(1, eventHandler));
        }

        [Test]
        public void SendEventToEventReceivers()
        {
            var fsm = new FSM<int, int>();

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
        public void SendEventsToEventReceiversAndStopWhenAnyReturnsTrue()
        {
            var fsm = new FSM<int, int>();

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
    }
}