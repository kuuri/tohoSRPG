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

        public UnitType type;
        public UnitType type2;

        public int level;
        public int pHP;

        public struct Parameter
        {
            public int speed;
            public int avoid;
            public int defense;
            public int close;
            public int far;

            public static Parameter operator +(Parameter value1, UnitGadget.UpParameter value2)
            {
                Parameter result = value1;
                result.speed += value2.speed;
                result.close += value2.close;
                result.far += value2.far;

                return result;
            }
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

        public void DrawBattle(SpriteBatch sb, Vector2 pos, Color color, bool reverse)
        {
            if (!reverse)
                sb.Draw(t_battle, pos, null, color, 0, t_battle_origin, 1, SpriteEffects.None, 0);
            else
                sb.Draw(t_battle, pos, null, color, 0, new Vector2(t_battle.Width - t_battle_origin.X, t_battle_origin.Y), 1, SpriteEffects.FlipHorizontally, 0);
        }
        
        public void DrawBattle(SpriteBatch sb, Vector2 pos, Color color, float scale, bool reverse)
        {
            Rectangle rect = new Rectangle((int)pos.X, (int)(pos.Y),
                t_battle.Width, (int)(scale * t_battle.Height));

            if (!reverse)
                sb.Draw(t_battle, rect, null, color, 0, t_battle_origin, SpriteEffects.None, 0);
            else
                sb.Draw(t_battle, rect, null, color, 0, new Vector2(t_battle.Width - t_battle_origin.X, t_battle_origin.Y), SpriteEffects.FlipHorizontally, 0);
        }

        public int GetAP(bool drive)
        {
            int ap = 16;
            if (!drive)
                ap += normalPar.speed / 10 + level / 5;
            else
                ap += drivePar.speed / 10 + level / 5;
            foreach (Act act in acts)
                if (act != null && (act.type == ActType.Charge))
                    ap += act.power;
            return ap;
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
        public int success;
        public int power;
        public int count;
        public int ap;
        public int sp;
        public ActTarget target;
        public int rangeMin;
        public int rangeMax;

        public int fact;

        public Act()
        {
            lastSpell = false;
            ability1 = ActAbility.None;
            ability2 = ActAbility.None;
            sympton = 0;
            count = 0;
            ap = 12;
            sp = 0;
        }

        public bool IsHaveAbility(ActAbility ability)
        {
            return ability1 == ability || ability2 == ability;
        }

        /// <summary>
        /// 貫通効果
        /// </summary>
        public bool IsPenetrate
        {
            get
            {
                return IsHaveAbility(ActAbility.Penetrate) || IsHaveAbility(ActAbility.Thunder)
                    || IsHaveAbility(ActAbility.Assassin) || IsHaveAbility(ActAbility.Whole) || IsHaveAbility(ActAbility.Revenge)
                    || IsHaveAbility(ActAbility.Sacrifice) || IsHaveAbility(ActAbility.Destroy) || IsHaveAbility(ActAbility.Sanctio);
            }
        }

        /// <summary>
        /// 上限解除効果
        /// </summary>
        public bool IsBreakthrough
        {
            get
            {
                return IsHaveAbility(ActAbility.Rush) || IsHaveAbility(ActAbility.Sacrifice)
                    || IsHaveAbility(ActAbility.Destroy) || IsHaveAbility(ActAbility.Sanctio)
                    || IsHaveAbility(ActAbility.Thunder);
            }
        }

        public int TypeInt
        {
            get { return (int)type; }
        }

        public bool IsLimited
        {
            get { return count > 0; }
        }

        public bool IsSpell
        {
            get { return sp > 0; }
        }

        public bool IsPassive
        {
            get { return target == ActTarget.Equip; }
        }

        public bool IsCover
        {
            get { return type == ActType.Guard || type == ActType.LessGuard || type == ActType.Utsusemi; }
        }

        public bool IsStance
        {
            get { return type == ActType.Counter || type == ActType.BarrierDefense || type == ActType.BarrierSpirit; }
        }

        public bool IsActiveDefense
        {
            get { return type == ActType.Guard || type == ActType.LessGuard || type == ActType.Counter || type == ActType.BarrierDefense || type == ActType.BarrierSpirit; }
        }

        public bool IsActiveDodge
        {
            get { return type == ActType.Utsusemi; }
        }

        public bool IsTargetAll
        {
            get { return target == ActTarget.AllyAll || target == ActTarget.EnemyAll || target == ActTarget.All; }
        }
    }
}
