using System;
using System.ComponentModel;

namespace SanusVitae
{
    [DisplayName("Мочевой камень")]
    public class UreaStoneTest : MedicalTest
    {
        [Null]
        [TestInfo("Размер камня", Unity.Внесист_ед, false)]
        public double Size { get; set; }

        [Null]
        [TestInfo("Плотность камня", Unity.Внесист_ед, false)]
        public double Density { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Локализация камня", Unity.Внесист_ед, false)]
        public string Location { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Вид камня", Unity.Внесист_ед, false)]
        public string Form { get; set; }

        [Null]
        [TestInfo("Твердость камня", Unity.Внесист_ед, false)]
        public double Hardness { get; set; }

        [Null]
        [TestInfo("Доп. информация", Unity.Внесист_ед, false)]
        public string AddInfo { get; set; }
    }
}
