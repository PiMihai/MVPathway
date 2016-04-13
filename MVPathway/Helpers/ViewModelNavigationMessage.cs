using System;

namespace MVPathway.Helpers
{
    public class ViewModelNavigationMessage
    {
        public Type ViewModelType { get; set; }
        public object Parameter { get; set; }
    }
}
