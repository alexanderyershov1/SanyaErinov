using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace GeometryApp2
{
    public class Enums
    {
    }
    public enum TypeOfParallelepiped
    {
        Прямоугольный,
        Куб,
        Простой
    }

    public class ColumnParse
    {
        public string Value { set; get; }
        public TypeOfParallelepiped ValueType { set; get; }
    }
    public class EnumToItemsSource : MarkupExtension
    {
        private readonly Type _type;

        public EnumToItemsSource(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _type.GetMembers().SelectMany(member => member.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>()).Select(x => x.Description).ToList();
        }
    }
}
