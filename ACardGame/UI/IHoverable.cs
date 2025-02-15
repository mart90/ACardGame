﻿using Microsoft.Xna.Framework.Graphics;
using System;

namespace ACardGame.UI
{
    public interface IHoverable
    {
        public bool IsHovered { get; set; }
        public Texture2D HoverTexture { get; set; }
        public string ToolTipOnHover { get; set; }
        public Action OnHoverAction { get; set; }
    }
}
