#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using ProduceWars.Managers;
using ParallaxEngine;
using EasyStorage;
#endregion

namespace ProduceWars
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class PressStartScreen : MenuScreen
    {
        #region Initialization
        IAsyncSaveDevice saveDevice;
        string text = "Press  A  or Start to begin";
        float textscale = 0.8f;
        Vector2 textLength = Vector2.Zero;
        Vector2 textPosition = new Vector2(640, 350);
        Video video;
        VideoPlayer player;
        Texture2D videoTexture;
        Rectangle videoRect = new Rectangle(0, 0, 1280, 720);
        float videoAlpha = 1.0f;
        bool showAButton = true;
        SharedSaveDevice sharedSaveDevice;
        bool isPrompt = false;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public PressStartScreen()
            : base("Press Start")
        {
        }

        public override void LoadContent(ContentManager _content)
        {
            base.LoadContent(_content);
            TransitionOnTime = TimeSpan.FromSeconds(0f);
            TransitionOffTime = TimeSpan.FromSeconds(0f);
            LevelDataManager.UItextures.TryGetValue("Title", out menuTitleGraphic);
            isMenuTitleGraphic = true;
            menuTitleGraphicScale = 1.0f;

            textLength = ScreenManager.font.MeasureString(text) * textscale;

            video = LevelDataManager.tempContent.Load<Video>(@"Video\pw_intro2");
            player = new VideoPlayer();
            player.Play(video);

            // we can set our supported languages explicitly or we can allow the
            // game to support all the languages. the first language given will
            // be the default if the current language is not one of the supported
            // languages. this only affects the text found in message boxes shown
            // by EasyStorage and does not have any affect on the rest of the game.
            EasyStorageSettings.SetSupportedLanguages(Language.English, Language.French, Language.Spanish, Language.German, Language.Italian, Language.Japanese);
            // on Windows and Xbox 360, we use a save device that gets a
            //shared StorageDevice to handle our file IO.

            // create and add our SaveDevice
            sharedSaveDevice = new SharedSaveDevice();
            ScreenManager.Game.Components.Add(sharedSaveDevice);
            // make sure we hold on to the device
            saveDevice = sharedSaveDevice;
            LevelDataManager.SaveDevice = saveDevice;
            // hook two event handlers to force the user to choose a new device if they cancel the
            // device selector or if they disconnect the storage device after selecting it
            sharedSaveDevice.DeviceSelectorCanceled +=
                (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceDisconnected +=
                (s, e) => e.Response = SaveDeviceEventResponse.Force;

#if XBOX
            // add the GamerServicesComponent
            ScreenManager.Game.Components.Add(
                new Microsoft.Xna.Framework.GamerServices.GamerServicesComponent(ScreenManager.Game));
#endif

            sharedSaveDevice.DeviceSelected += (s, e) =>
            {
                LoadOptions();
                LevelDataManager.ReadSaveGameData();
                isPrompt = true;
            };

        }


        public override void UnloadContent()
        {
            LevelDataManager.tempContent.Unload();
        }

        #endregion

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (isPrompt && IsActive)
            {
                SoundManager.Play(SoundManager.Sound.StarCollect, false, false);
                ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
                this.ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            ScreenManager.SpriteBatch.Begin();
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.font, text, textPosition - (textLength * 0.5f), Color.White*TransitionAlpha, Vector2.Zero, textscale, SpriteEffects.None, TransitionAlpha);
            ScreenManager.SpriteBatch.Draw(A, new Rectangle((int)textPosition.X-168,(int)textPosition.Y-30, 64, 64), Color.White * TransitionAlpha);
           
            // Only call GetTexture if a video is playing or paused
            if (player.State != MediaState.Stopped)
                videoTexture = player.GetTexture();
            else
            {
                videoAlpha -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            // Draw the video, if we have a texture to draw.
            if (videoTexture != null) ScreenManager.SpriteBatch.Draw(videoTexture, videoRect, Color.White*videoAlpha);
           
            ScreenManager.SpriteBatch.End();
        
        }

        public override void HandleInput(InputState input)
        {
            if (player.State != MediaState.Stopped)
            {
                if (input.IsAnyFourPressed(ControllingPlayer, out playerIndex) || input.IsMenuSelect(ControllingPlayer, out playerIndex))
                {
                    ControllingPlayer = playerIndex;
                    player.Stop();
                }
            }
            else
            {
                if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
                {
                    ControllingPlayer = playerIndex;
                    // prompt for a device on the first Update we can
                    if (!isPrompt) sharedSaveDevice.PromptForDevice();  
                }
            }
        }

        /// <summary>
        /// When the user cancels, exit
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }


        private void PromptMe(PlayerIndex playerIndex)
        {
            // we can set our supported languages explicitly or we can allow the
            // game to support all the languages. the first language given will
            // be the default if the current language is not one of the supported
            // languages. this only affects the text found in message boxes shown
            // by EasyStorage and does not have any affect on the rest of the game.
//            EasyStorageSettings.SetSupportedLanguages(Language.English, Language.French, Language.Spanish, Language.German, Language.Italian, Language.Japanese);
            // on Windows and Xbox 360, we use a save device that gets a
            //shared StorageDevice to handle our file IO.

            // create and add our SaveDevice
//            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
//            ScreenManager.Game.Components.Add(sharedSaveDevice);
            // make sure we hold on to the device
//            saveDevice = sharedSaveDevice;
            // hook two event handlers to force the user to choose a new device if they cancel the
            // device selector or if they disconnect the storage device after selecting it
//            sharedSaveDevice.DeviceSelectorCanceled +=
//                (s, e) => e.Response = SaveDeviceEventResponse.Force;
//            sharedSaveDevice.DeviceDisconnected +=
//                (s, e) => e.Response = SaveDeviceEventResponse.Force;
            // prompt for a device on the first Update we can
//            sharedSaveDevice.PromptForDevice();
//#if XBOX
//            // add the GamerServicesComponent
//            ScreenManager.Game.Components.Add(
//                new Microsoft.Xna.Framework.GamerServices.GamerServicesComponent(ScreenManager.Game));
//#endif

                //Save our save device to the global counterpart, so we can access it
                //anywhere we want to save/load
//                LevelDataManager.SaveDevice = (SaveDevice)s;
                //Once they select a storage device, we can load the main menu.
                //We need to perform a check to see if we're on the Press Start Screen.
                //If a storage device is selected NOT from this page, we don't want to
                //create a new Main Menu screen! (Thanks @FreelanceGames for the mention)
                
                {
              
                }
//            };
        }

        public static void LoadOptions()
        {
            //attempt to load options
            if (LevelDataManager.SaveDevice.FileExists(LevelDataManager.containerName, LevelDataManager.fileName_options))
            {
                try
                {
                    LevelDataManager.SaveDevice.Load(
                        LevelDataManager.containerName,
                        LevelDataManager.fileName_options,
                        stream =>
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                GameSettings.MusicOn = bool.Parse(reader.ReadLine());
                                GameSettings.MusicVolume = int.Parse(reader.ReadLine());
                                GameSettings.EffectVolume = int.Parse(reader.ReadLine());
                                GameSettings.ShotColorIndex = int.Parse(reader.ReadLine());
                                GameSettings.ShotPathColor = GameSettings.Colors[GameSettings.ShotColorIndex];
                            }
                        });
                }
                catch
                {
                }
            }
            return;
        }
    }
}
