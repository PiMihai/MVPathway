using MVPathway.MVVM;

namespace MVPathway.Utils.ViewModels.Qualities
{
    public class MenuQuality : FullscreenQuality { }
    public class ChildQuality : ViewModelQuality { }
    public class MainChildQuality : ChildQuality { }
    public class FullscreenQuality : ViewModelQuality { }
    public class ModalQuality : ViewModelQuality { }
}
