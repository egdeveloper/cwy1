using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Collections.Generic;

namespace SanusVitae
{

    internal struct TestNorms
    {
        public double Min { get; set; }
        public double MinError { get; set; }
        public double Max { get; set; }
        public double MaxError { get; set; }
    }
    public enum Unity : int
    {
        [Description("ммоль/сут")]
        ммоль_сут = 0,
        [Description("мг/сут")]
        мг_сут = 1,
        [Description("г/сут")]
        г_сут = 2,
        [Description("ммоль/л")]
        ммоль_л = 3,
        [Description("мкмоль/л")]
        мкмоль_л = 4,
        [Description("мг/л")]
        мг_л = 5,
        [Description("г/л")]
        г_л = 6,
        [Description("%")]
        perCents = 7,
        [Description("Клеток/л")]
        Клеток_л = 8,
        [Description("Ед/л")]
        Ед_л = 9,
        [Description("внесист. ед.")]
        Внесист_ед = 10,
        [Description("мм/ч")]
        мм_ч = 11,
        [Description("мкМ/Л")]
        мкМ_л = 12,
        [Description("мМ/л")]
        мМ_л = 13,
        [Description("Мкг/мл")]
        Мкг_мл = 14,
        [Description("мл")]
        мл = 15,
        [Description("Ммоль/л")]
        Ммоль_л = 16,
        [Description("Ммоль/сут")]
        Ммоль_сут = 17
    }
    
    [DisplayName("Медицинский анализ")]
    public abstract class MedicalTest : IDataErrorInfo
    {
        [PrimaryKey]
        public long ID { get; set; }

        [NotNull]
        [DisplayName("ID пациента")]
        public long PatientID { get; set; }

        [NotNull]
        [DisplayName("Дата сдачи анализов")]
        public DateTime Date { get; set; }

        [Null]
        [LengthAttribute(255)]
        [DisplayName("Описание")]
        public string Treatment { get; set; }

        [Transient]
        public List<Attachment> Attachments { get; set; }

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
                PropertyInfo property = this.GetType().GetProperty(columnName);
                string error_message = String.Empty;
                if (property.PropertyType == typeof(double) && (double)property.GetValue(this) < 0.0)
                    error_message = "Значение не может быть отрицательным!";
                return error_message;
            }
        }

        [Transient]
        public override string ToString()
        {
            return this.GetType().GetCustomAttribute<DisplayNameAttribute>().DisplayName;
        }
    }
}