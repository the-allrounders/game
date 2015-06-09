﻿using Microsoft.Xna.Framework;
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

        public Scoreboard(int scr, bool dead)
        {
            score = scr;
            isDead = dead;
        }

        public void BuildPlayerName(Frog frog)
        {
            if (InputManager.IsPressing(Keys.Back))
            {
                frog.RemoveCharFromName();
            }
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
            string playerName = frog.PlayerName;
        }

        public void Update(Frog frog)
        {
            if (InputManager.IsPressing(Keys.Enter) || InputManager.IsClicking(new Rectangle((int)ScreenManager.Dimensions.X / 2 - 40, (int)ScreenManager.Dimensions.Y / 2, 100, 20)))
            {
                if (buttonIsSaveButton)
                {
                    LeaderboardScreen.SaveScore(frog.PlayerName, score);
                    buttonIsSaveButton = false;
                }
                else
                {
                    ScreenManager.CurrentScreen = new LeaderboardScreen();
                }
            }
            else if (InputManager.IsPressing(Keys.Space) ||
                     InputManager.IsClicking(new Rectangle((int)ScreenManager.Dimensions.X / 2 - 45,
                         (int)ScreenManager.Dimensions.Y / 2 + 35, 100, 20)))
            {
                ScreenManager.CurrentScreen = new JumpScreen();
            }
            BuildPlayerName(frog);
        }

        public void Draw(SpriteBatch spriteBatch, int offset, string playerName)
        {
            string winText = "Hoera, gewonnen! Je scoorde " + score + " punten";
            string loseText = "Helaas, GameOver! Je scoorde " + score + " punten";
            string text = isDead ? loseText : winText;
            spriteBatch.DrawString(FontManager.Verdana, text,
                new Vector2(ScreenManager.Dimensions.X/2 - 230, ScreenManager.Dimensions.Y/2 - 100), Color.White);
            spriteBatch.Draw(TextureManager.InputMedium,
                new Vector2(ScreenManager.Dimensions.X/2 - 100, ScreenManager.Dimensions.Y/2 - 50));
            //spriteBatch.Draw(TextureManager.Caret, new Vector2(ScreenManager.Dimensions.X / 2 - 90 + spriteFont.MeasureString(playerName).X, ScreenManager.Dimensions.Y / 2 - 40));
            spriteBatch.DrawString(FontManager.Verdana, playerName,
                new Vector2(ScreenManager.Dimensions.X/2 - 90, ScreenManager.Dimensions.Y/2 - 40), Color.Black);
            if (buttonIsSaveButton)
                spriteBatch.DrawString(FontManager.Verdana, "Opslaan",
                    new Vector2(ScreenManager.Dimensions.X/2 - 40, ScreenManager.Dimensions.Y/2), Color.White);
            else
                spriteBatch.DrawString(FontManager.Verdana, "Leaderboard",
                    new Vector2(ScreenManager.Dimensions.X/2 - 60, ScreenManager.Dimensions.Y/2), Color.White);
            spriteBatch.DrawString(FontManager.Verdana, "Opnieuw",
                new Vector2(ScreenManager.Dimensions.X/2 - 45, ScreenManager.Dimensions.Y/2 + 30), Color.White);
        }
    }
}