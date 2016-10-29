using MVPathway.Builder;

namespace MVPathway.Integration.UWP
{
  public sealed partial class MainPage
  {
    public MainPage()
    {
      this.InitializeComponent();
      LoadApplication(PathwayCore.Create<MVPathway.Integration.App>());
    }
  }
}
