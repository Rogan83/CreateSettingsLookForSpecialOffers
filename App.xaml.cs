namespace CreateSettingsLookForSpecialOffers
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
        }
        protected override Window CreateWindow(IActivationState activationState) =>
        new Window(new AppShell())
        {
            Width = 800,
            //Height = 500,
            X = 100,
            Y = 100
        };
    }
}
