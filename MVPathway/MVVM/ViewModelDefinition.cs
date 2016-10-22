using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MVPathway.MVVM.Abstractions
{
  public sealed class ViewModelDefinition
  {
    private List<Type> mQualities = new List<Type>();

    public bool HasQuality<TQuality>()
      where TQuality : ViewModelQuality
    {
      return mQualities.Any(x => x.FullName == typeof(TQuality).FullName ||
                                 typeof(TQuality).GetTypeInfo().IsAssignableFrom(x.GetTypeInfo()));
    }

    public void AddQuality<TQuality>()
      where TQuality : ViewModelQuality
    {
      if(HasQuality<TQuality>())
      {
        return;
      }
      mQualities.Add(typeof(TQuality));
    }

    public void RemoveQuality<TQuality>()
      where TQuality : ViewModelQuality
    {
      if (!HasQuality<TQuality>())
      {
        return;
      }
      mQualities.Remove(typeof(TQuality));
    }
  }
}
