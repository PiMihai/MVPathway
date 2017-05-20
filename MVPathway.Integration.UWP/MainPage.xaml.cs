using MVPathway.Builder;

namespace MVPathway.Integration.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadApplication(PathwayFactory.Create<MVPathway.Integration.App>());
        }
    }
}
