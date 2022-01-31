using AdventOfCode2020.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
#if DEBUG
using System.Diagnostics;
#endif

namespace AdventOfCode2020.Solutions
{
    public class Day22 : IPuzzle
    {
        public string CalculateSolution(Parts part, string inputData)
        {
            var players = ParseInputData(inputData);

            switch (part)
            {
                case Parts.Part1:
                    return PlayCombatWithCrab(players[0], players[1]).ToString();

                case Parts.Part2:
                    return PlayRecursiveCombatWithCrab(players[0], players[1]).ToString();

                default:
                    throw new ApplicationException($"Invalid parameter {nameof(part)} value ({part})");
            }
        }

        private static int PlayCombatWithCrab(Deck player1, Deck player2)
        {
            while (player1.HasCards && player2.HasCards)
            {
                var card1 = player1.PlayCard();
                var card2 = player2.PlayCard();
                if (card1 > card2)
                {
                    player1.AddWonCards(card1, card2);
                }
                else if (card2 > card1)
                {
                    player2.AddWonCards(card2, card1);
                }
                else
                {
                    throw new InvalidOperationException("It is a DRAW! Unexpected! Unacceptable!!!");
                }
            }

            var winningScore = player1.HasCards ? player1.CalcScore() : player2.CalcScore();

            return winningScore;
        }

        private static int PlayRecursiveCombatWithCrab(Deck player1, Deck player2)
        {
            var winner = PlayRecursiveCombat(player1, player2, 1);

            var winningScore = winner switch
            {
                1 => player1.CalcScore(),
                2 => player2.CalcScore(),
                _ => throw new InvalidOperationException("It is a DRAW! Unexpected! Unacceptable!!!")
            };

            return winningScore;
        }

        private static int PlayRecursiveCombat(Deck player1, Deck player2, int game)
        {
            var configCache = new HashSet<string>();

#if DEBUG
            Debug.WriteLine($"=== Game {game} ===");
#endif
            var round = 0;

            while (player1.HasCards && player2.HasCards)
            {
                round++;

#if DEBUG
                Debug.WriteLine($"-- Round {round} (Game {game}) --");
                Debug.WriteLine($"Player 1's deck: {player1}");
                Debug.WriteLine($"Player 2's deck: {player2}");
#endif
                var configuration = player1.ToString();// + "|" + player2.ToString();
                if (configCache.Contains(configuration))
                {
#if DEBUG
                    Debug.WriteLine($"THIS ALREADY HAPPENED!!!");
#endif
                    return 1;
                }
                configCache.Add(configuration);

                var card1 = player1.PlayCard();
                var card2 = player2.PlayCard();

#if DEBUG
                Debug.WriteLine($"Player 1 plays: {card1}");
                Debug.WriteLine($"Player 2 plays: {card2}");
#endif

                if (player1.CanPlayRecursively(card1) && player2.CanPlayRecursively(card2))
                {
                    var winner = PlayRecursiveCombat(player1.CopyDeck(card1), player2.CopyDeck(card2), game+1);
                    switch (winner)
                    {
                        case 1:
#if DEBUG
                            Debug.WriteLine($"The winner of game {game} is player 1!\n");
#endif
                            player1.AddWonCards(card1, card2);
                            break;
                        case 2:
#if DEBUG
                            Debug.WriteLine($"The winner of game {game} is player 2!\n");
#endif
                            player2.AddWonCards(card2, card1);
                            break;
                        default:
                            throw new InvalidOperationException("It is a DRAW! Unexpected! Unacceptable!!!");
                    }
                }
                else
                {
                    if (card1 > card2)
                    {
#if DEBUG
                        Debug.WriteLine($"Player 1 wins round {round} of game {game}!");
#endif
                        player1.AddWonCards(card1, card2);
                    }
                    else if (card2 > card1)
                    {
#if DEBUG
                        Debug.WriteLine($"Player 2 wins round {round} of game {game}!");
#endif
                        player2.AddWonCards(card2, card1);
                    }
                    else
                    {
                        throw new InvalidOperationException("It is a DRAW! Unexpected! Unacceptable!!!");
                    }
                }
            }

            if (player1.HasCards)
                return 1;
            if (player2.HasCards)
                return 2;

            return 0;
        }
        
        private static List<Deck> ParseInputData(string inputData)
        {
            var playerDecks = inputData.Split(Environment.NewLine + Environment.NewLine)
                .Select(player => player.Split(Environment.NewLine).Skip(1).Select(int.Parse)).ToArray()
                .Select(cards => new Deck(cards));
            return playerDecks.ToList();
        }

        private class Deck
        {
            private readonly Queue<int> _cards;

            public Deck(IEnumerable<int> cards)
            {
                _cards = new Queue<int>(cards);
            }

            public bool HasCards => _cards.Count > 0;

            public int PlayCard()
            {
                return _cards.Dequeue();
            }

            public bool CanPlayRecursively(int numberOfCardsNeeded)
            {
                return _cards.Count >= numberOfCardsNeeded;
            }

            public void AddWonCards(int winningCard, int lostCard)
            {
                _cards.Enqueue(winningCard);
                _cards.Enqueue(lostCard);
            }

            public int CalcScore()
            {
                var score = 0;
                while (_cards.Count > 0)
                {
                    score += _cards.Count * _cards.Dequeue();
                }
                return score;
            }

            public Deck CopyDeck(int cardsCount)
            {
                return new Deck(_cards.Take(cardsCount).ToArray());
            }

            public override string ToString()
            {
                return string.Join(",", _cards.ToArray());
            }
        }
    }

}
