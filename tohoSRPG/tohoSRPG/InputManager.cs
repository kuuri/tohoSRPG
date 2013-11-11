using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace tohoSRPG
{
    static class InputManager
    {
        public enum GameButton
        {
            Up, Down, Left, Right,
            Decide, Cancel, ShoulderL, ShoulderR, Pause
        }
        public enum ButtonState { Free, Push, Hold, Pull }

        static Dictionary<GameButton, ButtonState> order;
        static Dictionary<GameButton, Keys> buttonsKey;
        static Dictionary<GameButton, Buttons> buttonsPad;

        static public void Init()
        {
            order = new Dictionary<GameButton, ButtonState>();
            order.Add(GameButton.Up, ButtonState.Free);
            order.Add(GameButton.Down, ButtonState.Free);
            order.Add(GameButton.Left, ButtonState.Free);
            order.Add(GameButton.Right, ButtonState.Free);
            order.Add(GameButton.Decide, ButtonState.Free);
            order.Add(GameButton.Cancel, ButtonState.Free);
            order.Add(GameButton.ShoulderL, ButtonState.Free);
            order.Add(GameButton.ShoulderR, ButtonState.Free);
            order.Add(GameButton.Pause, ButtonState.Free);

            buttonsKey = new Dictionary<GameButton, Keys>();
            buttonsKey.Add(GameButton.Up, Keys.Up);
            buttonsKey.Add(GameButton.Down, Keys.Down);
            buttonsKey.Add(GameButton.Left, Keys.Left);
            buttonsKey.Add(GameButton.Right, Keys.Right);
            buttonsKey.Add(GameButton.Decide, Keys.Z);
            buttonsKey.Add(GameButton.Cancel, Keys.X);
            buttonsKey.Add(GameButton.ShoulderL, Keys.A);
            buttonsKey.Add(GameButton.ShoulderR, Keys.S);
            buttonsKey.Add(GameButton.Pause, Keys.Q);

            buttonsPad = new Dictionary<GameButton, Buttons>();
            buttonsPad.Add(GameButton.Up, Buttons.LeftThumbstickUp);
            buttonsPad.Add(GameButton.Down, Buttons.LeftThumbstickDown);
            buttonsPad.Add(GameButton.Left, Buttons.LeftThumbstickLeft);
            buttonsPad.Add(GameButton.Right, Buttons.LeftThumbstickRight);
            buttonsPad.Add(GameButton.Decide, Buttons.A);
            buttonsPad.Add(GameButton.Cancel, Buttons.B);
            buttonsPad.Add(GameButton.ShoulderL, Buttons.LeftShoulder);
            buttonsPad.Add(GameButton.ShoulderR, Buttons.RightShoulder);
            buttonsPad.Add(GameButton.Pause, Buttons.Start);
        }

        static public void Update(KeyboardState key, GamePadState pad)
        {
            for (int i = 0; i < Enum.GetNames(typeof(GameButton)).Length; i++)
                CheckOrder((GameButton)i, key, pad);
        }

        static void CheckOrder(GameButton o, KeyboardState key, GamePadState pad)
        {
            bool keybool;
            keybool = key.IsKeyDown(buttonsKey[o]);

            if (keybool || GetPadButtonState(o, pad))
            {
                if (order[o] == ButtonState.Free || order[o] == ButtonState.Pull)
                    order[o] = ButtonState.Push;
                else
                    order[o] = ButtonState.Hold;
            }
            else
            {
                if (order[o] == ButtonState.Free || order[o] == ButtonState.Pull)
                    order[o] = ButtonState.Free;
                else
                    order[o] = ButtonState.Pull;
            }
        }

        static bool GetPadButtonState(GameButton order, GamePadState pad)
        {
            switch (order)
            {
                case GameButton.Up:
                    return pad.ThumbSticks.Left.Y > 0.2 || pad.IsButtonDown(Buttons.DPadUp);
                case GameButton.Down:
                    return pad.ThumbSticks.Left.Y < -0.2 || pad.IsButtonDown(Buttons.DPadDown);
                case GameButton.Right:
                    return pad.ThumbSticks.Left.X > 0.2 || pad.IsButtonDown(Buttons.DPadRight);
                case GameButton.Left:
                    return pad.ThumbSticks.Left.X < -0.2 || pad.IsButtonDown(Buttons.DPadLeft);
                default:
                    return pad.IsButtonDown(buttonsPad[order]);
            }
        }

        static public void ChangeButton(GameButton o, Keys value)
        {
            if (buttonsKey.ContainsValue(value))
            {
                GameButton oldPos = new GameButton();
                oldPos = buttonsKey.FirstOrDefault(x => x.Value == value).Key;
                Keys tmp = buttonsKey[o];
                buttonsKey[o] = value;
                buttonsKey[oldPos] = tmp;
            }
            else
                buttonsKey[o] = value;
        }

        static public void ChangeButton(GameButton o, Buttons value)
        {
            if (buttonsPad.ContainsValue(value))
            {
                GameButton oldPos = new GameButton();
                oldPos = buttonsPad.FirstOrDefault(x => x.Value == value).Key;
                Buttons tmp = buttonsPad[o];
                buttonsPad[o] = value;
                buttonsPad[oldPos] = tmp;
            }
            else
                buttonsPad[o] = value;
        }

        #region ゲッタ
        static public ButtonState GetButtonState(GameButton button) { return order[button]; }

        static public bool GetButtonStateIsPush(GameButton button) { return order[button] == ButtonState.Push; }

        static public bool GetButtonStateAsBool(GameButton button)
        {
            if (order[button] == ButtonState.Push || order[button] == ButtonState.Hold)
                return true;
            else
                return false;
        }

        #endregion
    }
}
