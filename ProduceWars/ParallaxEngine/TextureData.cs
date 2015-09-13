using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ParallaxEngine
{
    //this class holds the data needed for access to a single texture and is used in List form by the LevelTextures class

    
    public class TextureData
    {
        #region DECLARATIONS
        private Texture2D texture;  //get/set
        private string filePath;    //get/set
        private string fileName;    //get/set
        
        //used by sprites to identify which tilesheet or sprite sheet it is sourced from.
        //is assigned on level/map load/save so the ID is unique to each set of level data
        private int textureID = -1; //get/set
        private string spriteType = "";

        //used to calculate the source rectangle from a spritesheet
        //all sprite textures on a sprite sheet must have the same width and height 
        private bool isTiled = false; //get/set
        private bool isAnimated = false; //get/set
        private int tileWidth = 0;    //get/set
        private int tileHeight = 0;   //get/set

        #endregion

        #region CONSTRUCTOR
        public TextureData() { }
        public TextureData(string _filePath, string _fileName, int _textureID, string _spriteType, bool _isTiled, bool _isAnimated, int _tileWidth, int _tileHeight)
        {
            filePath = _filePath;
            fileName = _fileName;
            textureID = _textureID;
            spriteType = _spriteType;
            isTiled = _isTiled;
            isAnimated = _isAnimated;
            tileWidth = _tileWidth;
            tileHeight = _tileHeight;
        }
        #endregion

        #region READ ONLY PROPERTIES
        public int Width
        {
            get { return texture.Width; }
        }

        public int Height
        {
            get { return texture.Height; }
        }

        public int SpritesInRow
        {
            get
            {
                 return (this.texture.Width / tileWidth);
            }
        }

        public int SpritesInColumn
        {
            get 
            {
                return (this.texture.Height / tileHeight);
            }
        }
        #endregion

        #region PROPERTIES
        public Texture2D Texture
        {
            get { return this.texture; }
            set 
            {
                if (value != null)
                {
                    this.texture = value;
                    if (!isTiled)
                    {
                        tileWidth = this.texture.Width;
                        tileHeight = this.texture.Height;
                    }
                }
            } 
        }

        public string FilePath
        {
            get { return this.filePath; }
            set { this.filePath = value; }
        }

        public string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; }
        }

        public int TextureID
        {
            get { return this.textureID; }
            set { this.textureID = value; }
        }

        public string SpriteType
        {
            get { return this.spriteType; }
            set { this.spriteType = value; }
        }
        public bool IsTiled
        {
            get { return this.isTiled; }
            set { this.isTiled = value; }
        }

        public bool IsAnimated
        {
            get { return this.isAnimated; }
            set { this.isAnimated = value; }
        }

        public int TileWidth
        {
            get { return this.tileWidth; }
            set { this.tileWidth = value; }
        }

        public int TileHeight
        {
            get { return this.tileHeight; }
            set { this.tileHeight = value; }
        }


        #endregion
    }





}
