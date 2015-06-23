﻿using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SeriousGame.Managers;
using SeriousGame.Screens;

namespace SeriousGame.Objects
{
    class Scoreboard
    {
        private int score;
        private bool buttonIsSaveButton = true;
        private bool isDead;
        private bool caretVisible = true;
        private float opacity;

        public Scoreboard(int scr, bool dead)
        {
            score = scr;
            isDead = dead;
        }

        public void BuildPlayerName(Frog frog)
        {
            if (InputManager.IsPressing(Keys.Back))
                frog.RemoveCharFromName();
            else if (InputManager.IsPressing(Keys.Space))
                frog.AddCharToName(Keys.Space);
            else
            {
                for (Keys key = Keys.A; key <= Keys.Z; key++)
                {
                    if (InputManager.IsPressing(key))
                    {
                        frog.AddCharToName(key);
                    }
                }
            }
            if (!isDead && opacity < 1)
                opacity += 0.1f;
        }

        public void Update(Frog frog, GameTime gameTime)
        {
            if (InputManager.IsPressing(Keys.Enter) ||
                InputManager.IsClicking(new Rectangle((int) ScreenManager.Dimensions.X/2 - 40,
                    (int) ScreenManager.Dimensions.Y/2, 100, 20)))
            {
                if (buttonIsSaveButton)
                {
                    LeaderboardScreen.SaveScore(frog.PlayerName, score);
                    buttonIsSaveButton = false;
                }
                else
                    ScreenManager.CurrentScreen = new LeaderboardScreen();
            }
            else if (InputManager.IsClicking(new Rectangle((int) ScreenManager.Dimensions.X/2 - 45,
                (int) ScreenManager.Dimensions.Y/2 + 35, 100, 20)))
            {
                ScreenManager.CurrentScreen = new JumpScreen();
            }
            else
                BuildPlayerName(frog);
            if (gameTime.TotalGameTime.Milliseconds > 500 && gameTime.TotalGameTime.Milliseconds <= 999)
                caretVisible = true;
            else
                caretVisible = false;
        }

        public void Draw(SpriteBatch spriteBatch, int offset, string playerName)
        {
            if (!isDead)
                spriteBatch.Draw(TextureManager.WonGameBackground, new Vector2(0, 0), Color.White * opacity);
            spriteBatch.Draw(TextureManager.QuestionBox, new Vector2(ScreenManager.Dimensions.X / 2 - TextureManager.QuestionBox.Width / 2, ScreenManager.Dimensions.Y / 2 - TextureManager.QuestionBox.Height / 2));
            string winText = "Hoera, gewonnen! Je scoorde " + score + " punten";
            string loseText = "Helaas, game over! Je scoorde " + score + " punten";
            string text = isDead ? loseText : winText;
            spriteBatch.DrawString(FontManager.MarkerFelt12, text,
                new Vector2(ScreenManager.Dimensions.X/2 - FontManager.MarkerFelt12.MeasureString(text).X/2,
                    ScreenManager.Dimensions.Y/2 - 100), Color.Black);
            spriteBatch.Draw(TextureManager.InputMedium,
                new Vector2(ScreenManager.Dimensions.X/2 - 100, ScreenManager.Dimensions.Y/2 - 50));
            if (caretVisible)
            {
                float caretDistance = playerName == "<naam>" ? 0 : FontManager.MarkerFelt12.MeasureString(playerName).X;
                spriteBatch.Draw(TextureManager.Caret,
                    new Vector2(ScreenManager.Dimensions.X/2 - 90 + caretDistance + 1, ScreenManager.Dimensions.Y/2 - 40));
            }
            Color nameColor = playerName == "<naam>" ? Color.Gray : Color.Black;
            spriteBatch.DrawString(FontManager.MarkerFelt12, playerName,
                new Vector2(ScreenManager.Dimensions.X/2 - 90, ScreenManager.Dimensions.Y/2 - 40), nameColor);
            if (buttonIsSaveButton)
                spriteBatch.DrawString(FontManager.MarkerFelt12, "Opslaan",
                    new Vector2(ScreenManager.Dimensions.X/2 - FontManager.MarkerFelt12.MeasureString("Opslaan").X/2,
                        ScreenManager.Dimensions.Y/2), Color.Black);
            else
                spriteBatch.DrawString(FontManager.MarkerFelt12, "Ranglijst",
                    new Vector2(ScreenManager.Dimensions.X/2 - FontManager.MarkerFelt12.MeasureString("Ranglijst").X/2,
                        ScreenManager.Dimensions.Y/2), Color.Black);
            spriteBatch.DrawString(FontManager.MarkerFelt12, "Opnieuw",
                new Vector2(ScreenManager.Dimensions.X/2 - FontManager.MarkerFelt12.MeasureString("Opnieuw").X/2,
                    ScreenManager.Dimensions.Y/2 + 30), Color.Black);
        }
    }
}
