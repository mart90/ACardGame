using Microsoft.Xna.Framework.Graphics;
using System;

namespace ACardGame.UI
{
    public interface IRightClickable
    {
        public bool IsBeingRightClicked { get; set; }
        public Texture2D BeingRightClickedTexture { get; set; }
        public Action OnRightClickAction { get; set; }
    }
}
