﻿using MVPathway.Integration.Tasks.Base;
using MVPathway.Utils.Converters;
using System.Globalization;

namespace MVPathway.Integration.Tasks.Utils
{
  public class ConverterTask : UtilsIntegrationTask
  {
    public override bool Execute()
    {
      return testBoolToNotBoolConverter() &&
             testBoolToStringConverter();
    }

    bool testBoolToNotBoolConverter()
    {
      var c = new BoolToNotBoolConverter();

      return !(bool)c.Convert(true, typeof(bool), null, CultureInfo.InvariantCulture) &&
              (bool)c.Convert(false, typeof(bool), null, CultureInfo.InvariantCulture) &&
             !(bool)c.ConvertBack(true, typeof(bool), null, CultureInfo.InvariantCulture) &&
              (bool)c.ConvertBack(false, typeof(bool), null, CultureInfo.InvariantCulture);
    }

    bool testBoolToStringConverter()
    {
      var c = new BoolToStringConverter();
      var bOptions = "yes,no";

      return c.Convert(true, typeof(bool), bOptions, CultureInfo.InvariantCulture) as string == "yes" &&
             c.Convert(false, typeof(bool), bOptions, CultureInfo.InvariantCulture) as string == "no" &&
             (bool)c.ConvertBack("yes", typeof(bool), bOptions, CultureInfo.InvariantCulture) &&
             !(bool)c.ConvertBack("no", typeof(bool), bOptions, CultureInfo.InvariantCulture);
    }
  }
}
