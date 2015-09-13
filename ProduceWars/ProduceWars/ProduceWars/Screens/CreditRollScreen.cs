using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using ProduceWars.Managers;
using ParallaxEngine;

namespace ProduceWars
{
    class CreditRollScreen : MenuScreen
    {
        Video video;
        VideoPlayer player;
        Texture2D videoTexture;
        Rectangle videoRect = new Rectangle(0, 0, 1280, 720);
        float videoAlpha = 1.0f;

        public CreditRollScreen()
            : base("")
        {
            TransitionOnTime = TimeSpan.FromSeconds(1f);
            TransitionOffTime = TimeSpan.FromSeconds(1f);
        }

        public override void LoadContent(ContentManager _content)
        {
            base.LoadContent(_content);
            video = LevelDataManager.tempContent.Load<Video>(@"Video\endwithtomato2");
            player = new VideoPlayer();
            player.Play(video);
        }


        public override void UnloadContent()
        {
            LevelDataManager.tempContent.Unload();
        }

        public override void HandleInput(InputState input)
        {
            if (player.State != MediaState.Stopped)
            {
                if (input.IsBPressed(ControllingPlayer, out playerIndex))
                {
                    player.Stop();
                }
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (videoAlpha <= 0) LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.Begin();

            // Only call GetTexture if a video is playing or paused
            if (player.State != MediaState.Stopped)
                videoTexture = player.GetTexture();
            else
            {
                videoAlpha -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            // Draw the video, if we have a texture to draw.
            if (videoTexture != null) ScreenManager.SpriteBatch.Draw(videoTexture, videoRect, Color.White * videoAlpha);

            ScreenManager.SpriteBatch.End();

        }


    }
}
