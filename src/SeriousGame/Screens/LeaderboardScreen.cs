﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Frogano.Managers;

namespace Frogano.Screens
{
    class LeaderboardScreen : GameScreen
    {
        private bool showPlayButton;
        
        public LeaderboardScreen(bool showPlayButton = false)
        {
            this.showPlayButton = showPlayButton;
        }
        
        private string[] values;

        public override void Load()
        {
            string pathOfFile = "../../../Leaderboards/leaderboard" + SettingsManager.Difficulty + ".txt";
            values = File.ReadAllLines(pathOfFile);
        }

        public static void SaveScore(string playerName, int score)
        {
            string pathOfFile = "../../../Leaderboards/leaderboard" + SettingsManager.Difficulty + ".txt";
            string[] values = File.ReadAllLines(pathOfFile);
            List<string> scores = new List<string>();
            bool added = false;
            foreach (string t in values)
            {
                string[] sc = t.Split(',');
                if (!added && score > Convert.ToInt32(sc[1]))
                {
                    scores.Add(playerName + ", " + score);
                    added = true;
                }
                scores.Add(t);
            }
            if (!added)
                scores.Add(playerName + ", " + score);
            File.WriteAllLines(pathOfFile, scores);
        }

        public override void Update(GameTime gameTime)
        {
            // If user is pressing ESC, return to StartScreen
            if (InputManager.IsPressing(Keys.Escape) || 
                InputManager.IsClicking(new Rectangle(24, 14, TextureManager.SettingsArrow[0].Width, TextureManager.SettingsArrow[0].Height)))
            {
                ScreenManager.CurrentScreen = new StartScreen();
            }

            // If playagain
            if (
                showPlayButton &&
                InputManager.IsClicking(
                    new Rectangle(
                        (int) ScreenManager.Dimensions.X/2 -
                        (int) FontManager.MarkerFelt12.MeasureString("Speel opnieuw").X/2,
                        (int) ScreenManager.Dimensions.Y/2 + 200,
                        (int) FontManager.MarkerFelt12.MeasureString("Speel opnieuw").X,
                        (int) FontManager.MarkerFelt12.MeasureString("Speel opnieuw").Y))
                )
            {
                ScreenManager.CurrentScreen = new JumpScreen();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.CreditsScreen, new Vector2(0, 0));
            for (int i = 0; i < values.Length && i < 10; i++)
            {
                string[] score = values[i].Split(',');
                string text = (i + 1) + ". " + score[0] + ": " + score[1];
                spriteBatch.DrawString(FontManager.MarkerFelt12, text, new Vector2(ScreenManager.Dimensions.X / 2 - 150, 100 + (i * 40)), Color.White);
            }
            if(showPlayButton)
                spriteBatch.DrawString(FontManager.MarkerFelt12, "Speel opnieuw", new Vector2(ScreenManager.Dimensions.X / 2 - FontManager.MarkerFelt12.MeasureString("Terug").X / 2, ScreenManager.Dimensions.Y / 2 + 200), Color.White);
            
            // Draw back button
            spriteBatch.Draw(
                InputManager.IsHovering(new Rectangle(24, 14, TextureManager.SettingsArrow[0].Width, TextureManager.SettingsArrow[0].Height))
                ? TextureManager.SettingsArrow[1]
                : TextureManager.SettingsArrow[0],
                new Vector2(24, 14)
                );
        }
    }
}
