using System;
using System.ComponentModel;

namespace SanusVitae
{
    [DisplayName("Хроматография")]
    public class UreaColorTest : MedicalTest
    {
        [Null]
        [TestInfo("Диурез", Unity.Внесист_ед, false)]
        public double DUV { get; set; }

        [Null]
        [TestInfo("Хлорид", Unity.Ммоль_сут, true)]
        public double ClSalt { get; set; }

        [Null]
        [TestInfo("Нитрит", Unity.Ммоль_сут, true)]
        public double NO2Salt { get; set; }

        [Null]
        [TestInfo("Нитрат", Unity.Ммоль_сут, true)]
        public double NO3Salt { get; set; }

        [Null]
        [TestInfo("Сульфат", Unity.Ммоль_сут, true)]
        public double SO3Salt { get; set; }

        [Null]
        [TestInfo("Фосфат", Unity.Ммоль_сут, true)]
        public double PO3Salt { get; set; }

        [Null]
        [TestInfo("Цитрат", Unity.Ммоль_сут, true)]
        public double Citrate { get; set; }

        [Null]
        [TestInfo("Изоцитрат", Unity.Ммоль_сут, true)]
        public double IsoCitrate { get; set; }

        [Null]
        [TestInfo("Мочевая кислота", Unity.Ммоль_сут, true)]
        public double UreaAcid { get; set; }
    }
}
