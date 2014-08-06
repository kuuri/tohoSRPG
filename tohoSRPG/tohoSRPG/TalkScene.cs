using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.VisualBasic.FileIO;

namespace tohoSRPG
{
    static class TalkScene
    {
        static GraphicsDevice graphics;
        static SpriteBatch spriteBatch;
        static ContentManager content;
        static SpriteFont font;

        static TextFieldParser parser;
        static FileStream fs;
        static ICryptoTransform decryptor;
        static CryptoStream cs;

        enum State
        {
            Free,
            Talk,
            Select,
            Message,
        }
        static State state;
        static bool waitall;
        static TimeSpan sleepTime;

        static Texture2D t_stage;
        static Dictionary<string, TalkChara> charas;
        static string talking;
        static string message;
        static Rectangle bln_rect;
        static float bln_dir;
        static Texture2D t_balloon;
        static Texture2D t_balloon2;

        public static void Init(GraphicsDevice g, SpriteBatch s, ContentManager c)
        {
            graphics = g;
            spriteBatch = s;
            content = c;
            font = content.Load<SpriteFont>("font\\CommonFont");

            Random rand = new Random(13562538);
            AesManaged aes = new AesManaged();
            byte[] b_key = new byte[32];
            byte[] b_iv = new byte[16];
            rand.NextBytes(b_key);
            rand.NextBytes(b_iv);
            aes.Key = b_key;
            aes.IV = b_iv;
            decryptor = aes.CreateDecryptor();

            state = State.Free;
            waitall = false;
            sleepTime = TimeSpan.Zero;

            charas = new Dictionary<string, TalkChara>();
            t_balloon = content.Load<Texture2D>("img\\face\\balloon");
            t_balloon2 = content.Load<Texture2D>("img\\face\\balloon2");
        }

        public static void Update(GameTime gameTime)
        {
            sleepTime -= gameTime.ElapsedGameTime;
            if (waitall)
            {
                bool acting = false;
                foreach (TalkChara tc in charas.Values)
                    if (tc.Acting) acting = true;
                if (!acting)
                    waitall = false;
            }

            if (state == State.Free)
            {
                if (parser != null && !waitall && sleepTime.TotalMilliseconds <= 0)
                {
                    string[] row = parser.ReadFields();
                    switch (row[0])
                    {
                        case "Stage":
                            t_stage = content.Load<Texture2D>("img\\bg\\" + row[1]);
                            break;
                        case "StageName":
                            break;
                        case "Chara":
                            charas.Add(row[1], new TalkChara(content.Load<Texture2D>("img\\face\\" + row[1]), float.Parse(row[2]), float.Parse(row[3]), bool.Parse(row[4])));
                            break;
                        case "Move":
                            charas[row[1]].Move(float.Parse(row[2]), float.Parse(row[3]), double.Parse(row[4]));
                            break;
                        case "Speak":
                            #region
                            state = State.Talk;
                            talking = row[1];
                            message = "";
                            while (!parser.EndOfData)
                            {
                                message += parser.ReadLine();
                                if (message.IndexOf(";") >= 0)
                                {
                                    message = message.Replace(";", "");
                                    break;
                                }
                                message += "\n";
                            }
                            float[] score = new float[8];
                            Rectangle[] rect = new Rectangle[8];
                            float[] dir = new float[8];
                            Vector2 msize = font.MeasureString(message);
                            float a = 1.41421356237f * (msize.X / 2 + 10);
                            float b = 1.41421356237f * (msize.Y / 2 + 10);
                            Vector2 p = charas[talking].position;
                            for (int i = 0; i < 8; i++)
                            {
                                Vector2 center = p + Helper.GetPolarCoord(108, MathHelper.PiOver4 * i);
                                Vector2 c2 = center;
                                switch (i)
                                {
                                    case 6:
                                        center.Y -= b;
                                        c2.Y -= 0.2928932f * b + 10;
                                        break;
                                    case 7:
                                    case 0:
                                    case 1:
                                        center.X += a;
                                        c2.X += 0.2928932f * a + 10;
                                        break;
                                    case 2:
                                        center.Y += b;
                                        c2.Y += 0.2928932f * b + 10;
                                        break;
                                    case 3:
                                    case 4:
                                    case 5:
                                        center.X -= a;
                                        c2.X -= 0.2928932f * a + 10;
                                        break;
                                }
                                rect[i] = new Rectangle((int)(center.X - a), (int)(center.Y - b), (int)(a * 2), (int)(b * 2));
                                dir[i] = (float)Math.Atan2(c2.Y - p.Y, c2.X - p.X);
                                int w = graphics.Viewport.Width / 2;
                                int h = graphics.Viewport.Height / 2;
                                score[i] = 0;
                                score[i] += w - Math.Abs(center.X - w);
                                score[i] += h - Math.Abs(center.Y - h);
                                if (center.X - a < 5 || center.X + a > w * 2 - 5 || center.Y - b < 5 || center.Y + b > h * 2 - 5)
                                    score[i] -= 10000;
                                foreach(TalkChara tc in charas.Values)
                                    if(tc != charas[talking] && Helper.CheckIntersectRectCircle(rect[i], tc.position, 64))
                                        score[i] -= 30;
                            }
                            int j = 0;
                            if (row.Length > 2)
                                j = int.Parse(row[2]);
                            else
                            {
                                float s = score[0];
                                for(int i = 1; i < 8; i++)
                                    if(score[i] > s)
                                    {
                                        j = i;
                                        s = score[i];
                                    }
                            }
                            bln_rect = rect[j];
                            bln_dir = dir[j];
                            #endregion
                            break;
                        case "Battle":
                            break;
                        case "Select":
                            break;
                        case "If":
                            break;
                        case "PlayBGM":
                            break;
                        case "PlaySE":
                            break;
                        case "Sleep":
                            sleepTime = TimeSpan.FromMilliseconds(int.Parse(row[1]));
                            break;
                        case "WaitAll":
                            waitall = true;
                            break;
                        case "End":
                            EndScenario();
                            GameBody.ChangeScene(Scene.World);
                            break;
                        default:
                            throw new FormatException();
                    }
                }
            }
            else if (state == State.Talk)
            {
                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
                    state = State.Free;
            }

            foreach (TalkChara tc in charas.Values)
                tc.Update(gameTime.ElapsedGameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            Texture2D tw = new Texture2D(graphics, 1, 1);
            Color[] c = { Color.White };
            tw.SetData(c);
            string str;

            spriteBatch.Begin();
            if (t_stage != null)
            {
                spriteBatch.Draw(t_stage, new Vector2(-24, 0), Color.White);
                spriteBatch.Draw(t_stage, new Vector2(360, 0), Color.White);
            }
            else
                spriteBatch.Draw(tw, new Rectangle(0, 0, 720, 480), Color.Black);

            foreach (TalkChara tc in charas.Values)
            {
                if (!tc.reverse)
                    spriteBatch.Draw(tc.texture, tc.position, null, Color.White, 0, new Vector2(64), 1, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(tc.texture, tc.position, null, Color.White, 0, new Vector2(64), 1, SpriteEffects.FlipHorizontally, 0);
            }
            if (state == State.Talk)
            {
                Vector2 msize = font.MeasureString(message);
                Vector2 v = new Vector2(bln_rect.Center.X - msize.X / 2, bln_rect.Center.Y - msize.Y / 2);

                spriteBatch.Draw(t_balloon2, charas[talking].position, null, Color.White, bln_dir, new Vector2(-72, 16), 1, SpriteEffects.None, 0);
                spriteBatch.Draw(t_balloon, bln_rect, Color.White);
                spriteBatch.DrawString(font, message, v, Color.Black);
            }
            spriteBatch.End();
        }

        /// <summary>
        /// シナリオファイルの読み込み（暗号なし）
        /// </summary>
        public static void LoadScenario(string filename)
        {
            string path = content.RootDirectory + "\\scenario\\" + filename + ".txt";
            parser = new TextFieldParser(path, System.Text.Encoding.GetEncoding("Shift_JIS"));
            parser.SetDelimiters(" ");
            parser.CommentTokens = new string[] { "#"};

            string[] row = parser.ReadFields();
            t_stage = content.Load<Texture2D>("img\\bg\\" + row[1]);

            state = State.Free;
            waitall = false;
            sleepTime = TimeSpan.Zero;
        }

        /// <summary>
        /// シナリオファイルの読み込み（暗号あり）
        /// </summary>
        public static void LoadScenario2(string filename)
        {
            string path = content.RootDirectory + "\\scenario\\" + filename + ".xnb";

            fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            cs = new CryptoStream(fs, decryptor, CryptoStreamMode.Read);

            parser = new TextFieldParser(cs, System.Text.Encoding.GetEncoding("Shift_JIS"));
            parser.SetDelimiters(" ");
            parser.CommentTokens = new string[] { "#"};

            string[] row = parser.ReadFields();
            t_stage = content.Load<Texture2D>("img\\bg\\" + row[1]);

            state = State.Free;
            waitall = false;
            sleepTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Streamを閉じる
        /// </summary>
        static void EndScenario()
        {
            charas.Clear();
            parser = null;
            //cs.Close();
            //fs.Close();
        }
    }

    class TalkChara
    {
        public Texture2D texture;
        public Vector2 position;
        public bool reverse;

        Vector2 goal;
        float speed;

        public TalkChara(Texture2D tex, float x, float y, bool reverse)
        {
            texture = tex;
            position = goal = new Vector2(x, y);
            this.reverse = reverse;
        }

        public void Move(float x, float y, double s)
        {
            goal = new Vector2(x, y);
            speed = (float)s;
        }

        public void Update(TimeSpan elps)
        {
            if (Vector2.Distance(position, goal) > speed)
            {
                Vector2 dir = goal - position;
                dir.Normalize();
                position += dir * speed * (float)elps.TotalMilliseconds;
            }
            else
                position = goal;
        }

        public bool Acting
        {
            get { return position != goal; }
        }
    }
}
