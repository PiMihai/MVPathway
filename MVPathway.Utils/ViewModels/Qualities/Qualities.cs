using MVPathway.MVVM;

namespace MVPathway.Utils.ViewModels.Qualities
{
    public interface IParentQuality : IViewModelQuality { }
    public interface IChildQuality : IViewModelQuality { }
    public interface IMainChildQuality : IChildQuality { }
    public interface IModalQuality : IViewModelQuality { }
    public interface IFullscreenQuality : IViewModelQuality { }
}
