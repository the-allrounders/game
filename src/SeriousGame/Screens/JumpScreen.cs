﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Policy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Frogano.Managers;
using Frogano.Objects;

namespace Frogano.Screens
{
    class JumpScreen : GameScreen
    {
        private int offset;
        public static int GameHeight;
        private readonly List<Wall> walls = Wall.GenerateList();
        private readonly List<Platform> platforms = Platform.GenerateList();
        private readonly List<Obstacle> obstacles = Obstacle.GenerateList();
        private readonly List<Collectable> collectables = Fly.GenerateList();
        private readonly Frog frog = new Frog(new Vector2((ScreenManager.Dimensions.X / 2) - (TextureManager.Frog[SettingsManager.FrogType].Width / 2), ScreenManager.Dimensions.Y - TextureManager.Frog[SettingsManager.FrogType].Height), 5);
        private readonly Magma magma = new Magma(new Vector2(0, ScreenManager.Dimensions.Y));
        private Obstacle touchingObstacle = null;
        private bool gameEnded;
        private Scoreboard scoreboard;
        private bool controlInfoVisible = SettingsManager.ShowControlInfo;
        private bool dontShowControlInfoAgain;
        private bool isFrozen = false;
        private Timer timer;

        private bool wrong = false;
        private bool good = false;
        private string answer;
        private double waitTime;

        private int score;

        public static int Padding = 200;

        public override void Load()
        {
            SongManager.Play(Songs.SuperMarioHipHop);
        }

        public override void Update(GameTime gameTime)
        {
            #region Shortcuts

            // If user is pressing ESC, return to StartScreen
            if (InputManager.IsPressing(Keys.Escape))
            {
                ScreenManager.IsMouseVisible = true;
                ScreenManager.CurrentScreen = new StartScreen();
                return;
            }
            else if (!gameEnded && InputManager.IsPressing(Keys.P))
                isFrozen = !isFrozen;

            #endregion

            #region Info screen

            if (controlInfoVisible)
            {
                if (InputManager.IsPressing(Keys.Enter))
                {
                    dontShowControlInfoAgain = !dontShowControlInfoAgain;
                    SettingsManager.ShowControlInfo = !SettingsManager.ShowControlInfo;
                }
                if (InputManager.IsPressing(Keys.Space))
                {
                    if (timer == null)
                    {
                        timer = new Timer(2, gameTime);
                        controlInfoVisible = false;
                    }
                }
                return;
            }
            if (touchingObstacle == null && timer != null)
            {
                timer.Update(gameTime);
                if (!timer.waiting)
                {
                    timer = null;
                    return;
                }
            }

            #endregion

            #region Question screen

            // Show questionscreen if touching obstacle
            if (!wrong && touchingObstacle != null)
            {
                answer = "";
                if (InputManager.IsPressing(Keys.D1) || InputManager.IsPressing(Keys.NumPad1))
                    answer = touchingObstacle.popUp.questions[touchingObstacle.question].Answers[0];
                else if (InputManager.IsPressing(Keys.D2) || InputManager.IsPressing(Keys.NumPad2))
                    answer = touchingObstacle.popUp.questions[touchingObstacle.question].Answers[1];
                else if (InputManager.IsPressing(Keys.D3) || InputManager.IsPressing(Keys.NumPad3))
                    answer = touchingObstacle.popUp.questions[touchingObstacle.question].Answers[2];
                else if (InputManager.IsPressing(Keys.D4) || InputManager.IsPressing(Keys.NumPad4))
                    answer = touchingObstacle.popUp.questions[touchingObstacle.question].Answers[3];

                if (answer != string.Empty)
                {
                    bool right = touchingObstacle.CheckAnswer(answer);
                    touchingObstacle.FinishedQuestion();
                    if (right)
                    {
                        score += 1000;
                        if (SettingsManager.Difficulty != 3 && frog.Lives < 3)
                            frog.Lives++;
                        good = true;
                    }
                    else
                    {
                        score -= 1000;
                        wrong = true;
                        frog.Lives--;
                    }
                    waitTime = gameTime.TotalGameTime.TotalSeconds;
                    frog.Jump();
                }
            }
            else if (timer == null && !wrong && (!gameEnded || gameEnded && frog.IsDead) && !isFrozen)
                // Make the magma rise
                magma.Rise(offset);

            // Set wrong and good to false after 3 seconds
            if (good && gameTime.TotalGameTime.TotalSeconds >= waitTime + 3)
                good = false;

            #endregion

            ScreenManager.IsMouseVisible = gameEnded;

            if (touchingObstacle != null)
            {
                if (frog.Lives <= 0 && InputManager.IsPressing(Keys.Space))
                {
                    good = false;
                    wrong = false;
                    touchingObstacle = null;
                }
                else
                {
                    if ((InputManager.IsPressing(Keys.Space) && wrong) || good)
                    {
                        if (timer == null)
                            timer = new Timer(2, gameTime);
                        good = false;
                        wrong = false;
                    }
                    if (timer != null)
                    {
                        timer.Update(gameTime);
                        if (!timer.waiting)
                        {
                            timer = null;
                            touchingObstacle = null;
                        }
                    }
                }
                
            }

            if (timer == null && !gameEnded && touchingObstacle == null && !isFrozen)
            {
                #region Game actively running

                // Check if Frog touches obstacle
                foreach (
                    Obstacle obstacle in
                        obstacles.Where(
                            obstacle =>
                                obstacle.IsInViewport(offset) && frog.IsJumpingOnObstacle(obstacle) &&
                                !obstacle.IsDone()))
                    touchingObstacle = obstacle;

                // If user is pressing Left, go left. Same for Right.
                if (InputManager.IsPressing(Keys.Left, false) || InputManager.IsPressing(Keys.A, false))
                    frog.Left();
                else if (InputManager.IsPressing(Keys.Right, false) || InputManager.IsPressing(Keys.D, false))
                    frog.Right();


                // Check if jumping on platform
                if (platforms.Any(platform => platform.IsInViewport(offset) && frog.IsJumpingOn(platform)))
                {
                    frog.Jump();
                    SoundManager.Play(Sounds.Jump);
                }

                // Check if frog is catching any collectables
                foreach (Collectable collectable in collectables.Where(collectable => !collectable.IsDone && collectable.IsInViewport(offset)))
                {
                    collectable.Update(gameTime, offset);
                    if (!collectable.IsCatching(frog)) continue;
                    score += collectable.CollectableScoreWorth;
                    SoundManager.Play(Sounds.Coin);
                    collectable.IsDone = true;
                }

                // Apply gravity to Frog
                frog.ApplyGravity(gameTime);

                if (magma.IsTouchingFrog(frog))
                {
                    if (!frog.StealthMode)
                    {
                        if (frog.Lives > 1)
                        {
                            frog.StealthMode = true;
                            frog.TimeOfStealthMode = gameTime.TotalGameTime.TotalMilliseconds;
                        }
                        frog.Lives--;
                    }
                    frog.Jump();
                }

                if (frog.StealthMode)
                {
                    if (gameTime.TotalGameTime.TotalMilliseconds < frog.TimeOfStealthMode + 1500)
                    {
                        if ((gameTime.TotalGameTime.Milliseconds >= 0 && gameTime.TotalGameTime.Milliseconds < 125) || (gameTime.TotalGameTime.Milliseconds >= 250 && gameTime.TotalGameTime.Milliseconds <= 375) || (gameTime.TotalGameTime.Milliseconds >= 500 && gameTime.TotalGameTime.Milliseconds < 625) || (gameTime.TotalGameTime.Milliseconds >= 750 && gameTime.TotalGameTime.Milliseconds <= 875))
                            frog.IsVisible = true;
                        else
                            frog.IsVisible = false;
                    }
                    else
                        frog.StealthMode = false;
                }
                else
                    frog.IsVisible = true;

                //Check if frog is out of screen
                if (frog.BoundingBox.Top + offset - ScreenManager.Dimensions.Y > 0 || frog.Lives <= 0)
                {
                    frog.Die();
                    gameEnded = true;
                    SoundManager.Play(Sounds.Death);
                    SongManager.Stop();
                    scoreboard = new Scoreboard(score, frog.IsDead);
                }

                // Calculate new offset
                int newOffset = (int)ScreenManager.Dimensions.Y - frog.BoundingBox.Bottom - 500;

                // If new offset is bigger, apply
                if (newOffset > offset)
                {
                    decimal addPoints = (newOffset - offset) / 10;
                    score += (int)Math.Ceiling(addPoints);
                    offset = newOffset;
                }

                // Check if game is won
                if (offset > GameHeight + 400)
                {
                    gameEnded = true;
                    scoreboard = new Scoreboard(score, frog.IsDead);
                }

                #endregion
            }

            #region Scoreboard

            if (gameEnded)
                scoreboard.Update(frog, gameTime);

            #endregion

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw platforms
            foreach (Platform platform in platforms.Where(platform => platform.IsInViewport(offset)))
                platform.Draw(spriteBatch, offset);

            // Draw collectables
            foreach (
                Collectable collectable in
                    collectables.Where(collectable => collectable.IsInViewport(offset) && !collectable.IsDone))
                collectable.Draw(spriteBatch, offset);

            // Draw obstacles
            foreach (
                Obstacle obstacle in obstacles.Where(obstacle => obstacle.IsInViewport(offset) && !obstacle.IsDone()))
            {
                obstacle.Draw(spriteBatch, offset);
            }

            // Draw frog
            frog.Draw(spriteBatch, offset);

            // Draw magma
            magma.Draw(spriteBatch, offset);

            // Show feedback
            if (wrong)
            {
                spriteBatch.DrawString(
                    FontManager.MarkerFelt100, 
                    "FOUT", 
                    new Vector2(ScreenManager.Dimensions.X/2 - FontManager.MarkerFelt100.MeasureString("FOUT").X/2, 560), 
                    Color.Red
                );
                touchingObstacle.DrawFeedback(answer, spriteBatch);
            }
            else if (good)
                spriteBatch.DrawString(
                    FontManager.MarkerFelt100, 
                    "GOED",
                    new Vector2(ScreenManager.Dimensions.X / 2 - FontManager.MarkerFelt100.MeasureString("GOED").X / 2, ScreenManager.Dimensions.Y / 2 - FontManager.MarkerFelt100.MeasureString("GOED").Y), 
                    Color.Green
                );

            // Draw walls
            foreach (Wall wall in walls.Where(wall => wall.IsInViewport(offset)))
                wall.Draw(spriteBatch, offset);


            // Draw question popup
            foreach (Obstacle obstacle in obstacles.Where(obstacle => obstacle.IsInViewport(offset) && !obstacle.IsDone()).Where(obstacle => frog.IsJumpingOnObstacle(obstacle)))
                obstacle.DrawQuestion(spriteBatch);

            // Draw scorescreen of frog is dead
            if (gameEnded)
                scoreboard.Draw(spriteBatch, offset, frog.PlayerName);
            // If the frog is alive, draw the score
            else
            {
                string text = "Score: " + score;
                spriteBatch.DrawString(FontManager.MarkerFelt12, text, new Vector2(ScreenManager.Dimensions.X - FontManager.MarkerFelt12.MeasureString(text).X - 20, TextureManager.Heart.Height + 10),
                    Color.White);
                for (int i = 1; i <= frog.Lives; i++)
                    spriteBatch.Draw(TextureManager.Heart, new Vector2(ScreenManager.Dimensions.X - 5 - TextureManager.Heart.Width * i, 5));
            }

            if (controlInfoVisible)
            {
                Texture2D controlInfoTexture;
                GameTime gameTime = ScreenManager.Game.gameTime;
                if ((gameTime.TotalGameTime.Milliseconds >= 0 && gameTime.TotalGameTime.Milliseconds < 250) || (gameTime.TotalGameTime.Milliseconds >= 500 && gameTime.TotalGameTime.Milliseconds <= 750))
                    controlInfoTexture = TextureManager.ControlInfoArrows;
                else
                    controlInfoTexture = TextureManager.ControlInfoWASD;

                spriteBatch.Draw(controlInfoTexture, new Vector2(ScreenManager.Dimensions.X / 2 - TextureManager.ControlInfoArrows.Width / 2, ScreenManager.Dimensions.Y / 2 - TextureManager.ControlInfoArrows.Height / 2));
            }

            if (timer != null)
            {
                timer.Draw(spriteBatch);
            }

            if (isFrozen)
                spriteBatch.DrawString(
                    FontManager.MarkerFelt100,
                    "| |",
                    new Vector2(ScreenManager.Dimensions.X / 2 - FontManager.MarkerFelt100.MeasureString("| |").X / 2, ScreenManager.Dimensions.Y / 2 - FontManager.MarkerFelt100.MeasureString("| |").Y / 2),
                    Color.White
                );
        }
    }
}
