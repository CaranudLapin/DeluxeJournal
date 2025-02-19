﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using DeluxeJournal.Task;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Menus.Components
{
    public class TaskParameterDropDown : DropDownComponent
    {
        private readonly Texture2D? _backgroundTexture;
        private readonly IList<KeyValuePair<Texture2D, Rectangle>> _options;

        /// <summary>Get the selected option as an item quality.</summary>
        public int Quality => SelectedOption == 3 ? 4 : SelectedOption;

        public TaskParameter TaskParameter { get; set; }

        public TaskParameterDropDown(TaskParameter parameter, IList<KeyValuePair<Texture2D, Rectangle>> options, Rectangle bounds)
            : base(Enumerable.Repeat(string.Empty, options.Count), bounds, parameter.Attribute.Name, true)
        {
            if (parameter.Value is not int value)
            {
                throw new ArgumentException($"{nameof(TaskParameterDropDown)} must have a parameter of type int.");
            }
            else
            {
                SelectedOption = value;
            }

            _backgroundTexture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
            _options = options.ToList();
            TaskParameter = parameter;
        }

        public override void LeftClickReleased(int x, int y)
        {
            base.LeftClickReleased(x, y);

            if (TaskParameter.Attribute.Tag == TaskParameterTag.Quality)
            {
                TaskParameter.TrySetValue(Quality);
            }
            else
            {
                TaskParameter.TrySetValue(SelectedOption);
            }
        }

        protected override void DrawBackground(SpriteBatch b, Rectangle bgBounds, bool dropDown)
        {
            if (_backgroundTexture == null)
            {
                base.DrawBackground(b, bgBounds, dropDown);
                return;
            }

            bgBounds.X -= 4;
            bgBounds.Width += 8;

            if (dropDown)
            {
                bgBounds.Height += 8;

                // left border
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X, bgBounds.Y, 12, bgBounds.Height - 12),
                    new Rectangle(4, 12, 12, 24),
                    Color.White);

                // right border
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + bgBounds.Width - 12, bgBounds.Y, 12, bgBounds.Height - 12),
                    new Rectangle(_backgroundTexture.Bounds.Width - 12, 12, 12, 24),
                    Color.White);

                // bottom-left corner
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X, bgBounds.Y + bgBounds.Height - 12, 12, 12),
                    new Rectangle(4, 36, 12, 12),
                    Color.White);

                // bottom-center border
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + 12, bgBounds.Y + bgBounds.Height - 12, bgBounds.Width - 24, 12),
                    new Rectangle(16, 36, 4, 12),
                    Color.White);

                // bottom-right corner
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + bgBounds.Width - 12, bgBounds.Y + bgBounds.Height - 12, 12, 12),
                    new Rectangle(_backgroundTexture.Bounds.Width - 12, 36, 12, 12),
                    Color.White);

                // fill
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + 12, bgBounds.Y, bgBounds.Width - 24, bgBounds.Height - 12),
                    new Rectangle(16, 12, 4, 24),
                    Color.White);
            }
            else
            {
                // left side
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X, bgBounds.Y, 12, bgBounds.Height + 4),
                    new Rectangle(4, 0, 12, 48),
                    Color.White);

                // center
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + 12, bgBounds.Y, bgBounds.Width - 24, bgBounds.Height + 4),
                    new Rectangle(16, 0, 4, 48),
                    Color.White);

                // right side
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + bgBounds.Width - 12, bgBounds.Y, 12, bgBounds.Height + 4),
                    new Rectangle(_backgroundTexture.Bounds.Width - 12, 0, 12, 48),
                    Color.White);
            }
        }

        protected override void DrawOption(SpriteBatch b, Rectangle optionBounds, int whichOption, float layerDepth)
        {
            Texture2D texture = _options[whichOption].Key;
            Rectangle optionSource = _options[whichOption].Value;
            float scale = 3f;

            b.Draw(texture,
                new(optionBounds.X + (optionBounds.Width - optionSource.Width * scale) / 2,
                    optionBounds.Y + (optionBounds.Height - optionSource.Height * scale) / 2),
                optionSource,
                Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }
    }
}
