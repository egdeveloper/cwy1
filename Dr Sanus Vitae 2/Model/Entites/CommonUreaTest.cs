using System;
using System.ComponentModel;

namespace SanusVitae
{
    [DisplayName("Общий анализ мочи")]
    public class CommonUreaTest : MedicalTest
    {
        [Null]
        [TestInfo("Количество мочи", Unity.мл, false)]
        public double Amount { get; set; }

        [Null]
        [TestInfo("Ph", Unity.Внесист_ед, false)]
        public double Ph { get; set; }

        [Null]
        [TestInfo("Лейкоциты", Unity.Внесист_ед, true)]
        public double WBCells { get; set; }

        [Null]
        [TestInfo("Эритроциты", Unity.Внесист_ед, true)]
        public double RBCells { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Цвет", Unity.Внесист_ед, false)]
        public string Color { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Прозрачность", Unity.Внесист_ед, false)]
        public string Transparency { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Белок", Unity.Внесист_ед, false)]
        public string Protein { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Глюкоза", Unity.Внесист_ед, false)]
        public string Glucose { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Кетоновые тела", Unity.Внесист_ед, false)]
        public string KetoneBodies { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Реакция на кровь", Unity.Внесист_ед, false)]
        public string BloodReaction { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Билирубин", Unity.Внесист_ед, false)]
        public string BiliRuby { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Слизь", Unity.Внесист_ед, false)]
        public string Mucus { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Бактерии", Unity.Внесист_ед, false)]
        public string Bacteria { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Соли", Unity.Внесист_ед, false)]
        public string Salt { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Уробилиноиды", Unity.Внесист_ед, false)]
        public string UreaBilins { get; set; }

        [Null]
        [LengthAttribute(255)]
        [TestInfo("Цилиндры", Unity.Внесист_ед, false)]
        public string Cylinder { get; set; }

        [Transient]
        public override string this[string columnName]
        {
            get
            {
                string error_message = String.Empty;
                switch (columnName)
                {
                    case "Ph":
                        if (Ph < 0.0 || Ph > 14.0)
                            error_message = "Значения Ph лежат в пределах [0;14]!";
                        break;
                    default:
                        error_message = base[columnName];
                        break;
                }
                return error_message;
            }
        }
    }
}
