﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class UiContainer : UiElement
    {
        protected readonly List<UiElement> Children;

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

        public void SetCursorY(double y)
        {
            Cursor = new PointDouble(Cursor.X, y);
        }

        public void SetCursorX(double x)
        {
            Cursor = new PointDouble(x, Cursor.Y);
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

        protected void ClearChildren()
        {
            Children.Clear();
        }

        protected IEnumerable<UiElement> FilterChildren(Func<UiElement, bool> filter)
        {
            return Children.Where(filter);
        }

        protected IEnumerable<T> ChildrenOfType<T>()
        {
            return Children.OfType<T>();
        }

        public void GoRight(bool moveCursor = true)
        {
            NewDirection(Direction.Right, moveCursor);
        }

        public void GoDown(bool moveCursor = true)
        {
            NewDirection(Direction.Down, moveCursor);
        }

        public void GoLeft(bool moveCursor = true)
        {
            NewDirection(Direction.Left, moveCursor);
        }

        public void GoUp(bool moveCursor = true)
        {
            NewDirection(Direction.Up, moveCursor);
        }

        public void NewDirection(Direction direction, bool moveCursor = true)
        {
            Direction = direction;

            if (Children.Count != 0 && moveCursor)
            {
                SetCursor();
            }
        }

        public virtual void LeftClick(Point position)
        {
            var focusedTextbox = FocusedTextbox();

            if (focusedTextbox != null)
            {
                focusedTextbox.HasFocus = false;
            }

            var child = GetChildByPosition(position);

            if (child == null)
            {
                return;
            }

            if (child is ILeftClickable clickableChild && clickableChild.OnLeftClickAction != null)
            {
                if (child is Button)
                {
                    AssetManager.PlaySoundEffect("playcard");
                }

                clickableChild.OnLeftClickAction();
            }
            else if (child is UiContainer childContainer)
            {
                childContainer.LeftClick(position);
            }
        }

        public virtual void RightClick(Point position)
        {
            var child = GetChildByPosition(position);

            if (child == null)
            {
                return;
            }

            if (child is IRightClickable clickableChild && clickableChild.OnRightClickAction != null)
            {
                clickableChild.OnRightClickAction();
            }
            else if (child is UiContainer childContainer)
            {
                childContainer.RightClick(position);
            }
        }

        public virtual IHoverable Hover(Point position)
        {
            var child = GetChildByPosition(position);

            if (child is UiContainer childContainer)
            {
                return childContainer.Hover(position);
            }
            else if (child is IHoverable hoverableChild) 
            {
                return hoverableChild;
            }

            return this is IHoverable thisHoverable ? thisHoverable : null;
        }

        public virtual void ScrollUp(Point position)
        {
            var child = GetChildByPosition(position);

            if (child == null)
            {
                return;
            }

            if (child is UiContainer childContainer)
            {
                childContainer.ScrollUp(position);
            }
        }

        public virtual void ScrollDown(Point position) 
        {
            var child = GetChildByPosition(position);

            if (child == null)
            {
                return;
            }

            if (child is UiContainer childContainer)
            {
                childContainer.ScrollDown(position);
            }
        }

        protected UiElement GetChildByPosition(Point position)
        {
            return Children
                .Where(e => e.AbsoluteLocation.Contains(position) && e.IsVisible
                    && !(e is CardContainer cc && cc.Card == null))
                .OrderByDescending(e => e.DrawLayer)
                .FirstOrDefault();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
            {
                return;
            }

            base.Draw(spriteBatch);

            foreach (var child in Children.OrderBy(e => e.DrawLayer))
            {
                int childX = (int)(AbsoluteLocation.X + child.RelativeLocationInParent.X * AbsoluteLocation.Width / 100);
                int childY = (int)(AbsoluteLocation.Y + child.RelativeLocationInParent.Y * AbsoluteLocation.Height / 100);

                int childWidth = (int)(AbsoluteLocation.Width * (child.RelativeSizeInParentX ?? child.RelativeSizeInParentY * child.AspectRatio / AspectRatio) / 100);
                int childHeight = (int)(AbsoluteLocation.Height * (child.RelativeSizeInParentY ?? child.RelativeSizeInParentX / child.AspectRatio * AspectRatio) / 100);

                child.AbsoluteLocation = new Rectangle(childX, childY, childWidth, childHeight);

                child.Draw(spriteBatch);
            }
        }

        protected void AddCardContainersRecursive(List<CardContainer> cards)
        {
            var cardContainers = Children
                .Where(e => e is CardContainer)
                .Cast<CardContainer>();

            foreach (CardContainer c in cardContainers)
            {
                if (!cards.Contains(c))
                {
                    cards.Add(c);
                }
            }

            var uiContainers = Children
                .Where(e => e is UiContainer && e is not CardContainer)
                .Cast<UiContainer>();

            foreach (UiContainer child in uiContainers)
            {
                child.AddCardContainersRecursive(cards);
            }
        }

        private TextInput FocusedTextbox()
        {
            return (TextInput)Children.SingleOrDefault(e => e is TextInput tb && tb.HasFocus);
        }

        public virtual void ReceiveKeyboardInput(TextInputEventArgs args)
        {
            var focusedTextBox = FocusedTextbox();

            if (focusedTextBox == null)
            {
                return;
            }

            if (args.Key == Microsoft.Xna.Framework.Input.Keys.Back && focusedTextBox.Text != "")
            {
                focusedTextBox.Text = focusedTextBox.Text.Substring(0, focusedTextBox.Text.Length - 1);
            }
            else if (char.IsLetterOrDigit(args.Character))
            {
                focusedTextBox.Text += args.Character;
            }
        }
    }
}
