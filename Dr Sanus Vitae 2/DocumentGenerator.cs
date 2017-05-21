using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.IO;
using Novacode;

namespace SanusVitae
{
    public static class DocumentGenerator
    {
        public static void CreateTestReport(Patient patient, MedicalTest test, string file_path)
        {
            if (file_path == null)
                return;
            Type test_type = test.GetType();
            using (DocX document = DocX.Create(file_path))
            {
                Novacode.Paragraph header = document.InsertParagraph(
                //selected_procedure.ProcedureType.First().ToString().ToUpper() + String.Join("", selected_procedure.ProcedureType.Skip(1)).ToLower(),
                test_type.GetCustomAttribute<DisplayNameAttribute>().DisplayName,
                false,
                new Formatting()
                {
                    FontFamily = new System.Drawing.FontFamily("Times New Roman"),
                    Size = 16D
                }
                );
                header.Alignment = Alignment.center;
                header.Bold();
                document.InsertParagraph(" ");
                Novacode.Table patient_info = document.AddTable(5, 2);
                patient_info.Rows[0].Cells[0].Paragraphs.First().Append("ФИО пациента").Bold();
                patient_info.Rows[0].Cells[1].Paragraphs.First().Append(patient.LastName + " " + patient.FirstName + " " + patient.Patronym);
                patient_info.Rows[1].Cells[0].Paragraphs.First().Append("Пол").Bold();
                patient_info.Rows[1].Cells[1].Paragraphs.First().Append(patient.Sex == Sex.мужской ? "мужской" : "женский");
                patient_info.Rows[2].Cells[0].Paragraphs.First().Append("Дата рождения").Bold();
                patient_info.Rows[2].Cells[1].Paragraphs.First().Append(patient.Birthdate.ToShortDateString());
                patient_info.Rows[3].Cells[0].Paragraphs.First().Append("Адрес проживания").Bold();
                patient_info.Rows[3].Cells[1].Paragraphs.First().Append(
                    patient.PostIndex + ", " + patient.Country + ", " + patient.Region +
                    ", " + patient.City + ", " + patient.Address
                    );
                patient_info.Rows[4].Cells[0].Paragraphs.First().Append("Номер карты").Bold();
                patient_info.Rows[4].Cells[1].Paragraphs.First().Append(patient.CardNumber.ToString());
                patient_info.Alignment = Alignment.left;
                patient_info.AutoFit = AutoFit.Window;
                PropertyInfo[] indicators = test_type.GetProperties().Where(x => x.IsDefined(typeof(TestInfo))).ToArray();
                Novacode.Table test_info = document.AddTable(indicators.Count(), 2);
                int k = 0;
                foreach (var indicator in indicators)
                {
                    test_info.Rows[k].Cells[0].Paragraphs.First().Append(indicator.GetCustomAttribute<TestInfo>().DisplayName);
                    object property_value = indicator.GetValue(test);
                    test_info.Rows[k].Cells[1].Paragraphs.First().Append(property_value != null ? property_value.ToString() : "");
                    k++;
                }
                test_info.Alignment = Alignment.left;
                test_info.AutoFit = AutoFit.Window;
                document.InsertTable(patient_info);
                document.InsertParagraph(" ");
                document.InsertTable(test_info);
                document.Save();
            }
        }
        public static void CreateDynamicsReport(Patient patient, IEnumerable<MedicalTest> test_list, Type test_type, PropertyInfo indicator, string file_path)
        {
            if (file_path == null)
                return;
            else
            {
                var norms = PatientDB.Instance.GetTestNorms(test_type)[indicator.Name];
                using (DocX document = DocX.Create(file_path))
                {
                    Novacode.Paragraph header_test = document.InsertParagraph(
                    test_type.GetCustomAttribute<DisplayNameAttribute>().DisplayName,
                    false,
                    new Formatting()
                    {
                        FontFamily = new System.Drawing.FontFamily("Times New Roman"),
                        Size = 18D
                    }
                    );
                    header_test.Alignment = Alignment.center;
                    header_test.Bold();
                    Novacode.Paragraph header_indicator = document.InsertParagraph(
                    indicator.GetCustomAttribute<TestInfo>().DisplayName,
                    false,
                    new Formatting()
                    {
                        FontFamily = new System.Drawing.FontFamily("Times New Roman"),
                        Size = 16D
                    }
                    );
                    header_indicator.Alignment = Alignment.center;
                    header_indicator.Bold();
                    document.InsertParagraph(" ");
                    Novacode.Table patient_info = document.AddTable(5, 2);
                    patient_info.Rows[0].Cells[0].Paragraphs.First().Append("ФИО пациента").Bold();
                    patient_info.Rows[0].Cells[1].Paragraphs.First().Append(patient.LastName + " " + patient.FirstName + " " + patient.Patronym);
                    patient_info.Rows[1].Cells[0].Paragraphs.First().Append("Пол").Bold();
                    patient_info.Rows[1].Cells[1].Paragraphs.First().Append(patient.Sex == Sex.мужской ? "мужской" : "женский");
                    patient_info.Rows[2].Cells[0].Paragraphs.First().Append("Дата рождения").Bold();
                    patient_info.Rows[2].Cells[1].Paragraphs.First().Append(patient.Birthdate.ToShortDateString());
                    patient_info.Rows[3].Cells[0].Paragraphs.First().Append("Адрес проживания").Bold();
                    patient_info.Rows[3].Cells[1].Paragraphs.First().Append(
                        patient.PostIndex + ", " + patient.Country + ", " + patient.Region +
                        ", " + patient.City + ", " + patient.Address
                        );
                    patient_info.Rows[4].Cells[0].Paragraphs.First().Append("Номер карты").Bold();
                    patient_info.Rows[4].Cells[1].Paragraphs.First().Append(patient.CardNumber.ToString());
                    patient_info.Alignment = Alignment.left;
                    patient_info.AutoFit = AutoFit.Window;
                    Novacode.Table dynamics = document.AddTable(test_list.Count() + 1, 2);
                    dynamics.Rows[0].Cells[0].Paragraphs.First().Append("Дата сдачи анализов").Bold();
                    dynamics.Rows[0].Cells[1].Paragraphs.First().Append("Значение показателя (" + indicator.GetCustomAttribute<TestInfo>().Unity.GetDescription() + ")").Bold();
                    int k = 1;
                    foreach (var test_ in test_list.OrderBy(x => x.Date))
                    {
                        double value_ = (double)indicator.GetValue(test_);
                        dynamics.Rows[k].Cells[0].Paragraphs.First().Append(test_.Date.ToShortDateString());
                        if (value_ >= norms.Min - norms.MinError && value_ <= norms.Max + norms.MaxError)
                            dynamics.Rows[k].Cells[1].Paragraphs.First().Append(value_.ToString()).Highlight(Highlight.green);
                        else
                            dynamics.Rows[k].Cells[1].Paragraphs.First().Append(value_.ToString()).Highlight(Highlight.red);
                        k++;
                    }
                    dynamics.Alignment = Alignment.left;
                    dynamics.AutoFit = AutoFit.Window;
                    document.InsertTable(patient_info);
                    document.InsertParagraph(" ");
                    document.InsertTable(dynamics);
                    /*
                    bitmap_encoder = new PngBitmapEncoder();
                    using (MemoryStream pic_stream = new MemoryStream())
                    {
                        bitmap_encoder.Frames.Add(BitmapFrame.Create(rtb));
                        bitmap_encoder.Save(pic_stream);
                        pic_stream.Seek(0, SeekOrigin.Begin);
                        document.InsertParagraph(" ");
                        Novacode.Picture graph = document.AddImage(pic_stream).CreatePicture();
                        graph.SetPictureShape(Novacode.RectangleShapes.rect);
                        Novacode.Paragraph graph_paragraph = document.InsertParagraph();
                        graph_paragraph.InsertPicture(graph);
                    }
                    */
                    document.Save();
                }
            }
        }
    }
}
