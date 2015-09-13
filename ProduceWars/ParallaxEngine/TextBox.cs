using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParallaxEngine
{
    public class TextBox
    {
        string Text = "";
        SpriteFont font;
        Rectangle boxRectangle;
        int edgeBuffer = 0; //distance between edge of the box and text area
        Vector2 extraBufferLR = Vector2.Zero; //extra L or R offset to allow space for a graphic
        Vector2 textPos = Vector2.Zero;
        Vector2 textWidthHeight = Vector2.Zero;
        Texture2D background;
        Color backgroundColor;
        List<string> Lines;

        char[] letterArray; //internally used to divide the text into letters
        string nextWord; //used for measuring 
        int letterCounter = 0;
        Vector2 nextWordMeasurement = Vector2.Zero;
        Vector2 lineMeasurement = Vector2.Zero;

        int lettersPerSecond = 30;
        int timer = 0; //ms
        int timerThreshold = 0; //ms

        public bool isFinished = false;
        public bool finishNow = false;

        public TextBox(string _text, SpriteFont _font, Texture2D _background, Color _backgroundColor, Rectangle _box, int buffer, Vector2 _extraBufferLR)
        {
            Text = _text;
            font = _font;
            background = _background;
            backgroundColor = _backgroundColor;
            boxRectangle = _box; 
            edgeBuffer = buffer;
            extraBufferLR = _extraBufferLR;
            textPos = new Vector2 (boxRectangle.X + buffer + extraBufferLR.X, boxRectangle.Y + buffer);
            textWidthHeight = new Vector2 (boxRectangle.Width - (2*buffer) - extraBufferLR.X - extraBufferLR.Y, boxRectangle.Height - (2*buffer));
            Lines = new List<string>();
            Lines.Add("");

            letterArray = Text.ToCharArray();
            timerThreshold = 1000 / lettersPerSecond;
        }

        public void Update(GameTime gameTime)
        {
            if (isFinished) return;

            if (!finishNow)
            {

                timer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timer > timerThreshold)
                {
                    timer -= timerThreshold;
                    if (letterCounter < letterArray.Count())
                    {
                        int thisLine = Lines.Count - 1;

                        if (letterArray[letterCounter].ToString() != " ")
                        {
                            Lines[thisLine] = Lines[thisLine] + letterArray[letterCounter].ToString();
                            letterCounter += 1;
                        }
                        else
                        {
                            //build next word
                            BuildNextWord();

                            //measure the next word and see if it will fit (with a space)
                            nextWordMeasurement = font.MeasureString(nextWord + " ");

                            //measure the line
                            lineMeasurement = font.MeasureString(Lines[thisLine]);

                            //if the word fits on the line, add add space and continue letter adding, add 1 to word counter
                            if ((nextWordMeasurement.X + lineMeasurement.X) < textWidthHeight.X)
                            {
                                Lines[thisLine] = Lines[thisLine] + " "; 
                                letterCounter += 1;
                            }
                            else //else start a new line and continue
                            {
                                Lines.Add("");
                                letterCounter += 1;
                            }
                        }
                    }
                    else //out of letters
                    {
                        isFinished = true;
                    }

                }
            }
            else //if finishNow 
            {
                do 
                {
                    int thisLine = Lines.Count - 1;

                    if (letterArray[letterCounter].ToString() != " ")
                    {
                        Lines[thisLine] = Lines[thisLine] + letterArray[letterCounter].ToString();
                        letterCounter += 1;
                    }
                    else
                    {
                        //build next word
                        BuildNextWord();

                        //measure the next word and see if it will fit (with a space)
                        nextWordMeasurement = font.MeasureString(nextWord + "  ");

                        //measure the line
                        lineMeasurement = font.MeasureString(Lines[thisLine]);

                        //if the word fits on the line, add add space and continue letter adding, add 1 to word counter
                        if ((nextWordMeasurement.X + lineMeasurement.X) < textWidthHeight.X)
                        {
                            Lines[thisLine] = Lines[thisLine] + " ";
                            letterCounter += 1;
                        }
                        else //else start a new line and continue
                        {
                            Lines.Add("");
                            letterCounter += 1;
                        }
                    }
                         
                }
                while (letterCounter < letterArray.Count());
                
                //out of letters
                isFinished = true;
            }

            return;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha)
        {
            spriteBatch.Begin();
           
            //draw the box
            spriteBatch.Draw(background, boxRectangle, backgroundColor*alpha);

            //test draw textbox
            //spriteBatch.Draw(background, new Rectangle((int)textPos.X,(int)textPos.Y,(int)textWidthHeight.X,(int)textWidthHeight.Y), backgroundColor * alpha);

            //draw each line
            for( int i = 0; i < Lines.Count ; i++)
            {
                //test draw textbox
                //spriteBatch.Draw(background, new Rectangle((int)textPos.X, (int)textPos.Y + (int)(font.MeasureString(Lines[i]).Y * i), (int)font.MeasureString(Lines[i]).X, (int)font.MeasureString(Lines[i]).Y), Color.Red * alpha);
                spriteBatch.DrawString(font,Lines[i],new Vector2 (textPos.X, textPos.Y + (font.MeasureString(Lines[i]).Y * i)),Color.White * alpha);
            }

            spriteBatch.End();
        }

        private void BuildNextWord()
        {
            int i = 1;
            nextWord = "";
            if (letterCounter + i >= letterArray.Count()) return;
            while (letterArray[letterCounter + i].ToString() != " ")
            {
                nextWord += letterArray[letterCounter + i].ToString();
                i += 1;
                if (letterCounter + i >= letterArray.Count()) return;
            }
            return;
        }

    }
}
