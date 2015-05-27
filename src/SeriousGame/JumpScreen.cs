﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeriousGame
{
    class JumpScreen : GameScreen
    {
        int offset = 0;
        
        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            offset += 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.Instance.Vlieg, new Vector2(0, 0+offset));
            spriteBatch.Draw(TextureManager.Instance.Vlieg, new Vector2(1050, 650 + offset));

        }
    }
}
