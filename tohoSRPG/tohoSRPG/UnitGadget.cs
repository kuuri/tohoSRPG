using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tohoSRPG
{
    class UnitGadget
    {
        public CharaID id;
        public FrendOfFoe ff;
        public bool leader;

        public Unit unit;
        public Positon postion;
        public int actPage;
        public bool drive;

        public int level;
        public int HPmax;
        public int HP;
        public int SPmax;
        public int SP;
        public int AP;

        public Condition<SymptonPlus> symptonPlus;
        public Condition<SymptonMinus> symptonMinus;
        public Condition<Trap> trap;
        public int stance;

        public bool dedodge;
        public bool deguard;

        public float spChargeFact;

        public UnitGadget(FrendOfFoe ff, Unit unit, Positon pos, bool leader = false)
        {
            id = unit.id;
            this.ff = ff;
            this.leader = leader;
            this.unit = unit;

            postion = pos;
            actPage = 0;
            drive = false;

            level = unit.level;
            HP = HPmax = unit.pHP;
            SPmax = 160;
            SP = 0;

            symptonPlus = new Condition<SymptonPlus>(SymptonPlus.None, 0, 0);
            symptonMinus = new Condition<SymptonMinus>(SymptonMinus.None, 0, 0);
            trap = new Condition<Trap>(Trap.None, 0, 0);
            stance = -1;

            dedodge = false;
            deguard = false;

            spChargeFact = 0;
        }

        public int GetAP()
        {
            int ap = 16 + Parameter.speed / 10 + level / 5;
            foreach (Act act in unit.acts)
                if (act != null && (act.type == ActType.Charge))
                    ap += act.power;
            return ap;
        }

        public bool IsType(Type type)
        {
            return unit.type == type || (drive ? unit.type2 == type : false);
        }

        public Unit.Parameter Parameter
        {
            get { return drive ? unit.drivePar : unit.normalPar; }
        }
    }

    struct Condition<T>
    {
        public T sympton;
        public int turn;
        public int power;

        public Condition(T sympton, int turn, int power)
        {
            this.sympton = sympton;
            this.turn = turn;
            this.power = power;
        }
    }
}
