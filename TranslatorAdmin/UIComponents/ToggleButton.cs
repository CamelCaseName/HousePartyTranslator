namespace Translator.Helpers
{
    public class ToggleButton : Button
    {
        public Color SecondBackColor = Color.FromKnownColor(KnownColor.Highlight);
        private bool isToggled = false;

        public ToggleButton()
        {
            Click += (object? sender, EventArgs e) => isToggled = !isToggled;
        }

        public bool IsChecked => isToggled;
    }
}
