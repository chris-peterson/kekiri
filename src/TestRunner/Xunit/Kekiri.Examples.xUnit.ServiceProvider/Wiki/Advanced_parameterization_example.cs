// Supports https://github.com/chris-peterson/kekiri/wiki/Parameterize-Test-Using-A-Complex-Type

// using System;
// using System.Collections.Generic;
// using Kekiri.Xunit;
// using Xunit;

// namespace Kekiri.Examples.Xunit
// {
//     public class Advanced_parameterization_example : Scenarios
//     {
//         PokerShowdown _scenario;
//         int _actualWinner;

//         [ScenarioOutline]
//         [Example(ShowdownScenarios.between_two_players_with_different_highcards)]
//         [Example(ShowdownScenarios.between_two_players_with_straight_and_flush)]
//         public void Poker_showdown(ShowdownScenarios showdown)
//         {
//             Given(() => _scenario = ShowdownScenarioFactory.Create[showdown].Invoke());
//             When(Showing_down);
//             Then(Then_the_correct_player_wins);
//         }

//         void Showing_down()
//         {
//             _actualWinner = _scenario.Game.Showdown(_scenario.Players);
//         }

//         void Then_the_correct_player_wins()
//         {
//             Assert.Equal(_scenario.Winner, _actualWinner);
//         }
//     }

//     public enum ShowdownScenarios
//     {
//         between_two_players_with_different_highcards,
//         between_two_players_with_straight_and_flush
//     }

//     static class ShowdownScenarioFactory
//     {
//         public static readonly IDictionary<ShowdownScenarios, Func<PokerShowdown>> Create = new Dictionary<ShowdownScenarios, Func<PokerShowdown>>
//         {
//             {ShowdownScenarios.between_two_players_with_different_highcards, () => new BetweenTwoPlayersWithDifferentHighCards()},
//             {ShowdownScenarios.between_two_players_with_straight_and_flush, () => new BetweenTwoPlayersWithStraightAndFlush()}
//         };

//     }

//     sealed class BetweenTwoPlayersWithStraightAndFlush : PokerShowdown
//     {
//         PokerGame PokerShowdown.Game
//         {
//             get { throw new NotImplementedException(); }
//         }

//         IEnumerable<PokerPlayer> PokerShowdown.Players
//         {
//             get { throw new NotImplementedException(); }
//         }

//         int PokerShowdown.Winner
//         {
//             get { throw new NotImplementedException(); }
//         }
//     }

//     sealed class BetweenTwoPlayersWithDifferentHighCards : PokerShowdown
//     {
//         PokerGame PokerShowdown.Game
//         {
//             get { throw new NotImplementedException(); }
//         }

//         IEnumerable<PokerPlayer> PokerShowdown.Players
//         {
//             get { throw new NotImplementedException(); }
//         }

//         int PokerShowdown.Winner
//         {
//             get { throw new NotImplementedException(); }
//         }
//     }

//     interface PokerShowdown
//     {
//         PokerGame Game { get; }
//         IEnumerable<PokerPlayer> Players { get; }
//         int Winner { get; }
//     }

//     sealed class PokerGame
//     {
//         public int Showdown(IEnumerable<PokerPlayer> players)
//         {
//             throw new NotImplementedException();
//         }
//     }

//     sealed class PokerPlayer
//     {
//         public int Id { get; private set; }
//     }
// }