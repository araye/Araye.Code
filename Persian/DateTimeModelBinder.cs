using System;
using System.Globalization;
using System.Web.Mvc;

namespace Araye.Code.Persian
{
    public class DateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            var date = (DateTime)value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);
            date = PersianCulture.PersianToGregorianUS(date);

            return date;
        }
    }

    public class NullableDateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value != null)
            {
                var date = (DateTime)value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);
                date = PersianCulture.PersianToGregorianUS(date);

                return date;
            }
            return null;
        }
    }
}