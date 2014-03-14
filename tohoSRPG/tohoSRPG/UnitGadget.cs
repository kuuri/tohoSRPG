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
        public bool drive;
        public bool dead;

        public int level;
        public int HPmax;
        public int HP;
        public int HPold;
        public int SPmax;
        public int SP;
        public int DPmax;
        public int DP;
        public int AP;

        public int crystalErosion;

        public int[] actCount;

        public Condition<SymptonPlus> symptonPlus;
        public Condition<SymptonMinus> symptonMinus;
        public Condition<Trap> trap;
        public int stance;

        public struct UpParameter
        {
            public int search;
            public int hide;
            public int speed;
            public int close;
            public int far;

            public void AddSearch(int value)
            {
                search += value;
                if (search > 128)
                    search = 128;
                else if (search < 0)
                    search = 0;
            }

            public void AddHide(int value)
            {
                hide += value;
                if (hide > 128)
                    hide = 128;
                else if (hide < 0)
                    hide = 0;
            }

            public void AddSpeed(int value)
            {
                speed += value;
                if (speed > 128)
                    speed = 128;
            }

            public void AddClose(int value)
            {
                close += value;
                if (close > 128)
                    close = 128;
            }

            public void AddFar(int value)
            {
                far += value;
                if (far > 128)
                    far = 128;
            }
        }
        public UpParameter upParameter;

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
            drive = false;
            dead = false;

            level = unit.level;
            HP = HPmax = HPold = unit.pHP;
            SPmax = 160;
            SP = 160;
            DPmax = 160;
            DP = 0;
            crystalErosion = 0;

            actCount = new int[unit.acts.Length];
            for (int i = 0; i < unit.acts.Length; i++)
                actCount[i] = unit.acts[i].count;

            symptonPlus = new Condition<SymptonPlus>(SymptonPlus.None, 0, 0);
            symptonMinus = new Condition<SymptonMinus>(SymptonMinus.None, 0, 0);
            trap = new Condition<Trap>(Trap.None, 0, 0);
            stance = -1;

            upParameter = new UpParameter();

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

        public bool IsHaveAbility(Ability a)
        {
            return unit.IsHaveAbility(a);
        }

        public bool IsMinusSymptonInvalid(SymptonMinus symp)
        {
            if (symp == SymptonMinus.Stigmata)
                return IsPlusSymptonInvalid(SymptonPlus.Stigmata);

            if (symptonMinus.sympton == SymptonMinus.Invalid)
                return true;

            return false;
        }

        public bool IsPlusSymptonInvalid(SymptonPlus symp)
        {
            if (symptonPlus.sympton == SymptonPlus.Invalid)
                return true;

            return false;
        }

        public bool IsCrystalEffectInvalid(CrystalEffect effect)
        {
            return false;
        }

        public Unit.Parameter Parameter
        {
            get
            {
                return (drive ? unit.drivePar : unit.normalPar) + upParameter;
            }
        }

        public bool Dedodge
        {
            get { return dedodge || symptonMinus.sympton == SymptonMinus.Dedodge; }
        }

        public bool Deguard
        {
            get { return deguard || symptonMinus.sympton == SymptonMinus.Deguard; }
        }

        public Act StanceAct
        {
            get { return stance >= 0 ? unit.acts[stance] : null; }
        }
    }

    struct Condition<T>
    {
        public T sympton;
        public int turn;
        public int power;
        public UnitGadget doer;

        public Condition(T sympton, int turn, int power, UnitGadget doer = null)
        {
            this.sympton = sympton;
            this.turn = turn;
            this.power = power;
            this.doer = doer;
        }
    }
}
