using System.Reflection;

namespace MVPathway.MVVM
{
  public abstract class ViewModelQuality
  {
    public override bool Equals(object obj)
    {
      if(obj == null || !(obj is ViewModelQuality))
      {
        return false;
      }
      return GetType().FullName == obj.GetType().FullName ||
             GetType().GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
    }

    public override int GetHashCode()
    {
      return GetType().FullName.GetHashCode();
    }
  }
}
