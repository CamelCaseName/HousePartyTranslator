using Translator.Core.Helpers;

namespace Translator.Helpers
{
    public class ToggleButton : Button
    {
        public Color SecondBackColor = Color.FromKnownColor(KnownColor.Highlight);
        private Color BackgroundColor = DefaultBackColor;
        private bool isToggled = false;
        private bool ColorLock = false;

        public ToggleButton()
        {
            Click += (object? sender, EventArgs e) =>
            {
                isToggled = !isToggled;
                ColorLock = true;
                BackColor = isToggled ? SecondBackColor : BackgroundColor;
                ColorLock = false;
            };
        }

        public new Color BackColor
        {
            get => BackgroundColor;
            set => _ = ColorLock ? base.BackColor = value : BackgroundColor = base.BackColor = value;
        }

        public bool IsChecked => isToggled;
    }
}
