﻿using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ACardGame.UI
{
    public class AssetManager
    {
        private ContentManager _content;

        public AssetManager(ContentManager content)
        {
            _content = content;
        }

        public Texture2D LoadTexture(string filename)
        {
            return _content.Load<Texture2D>($"graphics/{filename}");
        }

        public SpriteFont LoadFont(string filename)
        {
            return _content.Load<SpriteFont>($"fonts/{filename}");
        }

        public SoundEffect LoadSoundEffect(string filename)
        {
            return _content.Load<SoundEffect>($"soundEffects/{filename}");
        }

        public Texture2D LoadCardTexture(string cardName)
        {
            return _content.Load<Texture2D>($"graphics/cards/{cardName.ToLower()}");
        }
    }
}
