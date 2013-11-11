using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tohoSRPG
{
    class Unit
    {
        public CharaID id;
        public string name;
        public string nickname;
        public Texture2D t_icon;
        public Texture2D t_battle;
        public Vector2 t_battle_origin;

        public Type type;
        public Type type2;

        public int level;
        public int pHP;

        public struct Parameter
        {
            public int speed;
            public int avoid;
            public int defence;
            public int close;
            public int far;
        }
        public Parameter normalPar;
        public Parameter drivePar;

        public Act[] acts;
        public Affinity[] affinity;
        public List<Ability> ability;
        public Artifact[] artifact;

        public Unit(CharaID id) 
        {
            this.id = id;
            acts = new Act[6];
            affinity = new Affinity[Enum.GetNames(typeof(Terrain)).Length - 1];
            ability = new List<Ability>();
            artifact = new Artifact[2];
        }

        public bool IsHaveAbility(Ability a)
        {
            foreach (Ability aa in ability)
                if (aa == a)
                    return true;

            return false;
        }
    }

    class Act
    {
        public string name;

        public ActType type;
        public bool lastSpell;
        public ActAbility ability1;
        public ActAbility ability2;
        public int sympton;
        public float proficiency;
        public int success;
        public int power;
        public int ap;
        public int sp;
        public ActTarget target;
        public int rangeMin;
        public int rangeMax;

        public Act()
        {
            lastSpell = false;
            ability1 = ActAbility.None;
            ability2 = ActAbility.None;
            sympton = 0;
            proficiency = 1;
            sp = 0;
        }

        public bool IsHaveAbility(ActAbility ability)
        {
            return ability1 == ability || ability2 == ability;
        }

        public int TypeInt
        {
            get { return (int)type; }
        }

        public bool IsSpell
        {
            get { return sp > 0; }
        }

        public bool IsPassive
        {
            get { return target == ActTarget.Equip; }
        }

        public bool IsTargetAll
        {
            get { return target == ActTarget.AllyAll || target == ActTarget.EnemyAll; }
        }
    }
}
