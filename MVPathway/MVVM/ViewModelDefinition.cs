using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MVPathway.MVVM.Abstractions
{
    public sealed class ViewModelDefinition
    {
        private List<Type> _qualities = new List<Type>();

        internal ViewModelDefinition()
        {
        }

        public bool HasQuality<TQuality>()
          where TQuality : IViewModelQuality
        {
            return _qualities.Any(x => x.FullName == typeof(TQuality).FullName ||
                                       typeof(TQuality).GetTypeInfo().IsAssignableFrom(x.GetTypeInfo()));
        }

        public ViewModelDefinition AddQuality<TQuality>()
          where TQuality : IViewModelQuality
        {
            if (HasQuality<TQuality>())
            {
                return this;
            }
            _qualities.Add(typeof(TQuality));
            return this;
        }

        public ViewModelDefinition RemoveQuality<TQuality>()
          where TQuality : IViewModelQuality
        {
            if (!HasQuality<TQuality>())
            {
                return this;
            }
            _qualities.Remove(typeof(TQuality));
            return this;
        }

        public ViewModelDefinition ClearQualities()
        {
            _qualities.Clear();
            return this;
        }
    }
}
