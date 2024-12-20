﻿using TMPro;
using UnityEngine.Events;

namespace remake.GameFlow
{
    public class PrepareState : IGameState
    {
        private static readonly int max_turn = 10;
        private readonly GM gm;

        private readonly Player[] players;

        private readonly TMP_Text turnText;

        public PrepareState(GM gm, Player[] players, TMP_Text turnText)
        {
            this.gm = gm;
            this.players = players;
            this.turnText = turnText;
        }

        public void OnExit()
        {
        }

        public void Update()
        {
            if (gm.turn > max_turn && !IsDuce())
            {
                gm.ToRoundStartState();
                gm.SetRoundText("no one win this round\n Press R to next round");
                // roundText.text = "no one win this round\n Press R to next round";
            }
            else if (gm.prepareOk)
            {
                gm.ToBeforeBattleState();
            }
        }

        public void OnPressR()
        {
        }

        public void OnEnter()
        {
            ResetShootCount();
            turnText.enabled = true;
            gm.turn++;
            if (gm.turn > max_turn && !IsDuce())
            {
                // do nothing, wait update do 
            }
            else
            {
                players[0].ChangeAttackState();
                players[1].ChangeAttackState();
                players[0].ResetThrowCount();
                players[1].ResetThrowCount();
                gm.ClearPreparedPlayer();
                players[0].Prepare(OnPrepareDone(players[0], gm));
                players[1].Prepare(OnPrepareDone(players[1], gm));
                gm.prepareOk = false;
            }
        }

        private void ResetShootCount()
        {
            gm.shootCount = 0;
        }

        private bool IsDuce()
        {
            return gm.round > 3;
        }

        private static UnityAction OnPrepareDone(Player player, GM gmAtStatic)
        {
            return
                () =>
                {
                    gmAtStatic.AddPreparedPlayer(player);
                    if (!gmAtStatic.AllPlayerPrepared()) return;
                    gmAtStatic.prepareOk = true;
                };
        }
    }
}