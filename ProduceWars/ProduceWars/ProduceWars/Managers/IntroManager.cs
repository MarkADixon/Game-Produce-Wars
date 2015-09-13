using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParallaxEngine;

namespace ProduceWars
{
    public class IntroManager
    {
        TextBox textBox;
        Vector2 textBoxPos = new Vector2(128, 196);
        Texture2D pixel, speakerTexture,stache;
        Texture2D cursor, A, B;
        Vector2 lookPos = Vector2.Zero;
        Vector2 cursorPos = Vector2.Zero;
        Vector2 cursorPosoffset = Vector2.Zero;
        int posDir = 1;
        SpriteFont font,fontB;
        enum Speaker {None, KingPear, DocBroc, AppleJack };
        Speaker speaker = Speaker.None;
        bool introFinished = false;
        bool transitionOut = false;
        float transitionAlpha = 0f;
        int textBoxIndex = 0;
        int textBoxCount = 0;
        int world;
        int level;
        Vector2 speakerOffset = Vector2.Zero;
        float talkYoffset = 0;
        bool talkoffsetDir = false;
        Vector2 speakerPos = Vector2.Zero;
        int textHeight;
        int buffer = 16;
        Color backColor = Color.White;
        int halfW = 0;

        public IntroManager(SpriteFont _font,SpriteFont _fontB, int _world, int _level)
        {
            font = _font;
            fontB = _fontB;
            world = _world;
            level = _level;
            textBoxCount = LevelDataManager.levelData[world, level].textBox.Count;
            textHeight = (int)(font.MeasureString("A").Y * 3) + (buffer * 2);
            if (!LevelDataManager.levelData[world, level].isIntro) introFinished = true;
            LevelDataManager.UItextures.TryGetValue("Pixel", out pixel);
            LevelDataManager.UItextures.TryGetValue("PearStache", out stache);
            LevelDataManager.UItextures.TryGetValue("Cursor", out cursor);
            LevelDataManager.UItextures.TryGetValue("A", out A);
            LevelDataManager.UItextures.TryGetValue("B", out B);
            halfW = Camera.ViewportWidth / 2;
               
            if (!introFinished)
            {
                SetSpeaker();
                textBox = new TextBox(LevelDataManager.levelData[world, level].textBox[textBoxIndex], font, pixel, Color.White * 0.3f, new Rectangle((int)textBoxPos.X, (int)textBoxPos.Y, Camera.ViewportWidth - 256, textHeight), buffer, speakerOffset);
                cursorPos = LevelDataManager.levelData[world, level].cursor[textBoxIndex];
                lookPos = LevelDataManager.levelData[world, level].look[textBoxIndex];
                if (lookPos == Vector2.Zero) lookPos = cursorPos;
                if (lookPos == Vector2.Zero) lookPos = new Vector2(0, Camera.WorldRectangle.Height - Camera.ViewportHeight/2);
                Camera.ScrollTo(lookPos,0);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (introFinished) 
                return;
            if (transitionAlpha < 1f && !transitionOut) transitionAlpha = Math.Min(1f, transitionAlpha + (float)gameTime.ElapsedGameTime.TotalSeconds);
            if (transitionOut)
            {
                transitionAlpha = Math.Max(0, transitionAlpha - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (transitionAlpha <= 0.001) introFinished = true;
            }

            if (!textBox.isFinished)
            {
                textBox.Update(gameTime);
                int roll = LevelDataManager.rand.Next(0, 8);
                if (roll == 0) talkoffsetDir = !talkoffsetDir;
                if (talkoffsetDir) talkYoffset += 0.5f;
                else talkYoffset -= 0.5f;
                talkYoffset = (float)MathHelper.Clamp(talkYoffset, -3, 1);
            }
            else talkYoffset = 0;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            backColor = Color.White * transitionAlpha;

            if (introFinished) return;
            
            spriteBatch.Begin();
            spriteBatch.Draw(pixel, new Rectangle(0,0,Camera.Viewport.Width,Camera.Viewport.Height), backColor * 0.3f);
            if (cursorPos != Vector2.Zero)
            {
                cursorPosoffset += new Vector2(0, posDir * 0.5f);
                if (cursorPosoffset.Y > 4) posDir = -1;
                if (cursorPosoffset.Y < -4) posDir = 1;
                spriteBatch.Draw(cursor, Camera.WorldToScreen(cursorPos + cursorPosoffset + new Vector2(-cursor.Width / 2, -cursor.Height)), null, backColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
            spriteBatch.End();

            textBox.Draw(gameTime, spriteBatch, transitionAlpha);

            spriteBatch.Begin();
            spriteBatch.Draw(speakerTexture, speakerPos, null, backColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            if (speaker == Speaker.KingPear)
            {
                spriteBatch.Draw(stache, new Vector2 (speakerPos.X, speakerPos.Y + talkYoffset), null,backColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }

            //draw buttons
            spriteBatch.Draw(A, new Rectangle((int)halfW+200, (int)textBoxPos.Y + textHeight, 48, 48), backColor);
            spriteBatch.Draw(B, new Rectangle((int)halfW+350, (int)textBoxPos.Y + textHeight, 48, 48), backColor);
            DrawStringHelper(spriteBatch, fontB, "TALK", new Vector2(halfW +250, textBoxPos.Y + textHeight+8), Color.White, Vector2.Zero, 1f, SpriteEffects.None, transitionAlpha);
            DrawStringHelper(spriteBatch, fontB, "EXIT", new Vector2(halfW +400, textBoxPos.Y + textHeight+8), Color.White, Vector2.Zero, 1f, SpriteEffects.None, transitionAlpha);

            spriteBatch.End();
        }

        public void AisPressed()
        {
            if (!textBox.isFinished)
            {
                textBox.finishNow = true;
            }
            else
            {
                textBoxIndex += 1;
                if (textBoxIndex >= textBoxCount) transitionOut = true;
                else
                {
                    SetSpeaker();
                    textBox = new TextBox(LevelDataManager.levelData[world, level].textBox[textBoxIndex], font, pixel, Color.White * 0.3f, new Rectangle(128, 196, Camera.ViewportWidth - 256, textHeight), buffer, speakerOffset);
                    cursorPos = LevelDataManager.levelData[world, level].cursor[textBoxIndex];
                    lookPos = LevelDataManager.levelData[world, level].look[textBoxIndex];
                    if (lookPos == Vector2.Zero) lookPos = cursorPos;
                    if (lookPos == Vector2.Zero) lookPos = new Vector2(0, Camera.WorldRectangle.Height - Camera.ViewportHeight/2);
                    Camera.ScrollTo(lookPos, 0);
                }
            }
        }

        public bool IntroFinished()
        {
            return introFinished;
        }

        public void Finish()
        {
            introFinished = true;
        }

        public void SetSpeaker()
        {
            //kingpear
            if (LevelDataManager.levelData[world, level].speaker[textBoxIndex] == 1)
            {
                speaker = Speaker.KingPear;
                LevelDataManager.UItextures.TryGetValue("Pear", out speakerTexture);
                speakerPos = new Vector2(textBoxPos.X, textBoxPos.Y + (textHeight * 0.5f) - (speakerTexture.Height * 0.5f));
                speakerOffset = new Vector2(256, 0);
            }
            //doc
            if (LevelDataManager.levelData[world, level].speaker[textBoxIndex] == 2)
            {
                speaker = Speaker.DocBroc;
                LevelDataManager.UItextures.TryGetValue("Broc", out speakerTexture);  
                speakerPos = new Vector2(textBoxPos.X, textBoxPos.Y + (textHeight * 0.5f) - (speakerTexture.Height * 0.5f));
                speakerOffset = new Vector2(256, 0);
            }
            //apple jack
            if (LevelDataManager.levelData[world, level].speaker[textBoxIndex] == 3)
            {
                speaker = Speaker.AppleJack;
                LevelDataManager.UItextures.TryGetValue("AppleJackX", out speakerTexture);
                speakerPos = new Vector2(textBoxPos.X + 32, textBoxPos.Y + (textHeight * 0.5f) - (speakerTexture.Height * 0.5f) - 16);
                speakerOffset = new Vector2(192, 0);
            }

            return; 
        }

        protected void DrawStringHelper(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, Vector2 origin, float scale, SpriteEffects effect, float alpha)
        {
            spriteBatch.DrawString(font, text, new Vector2(2, 2) + position, Color.Black * alpha, 0, origin, scale, effect, 0);
            spriteBatch.DrawString(font, text, new Vector2(-2, 2) + position, Color.Black * alpha, 0, origin, scale, effect, 0);
            spriteBatch.DrawString(font, text, new Vector2(2, -2) + position, Color.Black * alpha, 0, origin, scale, effect, 0);
            spriteBatch.DrawString(font, text, new Vector2(-2, -2) + position, Color.Black * alpha, 0, origin, scale, effect, 0);
            spriteBatch.DrawString(font, text, position, color, 0, origin, scale, effect, 0);
        }


    }
}
