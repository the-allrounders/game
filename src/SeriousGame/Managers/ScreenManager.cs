﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frogano.Screens;

namespace Frogano.Managers
{
    class ScreenManager
    {
        /// <summary>
        /// The dimensions of the game.
        /// </summary>
        public static Vector2 Dimensions = new Vector2(1280, 720);

        private static GameScreen currentScreen;
        /// <summary>
        /// The screen that gets Updated and Drawn. Also loads oldscreen.Unload() and screen.Load() if changed.
        /// </summary>
        public static GameScreen CurrentScreen
        {
            set
            {
                if (currentScreen != null) currentScreen.Unload();
                IsMouseVisible = true;
                value.Load();
                currentScreen = value;
            }
            get
            {
                return currentScreen;
            }
        }

        /// <summary>
        /// A reference to the Game object.
        /// </summary>
        public static Game1 Game;

        public static bool IsMouseVisible
        {
            get
            {
                return Game.IsMouseVisible;
            }
            set
            {
                Game.IsMouseVisible = value;
            }
        }

        /// <summary>
        /// Forwarder to the Update function of the CurrentScreen.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            CurrentScreen.Update(gameTime);
        }

        /// <summary>
        /// Forwarder to the Draw function of the CurrentScreen.
        /// </summary>
        public static void Draw(SpriteBatch spriteBatch)
        {
            CurrentScreen.Draw(spriteBatch);
        }


    }
}
