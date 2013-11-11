using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using BT = tohoSRPG.BattleScene;
using FF = tohoSRPG.FrendOfFoe;

namespace tohoSRPG
{
    static class BattleSetting
    {
        static ContentManager content;

        public static void Init(ContentManager c)
        {
            content = c;
        }

        public static void BattleSet001()
        {
            string[] map = {
                          "PPPPPPPPP",
                          "PPFFFFFPP",
                          "PFFFFFFFP",
                          "PFFFFFFFP",
                          "PFFFIFFFP",
                          "PFFFFFFFP",
                          "PFFFFFFFP",
                          "PPFFFFFPP",
                          "PPPPPPPPP"};

            string[] crystal = GetNothingCrystalEffect();
            BT.t_map = content.Load<Texture2D>("img\\maptip\\map");
            BT.MapSet(map, crystal);

            BT.t_bg = content.Load<Texture2D>("img\\bg\\bg001");

            BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, BT.allyUnit[0], new Positon(2, 4), true));
            BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, BT.allyUnit[1], new Positon(2, 2)));
            BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, BT.allyUnit[3], new Positon(2, 6)));
            BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, BT.allyUnit[2], new Positon(1, 3)));
            BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, BT.allyUnit[4], new Positon(1, 5)));

            BT.enemyUnitGadget.Add(new UnitGadget(FF.Enemy, UnitSetting.SetUnit(CharaID.Zero, 30), new Positon(6, 4), true));

            BT.allUnitGadget = new List<UnitGadget>(BT.allyUnitGadget);
            BT.allUnitGadget.AddRange(BT.enemyUnitGadget);
        }

        static string[] GetNothingCrystalEffect()
        {
            string[] effect = new string[BattleScene.MapSize];
            string ef = "";
            for (int i = 0; i < BattleScene.MapSize; i++)
                ef += "N";
            for (int i = 0; i < BattleScene.MapSize; i++)
                effect[i] = ef;
            return effect;
        }
    }
}
