using System;
using System.ComponentModel;

namespace SanusVitae
{
    [DisplayName("Титриметрия")]
    public class TitrationTest : MedicalTest
    {
        [Null]
        [TestInfo("Оксалат", Unity.Ммоль_сут, true)]
        public double Oxalate { get; set; }
    }
}
