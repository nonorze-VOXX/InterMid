using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

namespace remake
{
    public class SkillManager
    {
        private static readonly List<Tuple<DiceType, string>> defSkills = new();
        private static readonly List<Tuple<DiceType, string>> atkSkills = new();

        public static void ReadSkill()
        {
            // read csv
            // parse csv
            // create skill
            GenDefSkill();
            GenAtkSkill();
            Debug.Log(defSkills);
            Debug.Log(atkSkills);
        }

        private static void GenAtkSkill()
        {
            {
                var filePath = Path.Combine(
                    Application.streamingAssetsPath, "Skill", "AtkSkill.csv"
                );
                if (File.Exists(filePath))
                {
                    var content = File.ReadAllText(filePath);
                    content = content.Replace("\r", "");
                    var lines = content.Split('\n');
                    foreach (var line in lines)
                    {
                        var values = line.Split(',');
                        if (values.Length != 2) continue;
                        var type = GetDiceType(values[0]);
                        atkSkills.Add(new Tuple<DiceType, string>(type, values[1]));
                    }
                }
                else
                {
                    Debug.LogError("File not found at: " + filePath);
                }
            }
        }

        private static void GenDefSkill()
        {
            var filePath = Path.Combine(
                Application.streamingAssetsPath, "Skill", "DefSkill.csv"
            );
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                content = content.Replace("\r", "");
                var lines = content.Split('\n');
                foreach (var line in lines)
                {
                    var values = line.Split(',');
                    if (values.Length != 2) continue;
                    // foreach (var value in values)
                    // {
                    //     Debug.Log(value);
                    // }
                    var type = GetDiceType(values[0]);
                    defSkills.Add(new Tuple<DiceType, string>(type, values[1]));
                }
            }
            else
            {
                Debug.LogError("File not found at: " + filePath);
            }
        }

        private static UnityAction<rPlayer, rDice> GetAtkFun(string value, rDice dice)
        {
            #region TrueDamage

            {
                var pattern = @"(TrueDamage)";
                var match = Regex.Match(value, pattern);
                if (match.Success) dice.SetTrueDamage();
            }

            #endregion

            return
                (player, d) =>
                {
                    #region Atk

                    {
                        var pattern = @"(Atk)([+*])(\d+)";
                        var match = Regex.Match(value, pattern);
                        if (match.Success)
                        {
                            var op = match.Groups[2].Value;
                            var num = int.Parse(match.Groups[3].Value);
                            switch (op)
                            {
                                case "+":
                                    player.AddAttack(num);
                                    break;
                                case "*":
                                    player.MultiplyAttack(num);
                                    break;
                            }
                        }
                    }

                    #endregion
                };
        }

        private static UnityAction<rPlayer, rDice> GetDefFun(string value)
        {
            return
                (player, dice) =>
                {
                    #region Hp

                    {
                        var pattern = @"(Hp)([+])(\d+)";
                        var match = Regex.Match(value, pattern);

                        Debug.Log("DEf Match: " + match);
                        if (match.Success)
                        {
                            var op = match.Groups[2].Value;
                            var num = int.Parse(match.Groups[3].Value);
                            switch (op)
                            {
                                case "+":
                                    Debug.Log("Hp + " + num);
                                    player.AddHp(num);
                                    break;
                            }
                        }
                    }

                    {
                        var pattern = @"(HpFull)";
                        var match = Regex.Match(value, pattern);
                        if (match.Success) player.FullHeal();
                    }

                    #endregion
                };
        }

        private static DiceType GetDiceType(string value)
        {
            return value switch
            {
                "Single" => DiceType.Single,
                "Pair" => DiceType.Pair,
                "Triple" => DiceType.Triple,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public static void SetDiceFunc(rDice dice, rPlayer rPlayer)
        {
            var type = dice.GetDiceType();
            var isAtk = rPlayer.IsAtkState();
            if (isAtk)
                SetAtkFunc(dice, rPlayer, type);
            else
                SetDefFunc(dice, rPlayer, type);
        }

        private static void SetDefFunc(rDice dice, rPlayer player, DiceType type)
        {
            foreach (var skill in defSkills)
                if (skill.Item1 == type)
                {
                    var s = GetDefSkill(skill.Item2, dice, player);

                    if (s != null) dice.AddFunction(s);
                }
        }

        private static UnityAction<rPlayer, rDice> GetDefSkill(string skillItem2, rDice dice, rPlayer player)
        {
            return GetDefFun(skillItem2);
        }

        private static void SetAtkFunc(rDice dice, rPlayer player, DiceType type)
        {
            foreach (var skill in atkSkills)
                if (skill.Item1 == type)
                {
                    var s = GetAtkSkill(skill.Item2, dice, player);
                    dice.AddFunction(s);
                }
        }

        private static UnityAction<rPlayer, rDice> GetAtkSkill(string skillItem2, rDice dice, rPlayer player)
        {
            return GetAtkFun(skillItem2, dice);
        }
    }
}