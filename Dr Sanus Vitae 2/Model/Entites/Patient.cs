using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SanusVitae
{
    public enum Sex : int {
        [Description("Мужской")]
        мужской = 0, 
        [Description("Женский")]
        женский = 1 
    }
    public enum Rh : int {
        [Description("Rh+")]
        Positive = 0, 
        [Description("Rh-")]
        Negative = 1
    }
    public enum BloodGroup : int {
        [Description("I")]
        I = 0, 
        [Description("II")]
        II = 1, 
        [Description("III")]
        III = 2, 
        [Description("IV")]
        IV = 3
    }
    public enum Disability : int { 
        [Description("Нет")]
        None = 0, 
        [Description("I")]
        I = 1, 
        [Description("II")]
        II = 2, 
        [Description("III")]
        III = 3
    }
    public class Patient : Person
    {
        [PrimaryKey]
        public long ID { get; set; }

        [NotNull]
        [DisplayName("Пол")]
        public Sex Sex { get; set; }

        [NotNull]
        [DisplayName("Дата рождения")]
        public DateTime Birthdate { get; set; }

        [NotNull]
        [LengthAttribute(255)]
        [DisplayName("Номер карты")]
        public string CardNumber { get; set; }

        [NotNull]
        [LengthAttribute(255)]
        [DisplayName("Паспортные данные")]
        public string Passport { get; set; }

        [Null]
        [LengthAttribute(255)]
        [DisplayName("Телефон")]
        public string PhoneNumber { get; set; }

        [NotNull]
        [LengthAttribute(255)]
        [DisplayName("Страна")]
        public string Country { get; set; }

        [NotNull]
        [LengthAttribute(255)]
        [DisplayName("Почтовый индекс")]
        public string PostIndex { get; set; }

        [NotNull]
        [LengthAttribute(255)]
        [DisplayName("Регион")]
        public string Region { get; set; }

        [NotNull]
        [LengthAttribute(255)]
        [DisplayName("Населенный пункт")]
        public string City { get; set; }

        [NotNull]
        [LengthAttribute(255)]
        [DisplayName("Адрес проживания")]
        public string Address { get; set; }

        [Null]
        [LengthAttribute(255)]
        [DisplayName("Электронная почта")]
        public string Email { get; set; }

        [Null]
        [DisplayName("Фото пациента")]
        public byte[] Photo { get; set; }

        [NotNull]
        [DisplayName("Резус-фактор")]
        public Rh Rh { get; set; }

        [NotNull]
        [DisplayName("Группа крови")]
        public BloodGroup BloodGroup { get; set; }

        [NotNull]
        [DisplayName("Инвалидность")]
        public Disability Disability { get; set; }

        [Null]
        [LengthAttribute(255)]
        [DisplayName("ИНН")]
        public string TIN { get; set; }

        [NotNull]
        [LengthAttribute(255)]
        [DisplayName("Полис ОМС")]
        public string OMICard { get; set; }

        [Null]
        [LengthAttribute(255)]
        [DisplayName("Место работы")]
        public string JobPlace { get; set; }

        [Null]
        [LengthAttribute(255)]
        [DisplayName("Профессия")]
        public string Occupation { get; set; }

        [Null]
        [LengthAttribute(255)]
        [DisplayName("Должность")]
        public string Post{ get; set; }

        [Null]
        [LengthAttribute(255)]
        [DisplayName("Характер труда")]
        public string JobConditions { get; set; }

        [Null]
        [Length(255)]
        [DisplayName("Жалобы до поступления")]
        public string Complaints { get; set; }

        [Null]
        [Length(255)]
        [DisplayName("Лечение до поступления")]
        public string Premedication { get; set; }

        [Null]
        [Length(255)]
        [DisplayName("Сопутствующее заболевания")]
        public string AssociatedDisease { get; set; }

        [Null]
        [Length(255)]
        [DisplayName("Назначения до поступления")]
        public string PreMedicalSupplies { get; set; }

        [Null]
        [Length(255)]
        [DisplayName("Вредные привычки")]
        public string BadHabits { get; set; }

        [Null]
        [Length(255)]
        [DisplayName("Описание камня до поступления")]
        public string PreUreaStoneDescription { get; set; }

        [Null]
        [Length(255)]
        [DisplayName("Длительность заболевания")]
        public string DiseaseDuration { get; set; }

        [Transient]
        public override string ToString()
        {
            return LastName + " " + FirstName + " " + Patronym;
        }
    }
}
