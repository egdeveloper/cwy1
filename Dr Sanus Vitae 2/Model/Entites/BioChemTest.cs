using System;
using System.ComponentModel;

namespace SanusVitae
{
    [DisplayName("Биохимический анализ крови")]
    public class BioChemTest : MedicalTest
    {
        [TestInfo("Общий белок", Unity.г_л, true)]
        public double CommonProtein { get; set; }
        [TestInfo("Мочевина", Unity.ммоль_л, true)]
        public double Urea { get; set; }
        [TestInfo("Креатинин", Unity.мкМ_л, true)]
        public double Creatinine { get; set; }
        [TestInfo("Общий билирубин", Unity.мкМ_л, true)]
        public double CommonBiliRuby { get; set; }
        [TestInfo("Связанный билирубин", Unity.мкМ_л, true)]
        public double LinkedBiliRuby { get; set; }
        [TestInfo("Холестерин", Unity.мкмоль_л, true)]
        public double Cholesterol { get; set; }
        [TestInfo("Триглицериды", Unity.мМ_л, true)]
        public double TAG { get; set; }
        [TestInfo("Липопр. выс. кон.", Unity.ммоль_л, true)]
        public double HDL { get; set; }
        [TestInfo("Липопр. низ. кон.", Unity.ммоль_л, true)]
        public double LDL { get; set; }
        [TestInfo("Коэф. атер.", Unity.Внесист_ед, true)]
        public double CholesterolRatio { get; set; }
        [TestInfo("Аланинаминотрасфераза", Unity.Ед_л, true)]
        public double ALT { get; set; }
        [TestInfo("Аспартатаминотрансфераза", Unity.Ед_л, true)]
        public double AST { get; set; }
        [TestInfo("Щелочная фосфотаза", Unity.Ед_л, true)]
        public double ALKP { get; set; }
        [TestInfo("Креатинфосфокиназа", Unity.Ед_л, true)]
        public double CK { get; set; }
        [TestInfo("Креатинфосфокиназа МВ", Unity.Ед_л, true)]
        public double CKMB { get; set; }
        [TestInfo("Лактатдкгидрогеназа", Unity.Ед_л, true)]
        public double LDH { get; set; }
        [TestInfo("ГГТ", Unity.Ед_л, true)]
        public double GGT { get; set; }
        [TestInfo("Амилаза", Unity.Ед_л, true)]
        public double Amylase { get; set; }
        [TestInfo("Пакриатическая амилаза", Unity.Ед_л, true)]
        public double PancrAmylase { get; set; }
        [TestInfo("Глюкоза", Unity.Ед_л, true)]
        public double Glucose { get; set; }
        [TestInfo("Мочевая кислота", Unity.мкМ_л, true)]
        public double UreaAcid { get; set; }
        [TestInfo("С-реактивный белок", Unity.мг_л, true)]
        public double CRP { get; set; }
        [TestInfo("Ревматоидный фактор", Unity.Ед_л, true)]
        public double RF { get; set; }
        [TestInfo("Калий", Unity.ммоль_л, true)]
        public double Potassium { get; set; }
        [TestInfo("Натрий", Unity.ммоль_л, true)]
        public double Sodium { get; set; }
        [TestInfo("Хлор", Unity.ммоль_л, true)]
        public double Chlorine { get; set; }
        [TestInfo("Общий кальций", Unity.ммоль_л, true)]
        public double CommonCalcium { get; set; }
        [TestInfo("Ионизированный кальций", Unity.мм_ч, true)]
        public double IonCalcium { get; set; }
        [TestInfo("Фосфор", Unity.мМ_л, true)]
        public double Phosphor { get; set; }
        [TestInfo("Железо", Unity.мкМ_л, true)]
        public double Ferrum { get; set; }
        [TestInfo("Ферритин", Unity.Мкг_мл, true)]
        public double Ferritin { get; set; }
    }
}
