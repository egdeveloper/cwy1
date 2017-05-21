using System;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SanusVitae
{
    public abstract class Person : IDataErrorInfo
    {
        [Transient]
        public string FullName { get { return ToString(); } }

        [NotNull]
        [LengthAttribute(255)]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [NotNull]
        [LengthAttribute(255)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [NotNull]
        [LengthAttribute(255)]
        [Display(Name = "Отчество")]
        public string Patronym { get; set; }

        [Transient]
        public string Error
        {
            get { return String.Empty; }
        }

        [Transient]
        public virtual string this[string columnName]
        {
            get
            {
                string error_message = String.Empty;
                PropertyInfo property = this.GetType().GetProperty(columnName);
                if (property != null && property.IsDefined(typeof(RequiredAttribute), true))
                {
                    RequiredAttribute required_attribute = new RequiredAttribute();
                    if (!required_attribute.IsValid(property.GetValue(this)))
                        return "Заполните обязательное поле!";
                }
                switch (columnName)
                {
                    case "FirstName":
                        if (FirstName != null && FirstName.Any(c => Char.IsDigit(c)))
                            error_message = "Введите имя русскими или латинскими буквами!";
                        break;
                    case "LastName":
                        if (LastName != null && LastName.Any(c => Char.IsDigit(c)))
                            error_message = "Введите фамилию русскими или латинскими буквами!";
                        break;
                    case "Patronym":
                        if (Patronym != null && Patronym.Any(c => Char.IsDigit(c)))
                            error_message = "Введите отчество русскими или латинскими буквами!";
                        break;
                }
                return error_message;
            }
        }
    }
}
