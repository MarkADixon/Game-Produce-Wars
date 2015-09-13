#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using ParallaxEngine;
using ProduceWars.Managers;
#endregion

namespace ProduceWars
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization

        MenuEntry musicOn;
        MenuEntry multiSample;
        MenuEntry debugView;
        MenuEntry showControlsMenuEntry;
        MenuEntry blockDeath;
        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry restartGameMenuEntry = new MenuEntry("Restart Level");
            musicOn = new MenuEntry("Music :" + (GameSettings.MusicOn ? " On" : " Off"));
            multiSample = new MenuEntry("Graphics SuperSampling :" + (GameSettings.MultiSampling ? " On" : " Off"));
            debugView = new MenuEntry("Debug View :" + (GameSettings.DebugViewEnabled ? " On" : " Off"));
            blockDeath = new MenuEntry("Enable Destructable Blocks :" + (GameSettings.BlockDeathEnabled ? " On" : " Off"));
            showControlsMenuEntry = new MenuEntry("Show Controls :" + (GameSettings.ShowControlsEnabled ? " On" : " Off"));
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
            
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            restartGameMenuEntry.Selected += RestartGameMenuEntrySelected;
            musicOn.Selected += MusicOnSelected;
            multiSample.Selected += MultiSampleSelected;
            debugView.Selected += DebugViewSelected;
            blockDeath.Selected += BlockDeathSelected;
            showControlsMenuEntry.Selected += ShowControlsMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(multiSample);
            MenuEntries.Add(musicOn);
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(restartGameMenuEntry);
            MenuEntries.Add(debugView);
            MenuEntries.Add(blockDeath);
            MenuEntries.Add(showControlsMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }


        #endregion

        #region Handle Input
        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
            if (input.IsYPressed(ControllingPlayer, out playerIndex))
            {
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new GameplayScreen(LevelDataManager.world, LevelDataManager.level));
            }
        }

        void RestartGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new GameplayScreen(LevelDataManager.world, LevelDataManager.level));
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
        }

        void MusicOnSelected(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MusicOn = !GameSettings.MusicOn;
            musicOn.Text = ("Music :" + (GameSettings.MusicOn ? " On" : " Off"));
            SoundManager.PauseMusic(GameSettings.MusicOn);
        }

        void MultiSampleSelected(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MultiSampling = !GameSettings.MultiSampling;
            multiSample.Text = ("Graphics SuperSampling :" + (GameSettings.MultiSampling ? " On" : " Off"));
        }

        void DebugViewSelected(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.DebugViewEnabled = !GameSettings.DebugViewEnabled;
            debugView.Text  = ("Debug View :" + (GameSettings.DebugViewEnabled ? " On" : " Off"));
        }

        void BlockDeathSelected(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.BlockDeathEnabled = !GameSettings.BlockDeathEnabled;
            blockDeath.Text = ("Enable Destructable Blocks :" + (GameSettings.BlockDeathEnabled ? " On" : " Off"));
        }

        void ShowControlsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.ShowControlsEnabled = !GameSettings.ShowControlsEnabled;
            showControlsMenuEntry.Text = ("Show Controls :" + (GameSettings.ShowControlsEnabled ? " On" : " Off"));
        }
        #endregion
    }
}
