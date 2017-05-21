using System;
using System.ComponentModel;

namespace SanusVitae
{
    [DisplayName("Суточная экскреция")]
    public class DailyExcreationTest : MedicalTest
    {
        [Null]
        [TestInfo("Креатинин", Unity.ммоль_сут, true)]
        public double Creatinine { get; set; }

        [Null]
        [TestInfo("Мочевина", Unity.ммоль_сут, true)]
        public double Urea { get; set; }

        [Null]
        [TestInfo("Мочевая кислота", Unity.ммоль_сут, true)]
        public double UreaAcid { get; set; }

        [Null]
        [TestInfo("Кальций", Unity.ммоль_сут, true)]
        public double Calcium { get; set; }

        [Null]
        [TestInfo("Калий", Unity.ммоль_сут, true)]
        public double Potassium { get; set; }

        [Null]
        [TestInfo("Магний", Unity.ммоль_сут, true)]
        public double Magnesium { get; set; }

        [Null]
        [TestInfo("Натрий", Unity.Ммоль_л, true)]
        public double Sodium { get; set; }

        [Null]
        [TestInfo("Оксид Фосфора", Unity.Ммоль_л, true)]
        public double PhosphorOxide { get; set; }

        [Null]
        [TestInfo("Хлор", Unity.Ммоль_л, true)]
        public double Chlorine { get; set; }
    }
}
