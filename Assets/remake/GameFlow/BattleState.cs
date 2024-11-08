using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;

namespace remake.GameFlow
{
    public class BattleState : IGameState
    {
        private readonly GM gm;
        private readonly rPlayer[] players;
        private readonly TMP_Text roundText;

        private readonly HashSet<rPlayer> turnEndPlayer = new();

        public BattleState(GM gm, rPlayer[] players, TMP_Text roundText)
        {
            this.gm = gm;
            this.players = players;
            this.roundText = roundText;
        }


        public void OnExit()
        {
        }

        public void Update()
        {
        }

        public void OnPressR()
        {
        }

        public void OnEnter()
        {
            turnEndPlayer.Clear();
            players[0].Turn(OnOneShootEnd(players[0]));
            players[1].Turn(OnOneShootEnd(players[1]));
        }

        private UnityAction OnOneShootEnd(rPlayer player)
        {
            return () =>
            {
                turnEndPlayer.Add(player);
                if (turnEndPlayer.Count < 2) return;
                gm.shootCount++;
                players[0].UsedDice();
                players[1].UsedDice();
                if (players[0].Hp <= 0)
                {
                    players[1].score++;
                    if (players[1].score >= 2)
                    {
                        gm.winner = players[1];
                        gm.ToEndState();
                    }
                    else
                    {
                        gm.ToRoundStartState();
                        roundText.text = "Player 2 win this round\n Press R to next";
                    }
                    // next turn
                }
                else if (players[1].Hp <= 0)
                {
                    players[0].score++;
                    if (players[0].score >= 2)
                    {
                        gm.winner = players[0];
                        gm.ToEndState();
                    }
                    else
                    {
                        gm.ToRoundStartState();
                        roundText.text = "Player 1 win this round\n Press R to next";
                    }
                    // next turn
                }
                else if (gm.shootCount >= rPlayer.maxDiceCount)
                {
                    // sus
                    gm.ToPrepareState();
                }
                else
                {
                    gm.ToBeforeBattleState();
                }
            };
        }
    }
}