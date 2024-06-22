using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class UiContainer : UiElement
    {
        public List<UiElement> Children { get; }

        public PointDouble Cursor { get; set; }

        protected AssetManager AssetManager { get; }

        /// <summary>
        /// The direction of new children being placed
        /// </summary>
        private Direction Direction;

        public UiContainer(AssetManager assetManager, double aspectRatio, double relativeSizeInParent, bool sizeExpressedInX)
            : base(aspectRatio, relativeSizeInParent, sizeExpressedInX)
        {
            AssetManager = assetManager;
            AspectRatio = aspectRatio;

            Children = new List<UiElement>();
            Direction = Direction.Right;
            Cursor = new PointDouble(0, 0);
        }

        public void AddChild(UiElement element)
        {
            if (Direction == Direction.Left)
            {
                // The cursor is at the top right corner of our new element
                Cursor.X -= element.RelativeSizeInParentX ?? (double)element.RelativeSizeInParentY * element.AspectRatio / AspectRatio;
            }
            else if (Direction == Direction.Up)
            {
                // The cursor is at the bottom left corner of our new element
                Cursor.Y -= element.RelativeSizeInParentY ?? (double)element.RelativeSizeInParentX / element.AspectRatio * AspectRatio;
            }

            element.RelativeLocationInParent = Cursor;
            Children.Add(element);
            SetCursor();
        }

        public void AddSpacing(double spacingPct)
        {
            if (Direction == Direction.Right)
            {
                Cursor.X += spacingPct;
            }
            else if (Direction == Direction.Down)
            {
                Cursor.Y += spacingPct;
            }
            else if (Direction == Direction.Left)
            {
                Cursor.X -= spacingPct;
            }
            else if (Direction == Direction.Up)
            {
                Cursor.Y -= spacingPct;
            }
        }

        public void SetCursor(double x, double y)
        {
            Cursor = new PointDouble(x, y);
        }

        private void SetCursor()
        {
            var lastChild = Children.Last();

            double x = lastChild.RelativeLocationInParent.X;
            double y = lastChild.RelativeLocationInParent.Y;

            if (Direction == Direction.Right)
            {
                // Put cursor in the top right corner of the last child
                x = lastChild.RelativeLocationInParent.X + (double)(lastChild.RelativeSizeInParentX ?? lastChild.RelativeSizeInParentY * lastChild.AspectRatio / AspectRatio);
            }
            else if (Direction == Direction.Down)
            {
                // Put cursor in the bottom left corner of the last child
                y = lastChild.RelativeLocationInParent.Y + (double)(lastChild.RelativeSizeInParentY ?? lastChild.RelativeSizeInParentX / lastChild.AspectRatio * AspectRatio);
            }

            Cursor = new PointDouble(x, y);
        }

        public void GoRight()
        {
            NewDirection(Direction.Right);
        }

        public void GoDown()
        {
            NewDirection(Direction.Down);
        }

        public void GoLeft()
        {
            NewDirection(Direction.Left);
        }

        public void GoUp()
        {
            NewDirection(Direction.Up);
        }

        public void NewDirection(Direction direction)
        {
            Direction = direction;

            if (Children.Any())
            {
                SetCursor();
            }
        }

        public virtual void LeftClick(Point position)
        {
            var child = Children.Where(e => e.AbsoluteLocation.Contains(position)).FirstOrDefault();

            if (child == null || !child.IsVisible)
            {
                return;
            }

            if (child is ILeftClickable clickableChild && clickableChild.OnLeftClickAction != null)
            {
                clickableChild.OnLeftClickAction();
            }
            else if (child is UiContainer childContainer)
            {
                childContainer.LeftClick(position);
            }
        }

        public virtual void RightClick(Point position)
        {
            var child = Children.Where(e => e.AbsoluteLocation.Contains(position)).FirstOrDefault();

            if (child == null || !child.IsVisible)
            {
                return;
            }

            // TODO
        }

        public virtual void Hover(Point position)
        {
            var child = Children.Where(e => e.AbsoluteLocation.Contains(position)).FirstOrDefault();

            if (child == null || !child.IsVisible)
            {
                return;
            }

            // TODO
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
            {
                return;
            }

            base.Draw(spriteBatch);

            foreach (var child in Children)
            {
                int childX = (int)(AbsoluteLocation.X + child.RelativeLocationInParent.X * AbsoluteLocation.Width / 100);
                int childY = (int)(AbsoluteLocation.Y + child.RelativeLocationInParent.Y * AbsoluteLocation.Height / 100);

                int childWidth = (int)(AbsoluteLocation.Width * (child.RelativeSizeInParentX ?? child.RelativeSizeInParentY * child.AspectRatio / AspectRatio) / 100);
                int childHeight = (int)(AbsoluteLocation.Height * (child.RelativeSizeInParentY ?? child.RelativeSizeInParentX / child.AspectRatio * AspectRatio) / 100);

                child.AbsoluteLocation = new Rectangle(childX, childY, childWidth, childHeight);

                child.Draw(spriteBatch);
            }
        }
    }
}
