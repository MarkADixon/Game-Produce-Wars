using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ParallaxEngine;
//using ProduceWars.Managers;

namespace ProduceWars
{
    class PopUpGraphicScreen : GameScreen
    {
        Texture2D graphic;
        string graphicToLoad;
        public enum TransitionType{ Fade }
        TransitionType ttype;
        Vector2 positionOffset;
        PlayerIndex playerIndex;
        Vector2 position;

        public PopUpGraphicScreen(Texture2D _graphic, Vector2 _positionOffset, TransitionType _ttype, TimeSpan _transitionTime)
        {
            graphic = _graphic;
            ttype = _ttype;
            TransitionOnTime = _transitionTime;
            TransitionOffTime = _transitionTime;
            positionOffset = _positionOffset;
            position = Camera.Origin - new Vector2(graphic.Width * 0.5f, graphic.Height * 0.5f) + positionOffset;
        }

        public override void LoadContent(ContentManager _content)
        {
            base.LoadContent(_content);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(graphic, position, Color.White*TransitionAlpha);
            ScreenManager.SpriteBatch.End();
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            if (input.IsAnyFourPressed(ControllingPlayer, out playerIndex))
            {
                ExitScreen();
            }
        }


    }
}
