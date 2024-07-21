namespace LocalSearchEngine.Controls
{
    public abstract class HyperlinkBase : Label
    {
        public HyperlinkBase()
        {
            TextDecorations = TextDecorations.Underline;
            TextColor = Colors.Blue;
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await this.OnClick())
            });
        }

        protected abstract Task OnClick();
    }
}
