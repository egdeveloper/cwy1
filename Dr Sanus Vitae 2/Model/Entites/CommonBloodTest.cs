using System;
using System.ComponentModel;
using System.Reflection;

namespace SanusVitae
{
    [DisplayName("Общий анализ крови")]
    public class CommonBloodTest : MedicalTest
    {
        [Null]
        [TestInfo("Гемоглобин", Unity.г_л, true)]
        public double Hemoglobin { get; set; }

        [Null]
        [TestInfo("Эритроциты", Unity.Клеток_л, true)]
        public double RBCells { get; set; }

        [Null]
        [TestInfo("Цветовой показатель", Unity.Внесист_ед, true)]
        public double ColorIndex { get; set; }

        [Null]
        [TestInfo("Ретикулоциты", Unity.perCents, true)]
        public double ImRBCells { get; set; }

        [Null]
        [TestInfo("Тромбоциты", Unity.Клеток_л, true)]
        public double Platelets { get; set; }

        [Null]
        [TestInfo("СОЭ", Unity.мм_ч, true)]
        public double ESR { get; set; }

        [Null]
        [TestInfo("Лейкоциты", Unity.Клеток_л, true)]
        public double WBCells { get; set; }

        [Null]
        [TestInfo("Палочкоядерные", Unity.perCents, true)]
        public double BandCells { get; set; }

        [Null]
        [TestInfo("Сегментоядерные", Unity.perCents, true)]
        public double SegmentCells { get; set; }

        [Null]
        [TestInfo("Эозинофилы", Unity.perCents, true)]
        public double EosinCells { get; set; }

        [Null]
        [TestInfo("Базофилы", Unity.perCents, true)]
        public double Basophil { get; set; }

        [Null]
        [TestInfo("Лимфоциты", Unity.perCents, true)]
        public double NKCells { get; set; }

        [Null]
        [TestInfo("Моноциты", Unity.perCents, true)]
        public double MonoCells { get; set; }
    }
}
