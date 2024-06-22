using Microsoft.Xna.Framework.Graphics;
using System;

namespace ACardGame.UI
{
    public interface ILeftClickable
    {
        public bool IsBeingLeftClicked { get; set; }
        public Texture2D BeingLeftClickedTexture { get; set; }
        public Action OnLeftClickAction { get; set; }
    }
}
