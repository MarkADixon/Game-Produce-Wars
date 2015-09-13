using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParallaxEngine
{
    public class LevelData
    {
        //loaded from leveldata file when game starts
        public int world = 9;
        public int level = 9;
        public string name="null";
        public int bronze=97;
        public int silver=98;
        public int gold=99;
        public bool safety = false;
        public bool isIntro = false;
        public List<string> textBox = new List<string>();
        public List<int> speaker = new List<int>();
        public List<Vector2> cursor = new List<Vector2>();
        public List<Vector2> look = new List<Vector2>();
        //loaded from savegame file when game starts
        //save these variables 
        public bool starCollected = false;
        public bool unlocked = false;
        public int shots = 0;
        public int bestScore = 0;

        public LevelData()
        {
        }

        public LevelData(int _world, int _level, string _name, int _bronze, int _silver, int _gold, bool _safety, bool _isIntro, List<string> _textBox, List<int> _speaker, List<Vector2> _cursor, List<Vector2> _look)
        {
            this.world = _world; 



            this.level = _level;
            this.name = _name;
            this.bronze = _bronze;
            this.silver = _silver;
            this.gold = _gold;
            this.safety = _safety;
            this.isIntro = _isIntro;
            this.textBox = _textBox;
            this.speaker = _speaker;
            this.cursor = _cursor;
            this.look = _look;
            return;
        }

        public bool IsBronze()
        {
            if (shots != 0 && shots <= bronze) return true;
            else return false;
        }

        public bool IsSilver()
        {
            if (shots != 0 && shots <= silver) return true;
            else return false;
        }

        public bool IsGold()
        {
            if (shots != 0 && shots <= gold) return true;
            else return false;
        }


    }
}
