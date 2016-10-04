using MVPathway.Presenters;
using System.Threading.Tasks;

namespace MVPathway.Integration.Presenters
{
  class MyPresenter : BasePresenter
  {
    public MyPresenter(IPathwayCore pathwayCore) : base(pathwayCore)
    {
    }

    protected override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText)
    {
      return await Task.FromResult(true);
    }
  }
}
