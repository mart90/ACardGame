using Microsoft.Xna.Framework.Graphics;

namespace ACardGame.UI
{
    public interface ISelectable
    {
        public bool IsSelected { get; set; }
        public Texture2D SelectedTexture { get; set; }
    }
}
