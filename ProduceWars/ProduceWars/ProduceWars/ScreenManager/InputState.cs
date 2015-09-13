#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
#endregion

namespace ProduceWars
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields

        public const int MaxInputs = 4;

        
        public readonly GamePadState[] CurrentGamePadStates;
        public readonly GamePadState[] LastGamePadStates;
        public readonly bool[] GamePadWasConnected;

#if WINDOWS
        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly KeyboardState[] LastKeyboardStates;
        public MouseState CurrentMouseState;
        public MouseState LastMouseState;
#endif


     //   public TouchCollection TouchState;
    //    public readonly List<GestureSample> Gestures = new List<GestureSample>();

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentGamePadStates = new GamePadState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
            GamePadWasConnected = new bool[MaxInputs];
#if WINDOWS
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            LastKeyboardStates = new KeyboardState[MaxInputs];
            CurrentMouseState = new MouseState();
            LastMouseState = new MouseState();
#endif
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
#if WINDOWS 
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
#endif
                LastGamePadStates[i] = CurrentGamePadStates[i];
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }
            }
#if WINDOWS

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
#endif


       //     TouchState = TouchPanel.GetState();
       //     Gestures.Clear();
       //     while (TouchPanel.IsGestureAvailable)
       //     {
       //         Gestures.Add(TouchPanel.ReadGesture());
       //     }
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
                                            out PlayerIndex playerIndex)
        {
#if WINDOWS
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
#endif
            playerIndex = controllingPlayer.Value;
            return false; //xbox keyboard disabled
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
                                                     out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }

        public bool IsNewMouseLeftClick()
        {
#if WINDOWS
                return (CurrentMouseState.LeftButton == ButtonState.Pressed &&
                        LastMouseState.LeftButton == ButtonState.Released);
#endif
            return false;
        }
        public bool IsNewMouseRightClick()
        {
#if WINDOWS
            return (CurrentMouseState.RightButton == ButtonState.Pressed &&
                    LastMouseState.RightButton == ButtonState.Released);
#endif
            return false;
        }
        public bool IsMouseHover(Rectangle itemBounds)
        {
#if WINDOWS
            return itemBounds.Contains(CurrentMouseState.X, CurrentMouseState.Y);
#endif
            return false;
        }
        public bool IsMouseSelect(Rectangle itemBounds)
        {
#if WINDOWS
            return (IsMouseHover(itemBounds) && IsNewMouseLeftClick());
#endif
            return false;
        }

        /// <summary>
        /// Checks for a "menu select" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex)) return true;
            if (IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex)) return true;
            if (IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex)) return true;
            if (IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex)) return true;
            if (IsNewMouseRightClick()) return true;
#endif
            return false;
        }

        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex)) return true;
            if (IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsMenuDown(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex)) return true;
            if (IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsMenuRight(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.LeftThumbstickRight, controllingPlayer, out playerIndex)) return true;
            if (IsNewButtonPress(Buttons.DPadRight, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Right, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsMenuLeft(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.LeftThumbstickLeft, controllingPlayer, out playerIndex)) return true;
            if (IsNewButtonPress(Buttons.DPadLeft, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Left, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsPauseGame(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex)) return true;
            if (IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsAPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex)) return true;
            if (IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex)) return true;
            if (IsNewMouseLeftClick()) return true;
#endif
            return false;
        }

        public bool IsBPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Back, controllingPlayer, out playerIndex)) return true;
            if (IsNewMouseRightClick()) return true;
#endif
            return false;
        }

        public bool IsXPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.X, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.X, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsYPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.Y, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Y, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsLShoulderPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.LeftShoulder, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Q, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsRShoulderPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.RightShoulder, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.E, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsLTriggerPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.LeftTrigger, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.Z, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsRTriggerPressed(PlayerIndex? controllingPlayer,out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.RightTrigger, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.C, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsDpadLeftPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.DPadLeft, controllingPlayer, out playerIndex)) return true; 
#if WINDOWS
            if (IsNewKeyPress(Keys.F, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }
        public bool IsDpadRightPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.DPadRight, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.H, controllingPlayer, out playerIndex)) return true; 
#endif
            return false;
        }
        public bool IsDpadUpPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex)) return true;
#if WINDOWS
            if (IsNewKeyPress(Keys.T, controllingPlayer, out playerIndex)) return true; 
#endif
            return false;
        }
        public bool IsDpadDownPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex)) return true; 
#if WINDOWS
            if (IsNewKeyPress(Keys.G, controllingPlayer, out playerIndex)) return true;
#endif
            return false;
        }

        public bool IsAnyFourPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (IsAPressed(controllingPlayer, out playerIndex)) return true;
            if (IsBPressed(controllingPlayer, out playerIndex)) return true;
            if (IsXPressed(controllingPlayer, out playerIndex)) return true;
            if (IsYPressed(controllingPlayer, out playerIndex)) return true;
            return false;
        }


        #endregion
    }
}
