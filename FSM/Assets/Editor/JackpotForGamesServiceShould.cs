using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.Threading.Tasks;

namespace Tests
{
    public interface IJackpotClient
    {
        void DisplayInfo();

        string GetCurrentGame();
    }

    public class JackpotForGamesService
    {
        public IJackpotClient JackpotClient { get; }

        public JackpotForGamesService(IJackpotClient jackpotClient)
        {
            JackpotClient = jackpotClient;
        }

        public void DisplayJackpotClientInfo()
        {
            JackpotClient.DisplayInfo();
        }

        public void DisplayCurrentGame()
        {
            string currentGame = JackpotClient.GetCurrentGame();

            if(currentGame == "Mexicana2")
            {
                throw new InvalidOperationException("Mexicana2 no va loco");
            }
            else
            {
                Debug.Log(currentGame + " es el mejor juego");
            }
        }
    }

    public interface IJackpotForGamesService
    {
        event Action cuandoPasaAlgo;

        Task SavePlayInfo(string info);

        string GetPlayInfo();
    }

    public class JackpotForGamesServiceShould
    {
        public void Prueba()
        {
            var service = Substitute.For<IJackpotForGamesService>();

            service.SavePlayInfo(Arg.Any<string>()).Returns(Task.Delay(300));
        }

        public void PruebaEventos()
        {
            var service = Substitute.For<IJackpotForGamesService>();

            service.cuandoPasaAlgo += delegate () { Debug.Log("HOLA"); };

            service.cuandoPasaAlgo += Raise.Event<Action>();

            var miDelegate = Substitute.For<Action>();
        }

        [Test]
        public void DisplayJackpotClientInfo()
        {
            var jackpotClient = Substitute.For<IJackpotClient>();

            var jackpotForGamesService = new JackpotForGamesService(jackpotClient);

            jackpotForGamesService.DisplayJackpotClientInfo();

            jackpotClient.Received().DisplayInfo();
        }

        [Test]
        public void ThrowAnExceptionIfCurrentGameIsMexicana2()
        {
            var jackpotClient = Substitute.For<IJackpotClient>();

            var jackpotForGamesService = new JackpotForGamesService(jackpotClient);

            jackpotClient.GetCurrentGame().Returns("Mexicana2");

            Assert.Throws<InvalidOperationException>(jackpotForGamesService.DisplayCurrentGame);
        }

        [Test]
        public void DisplayCurrentGameIfItIsNotMexicana2()
        {
            var jackpotClient = Substitute.For<IJackpotClient>();

            var jackpotForGamesService = new JackpotForGamesService(jackpotClient);

            jackpotClient.GetCurrentGame().Returns("Mexicana");

            jackpotForGamesService.DisplayCurrentGame();
        }
    }
}
