using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace tohoSRPG
{
    static class WorldScene
    {
        static GraphicsDevice graphics;
        static SpriteBatch spriteBatch;
        static ContentManager content;
        static SpriteFont font;

        static List<EventNode> nodes;
        static int select;

        public static void Init(GraphicsDevice g, SpriteBatch s, ContentManager c)
        {
            graphics = g;
            spriteBatch = s;
            content = c;
            font = content.Load<SpriteFont>("font\\CommonFont");
            select = 0;

            nodes = new List<EventNode>();
        }

        public static void Update(GameTime gameTime)
        {
            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
            {
                select--;
                if (select < 0)
                    select = nodes.Count - 1;
            }
            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
            {
                select++;
                if (select >= nodes.Count)
                    select = 0;
            }

            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
            {
                ConfrontScene.State = 0;
                List<Unit> lu = new List<Unit>();
                lu.Add(UnitSetting.SetUnit(CharaID.Reimu, 10));
                //lu.Add(UnitSetting.SetUnit(CharaID.Zero, 10));
                //lu.Add(UnitSetting.SetUnit(CharaID.Zero, 10));
                //lu.Add(UnitSetting.SetUnit(CharaID.Zero, 10));
                //lu.Add(UnitSetting.SetUnit(CharaID.Zero, 10));
                ConfrontScene.SetScene("ゼロスーサイド", lu, "戦闘前セリフ", "", "勝利時セリフ", "", "敗北時セリフ", "");
                ConfrontScene.SetMapData(Location.Shinrabansho);
                GameBody.ChangeScene(Scene.Confront);
            }
            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Pause))
            {
                GameBody.ChangeScene(Scene.Menu);
            }
        }

        public static void Draw(GameTime gameTime)
        {
            Texture2D tw = new Texture2D(graphics, 1, 1);
            Color[] c = { Color.White };
            tw.SetData(c);
            string str;

            spriteBatch.Begin();

            Vector2 pos = new Vector2(500, 50);
            Helper.DrawWindow(new Rectangle((int)pos.X, (int)pos.Y, 200, 29 + 32 * nodes.Count));
            spriteBatch.Draw(tw, new Rectangle((int)pos.X + 20, (int)pos.Y + 28 + 32 * select, 10, 10), Color.Black);
            for (int i = 0; i < nodes.Count; i++)
            {
                spriteBatch.DrawString(font, nodes[i].name, pos + new Vector2(40, 15 + 32 * i), Color.Black);
            }

            Helper.DrawWindowBottom1("Aで決定。Startでメニューを開く");

            spriteBatch.End();
        }

        public static void NodeClear()
        {
            nodes.Clear();
        }

        public static void AddNode(EventNode node)
        {
            nodes.Add(node);
        }
    }

    public struct EventNode
    {
        public string name;
        public Vector2 position;
        public int eventNo;

        public EventNode(string name, Vector2 pos, int no)
        {
            this.name = name;
            position = pos;
            eventNo = no;
        }
    }
}
