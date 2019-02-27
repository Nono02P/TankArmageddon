using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TankArmageddon
{
    public enum ClickType
    {
        Left,
        Middle,
        Right,
        X1,
        X2,
    }

    static class Input
    {
        private class Click : InputElement
        {
            public ClickType ClickCode { get; }

            public Click(ClickType click)
            {
                ClickCode = click;
            }

            public void Update(MouseState _lastMouseState, MouseState _currentMouseState)
            {
                switch (ClickCode)
                {
                    case ClickType.Left:
                        IsDown = _currentMouseState.LeftButton == ButtonState.Pressed;
                        OnPressed = _lastMouseState.LeftButton == ButtonState.Released && IsDown;
                        IsUp = _currentMouseState.LeftButton == ButtonState.Released;
                        OnReleased = _lastMouseState.LeftButton == ButtonState.Pressed && IsUp;
                        break;
                    case ClickType.Middle:
                        IsDown = _currentMouseState.MiddleButton == ButtonState.Pressed;
                        OnPressed = _lastMouseState.MiddleButton == ButtonState.Released && IsDown;
                        IsUp = _currentMouseState.MiddleButton == ButtonState.Released;
                        OnReleased = _lastMouseState.MiddleButton == ButtonState.Pressed && IsUp;
                        break;
                    case ClickType.Right:
                        IsDown = _currentMouseState.RightButton == ButtonState.Pressed;
                        OnPressed = _lastMouseState.RightButton == ButtonState.Released && IsDown;
                        IsUp = _currentMouseState.RightButton == ButtonState.Released;
                        OnReleased = _lastMouseState.RightButton == ButtonState.Pressed && IsUp;
                        break;
                    case ClickType.X1:
                        IsDown = _currentMouseState.XButton1 == ButtonState.Pressed;
                        OnPressed = _lastMouseState.XButton1 == ButtonState.Released && IsDown;
                        IsUp = _currentMouseState.XButton1 == ButtonState.Released;
                        OnReleased = _lastMouseState.XButton1 == ButtonState.Pressed && IsUp;
                        break;
                    case ClickType.X2:
                        IsDown = _currentMouseState.XButton2 == ButtonState.Pressed;
                        OnPressed = _lastMouseState.XButton2 == ButtonState.Released && IsDown;
                        IsUp = _currentMouseState.XButton2 == ButtonState.Released;
                        OnReleased = _lastMouseState.XButton2 == ButtonState.Pressed && IsUp;
                        break;
                    default:
                        throw new Exception("Type de bouton non géré pour la souris");
                }
            }

            public override string ToString()
            {
                return "ClickCode : " + ClickCode + "; OnPressed : " + OnPressed + "; IsDown : " + IsDown + "; OnReleased : " + OnReleased + "; IsUp : " + IsUp;
            }
        }

        private class Key : InputElement
        {
            public Keys KeyCode { get; }

            public Key(Keys key)
            {
                KeyCode = key;
            }

            public void Update(KeyboardState _lastKeyboardState, KeyboardState _currentKeyboardState)
            {
                IsDown = _currentKeyboardState.IsKeyDown(KeyCode);
                OnPressed = !_lastKeyboardState.IsKeyDown(KeyCode) && IsDown;
                IsUp = _currentKeyboardState.IsKeyUp(KeyCode);
                OnReleased = _lastKeyboardState.IsKeyDown(KeyCode) && IsUp;
            }

            public override string ToString()
            {
                return "KeyCode : " + KeyCode + "; OnPressed : " + OnPressed + "; IsDown : " + IsDown + "; OnReleased : " + OnReleased + "; IsUp : " + IsUp;
            }
        }

        private class Button : InputElement
        {
            public Buttons ButtonCode { get; }

            public Button(Buttons button)
            {
                ButtonCode = button;
            }

            public void Update(GamePadState lastGamePadState, GamePadState currentGamePadState)
            {
                IsDown = currentGamePadState.IsButtonDown(ButtonCode);
                OnPressed = lastGamePadState.IsButtonUp(ButtonCode) && IsDown;
                IsUp = currentGamePadState.IsButtonUp(ButtonCode);
                OnReleased = lastGamePadState.IsButtonDown(ButtonCode) && IsUp;
            }

            public override string ToString()
            {
                return "ButtonCode : " + ButtonCode + "; OnPressed : " + OnPressed + "; IsDown : " + IsDown + "; OnReleased : " + OnReleased + "; IsUp : " + IsUp;
            }
        }


        public static readonly bool usingMouse = MainGame.UsingMouse;
        public static readonly bool usingKeyboard = MainGame.UsingKeyboard;
        public static readonly bool usingGamePad = MainGame.UsingGamePad;
        public static readonly int GamePadMaxPlayer = MainGame.GamePadMaxPlayer;

        //Mouse
        private static MouseState _currentMouseState;
        private static MouseState _lastMouseState;
        private static readonly Dictionary<ClickType, Click> _clicks;

        //Keyboard --TODO Add a list or other to never delete Key object when Clear() is called, this list contains few Keys wich are updated automatically
        private static KeyboardState _currentKeyboardState;
        private static KeyboardState _lastKeyboardState;
        private static Dictionary<Keys, Key> _keys;

        //GamePad
        private static int _gamePadCurrentPlayer;
        public static int GamePadCurrentPlayer
        {
            get { return _gamePadCurrentPlayer; }
            set
            {
                int total = value - _gamePadCurrentPlayer;
                if (total > 0)
                {
                    for (int i = 0; i < total; i++)
                    {
                        bool result = AddGamePad();
                        if (!result)
                        {
                            break;
                        }
                    }
                }
                else if (total < 0)
                {
                    for (int i = 0; i > total; i--)
                    {
                        RemoveGamePad();
                        if (_gamePadCurrentPlayer == 0)
                        {
                            break;
                        }
                    }
                }
            }
        }
        private static bool[] _gamePadConnected;
        private static GamePadState[] _currentGamePadState;
        private static GamePadState[] _lastGamePadState;
        private static Dictionary<Buttons, Button>[] _buttons;
        

        //Constructor
        static Input()
        {
            //Mouse
            if (usingMouse)
            {
                _currentMouseState = Mouse.GetState();
                _lastMouseState = _currentMouseState;
                _clicks = new Dictionary<ClickType, Click>()
                {
                    {ClickType.Left, new Click(ClickType.Left)},
                    {ClickType.Middle, new Click(ClickType.Middle)},
                    {ClickType.Right, new Click(ClickType.Right)},
                    {ClickType.X1, new Click(ClickType.X1)},
                    {ClickType.X2, new Click(ClickType.X2)},
                };
            }

            //Keyboard
            if (usingKeyboard)
            {
                _currentKeyboardState = Keyboard.GetState();
                _lastKeyboardState = _currentKeyboardState;
                _keys = new Dictionary<Keys, Key>();
            }

            //GamePad
            if (usingGamePad)
            {
                if (GamePadMaxPlayer > 4)
                {
                    GamePadMaxPlayer = 4;
                }
                _gamePadCurrentPlayer = 0;
                _gamePadConnected = new bool[GamePadMaxPlayer];
                _currentGamePadState = new GamePadState[GamePadMaxPlayer];
                _lastGamePadState = new GamePadState[GamePadMaxPlayer];
                _buttons = new Dictionary<Buttons, Button>[GamePadMaxPlayer];
                for (int i = 0; i < GamePadMaxPlayer; i++)
                {
                    _gamePadConnected[i] = GamePad.GetCapabilities((PlayerIndex)i).IsConnected;
                    if (_gamePadConnected[i])
                    {
                        _currentGamePadState[i] = GamePad.GetState((PlayerIndex)i, GamePadDeadZone.IndependentAxes);
                        _lastGamePadState[i] = _currentGamePadState[i];
                    }
                    _buttons[i] = new Dictionary<Buttons, Button>();
                }
            }
        }


        //Get
        public static InputElement Get(ClickType click)
        {
            return _clicks[click];
        }

        public static InputElement Get(Keys key)
        {
            return _keys[key];
        }

        //Add
        private static void Add(Keys key)
        {
            _keys.Add(key, new Key(key));
            _keys[key].Update(_lastKeyboardState, _currentKeyboardState);
        }
        
        private static void Add(Buttons button, PlayerIndex playerIndex)
        {
            if ((int)playerIndex >= 0 && (int)playerIndex < _gamePadCurrentPlayer)
            {
                _buttons[(int)playerIndex].Add(button, new Button(button));
            }
        }

        private static bool AddGamePad()
        {
            if (GamePadCurrentPlayer < GamePadMaxPlayer)
            {
                _gamePadCurrentPlayer += 1;
                return true;
            }
            return false;
        }


        //Remove
        public static void RemoveGamePad()
        {
            if (GamePadCurrentPlayer > 0)
            {
                _buttons[GamePadCurrentPlayer].Clear();
                _gamePadCurrentPlayer -= 1;
            }
        }


        //Clear
        public static void Clear()
        {
            ClearKey();
            ClearButton();
        }

        public static void ClearKey()
        {
            _keys.Clear();
        }

        public static void ClearButton()
        {
            for (int i = 0; i < GamePadMaxPlayer; i++)
            {
                _buttons[i].Clear();
            }
        }


        //Update
        public static void Update()
        {
            //Mouse
            if (usingMouse)
            {
                _lastMouseState = _currentMouseState;
                _currentMouseState = Mouse.GetState();
                foreach (KeyValuePair<ClickType, Click> kVP in _clicks)
                {
                    kVP.Value.Update(_lastMouseState, _currentMouseState);
                }
            }

            //Keyboard
            if (usingKeyboard)
            {
                _lastKeyboardState = _currentKeyboardState;
                _currentKeyboardState = Keyboard.GetState();
                foreach (KeyValuePair<Keys, Key> kVP in _keys)
                {
                    kVP.Value.Update(_lastKeyboardState, _currentKeyboardState);
                }
            }

            //GamePad
            if (usingGamePad)
            {
                for (int i = 0; i < GamePadCurrentPlayer; i++)
                {
                    _gamePadConnected[i] = GamePad.GetCapabilities((PlayerIndex)i).IsConnected;
                    if (_gamePadConnected[i])
                    {
                        _lastGamePadState[i] = _currentGamePadState[i];
                        _currentGamePadState[i] = GamePad.GetState((PlayerIndex)i, GamePadDeadZone.IndependentAxes);

                        foreach (KeyValuePair<Buttons, Button> kVP in _buttons[i])
                        {
                            kVP.Value.Update(_lastGamePadState[i], _currentGamePadState[i]);
                        }
                    }
                }
            }
        }


        //Mouse
        public static bool IsDown(ClickType click)
        {
            return _clicks[click].IsDown;
        }

        public static bool OnPressed(ClickType click)
        {
            return _clicks[click].OnPressed;
        }

        public static bool IsUp(ClickType click)
        {
            return _clicks[click].IsUp;
        }

        public static bool OnReleased(ClickType click)
        {
            return _clicks[click].OnReleased;
        }

        //Key
        public static bool IsDown(Keys key)
        {
            if (!_keys.ContainsKey(key))
            {
                Add(key);
            }
            return _keys[key].IsDown;
        }

        public static bool OnPressed(Keys key)
        {
            if (!_keys.ContainsKey(key))
            {
                Add(key);
            }
            return _keys[key].OnPressed;
        }

        public static bool IsUp(Keys key)
        {
            if (!_keys.ContainsKey(key))
            {
                Add(key);
            }
            return _keys[key].IsUp;
        }

        public static bool OnReleased(Keys key)
        {
            if (!_keys.ContainsKey(key))
            {
                Add(key);
            }
            return _keys[key].OnReleased;
        }

        //Button
        public static bool IsDown(Buttons button, PlayerIndex playerIndex)
        {
            if (!_buttons[(int)playerIndex].ContainsKey(button))
            {
                Add(button, playerIndex);
            }
            return _buttons[(int)playerIndex][button].IsDown;
        }

        public static bool OnPressed(Buttons button, PlayerIndex playerIndex)
        {
            if (!_buttons[(int)playerIndex].ContainsKey(button))
            {
                Add(button, playerIndex);
            }
            return _buttons[(int)playerIndex][button].OnPressed;
        }

        public static bool IsUp(Buttons button, PlayerIndex playerIndex)
        {
            if (!_buttons[(int)playerIndex].ContainsKey(button))
            {
                Add(button, playerIndex);
            }
            return _buttons[(int)playerIndex][button].IsUp;
        }

        public static bool OnReleased(Buttons button, PlayerIndex playerIndex)
        {
            if (!_buttons[(int)playerIndex].ContainsKey(button))
            {
                Add(button, playerIndex);
            }
            return _buttons[(int)playerIndex][button].OnReleased;
        }


        //ToString
        public static new string ToString()
        {
            string total = "";
            total += "\n\tClicks : ";
            foreach (KeyValuePair<ClickType, Click> kVP in _clicks)
            {
                total += "\n\t\t" + kVP.Value;
            }
            total += "\n\tKeys : ";
            foreach(KeyValuePair<Keys, Key> kVP in _keys)
            {
                total += "\n\t\t" + kVP.Value;
            }
            total += "\n\tButtons : ";
            for (int i = 0; i < GamePadCurrentPlayer; i++)
            {
                total += "\n\t\tPlayer : " + (i + 1);
                foreach (KeyValuePair<Buttons, Button> kVP in _buttons[i])
                {
                    total += "\n\t\t\t" + kVP.Value;
                }
            }
            return "Input are : " + total;
        }
    }

    public class InputElement
    {
        public bool IsDown { get; protected set; }
        public bool OnPressed { get; protected set; }
        public bool IsUp { get; protected set; }
        public bool OnReleased { get; protected set; }

        public InputElement()
        {
            IsDown = false;
            OnPressed = false;
            IsUp = false;
            OnReleased = false;
        }
    }
}