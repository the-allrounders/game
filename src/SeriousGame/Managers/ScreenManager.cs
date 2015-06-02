﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SeriousGame
{
    class ScreenManager
    {
        public Vector2 Dimensions { private set; get; }
        public float leftBound { private set; get; }
        public float rightBound { private set; get; }

        public ContentManager Content { private set; get; }

        private GameScreen _currentScreen;
        public GameScreen CurrentScreen
        {
            set
            {
                if(_currentScreen != null){
                    _currentScreen.Unload();
                }
                
                value.Load();
                _currentScreen = value;
            }
            get
            {
                return this._currentScreen;
            }
        }

        public Game1 Game;

        private static ScreenManager _instance;
        public static ScreenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScreenManager();
                }
                return _instance;
            }
        }

        public ScreenManager()
        {
            Dimensions = new Vector2(1280, 720);
            leftBound = 200;
            rightBound = Dimensions.X - 200;
        }
        
        public void Load(Game1 game)
        {
            CurrentScreen = new SplashScreen();
            Game = game;
        }

        public void UnloadContent()
        {
            
        }

        public void Update(GameTime gameTime)
        {
            CurrentScreen.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentScreen.Draw(spriteBatch);
        }


    }
}
