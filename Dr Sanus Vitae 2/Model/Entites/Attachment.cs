using System;
using System.ComponentModel;

namespace SanusVitae
{
    [DisplayName("Прикрепляемый файл")]
    public class Attachment
    {
        [PrimaryKey]
        public long ID { get; set; }

        [DisplayName("ID анализа")]
        public long TestID { get; set; }

        [DisplayName("Ресурс")]
        public byte[] Item { get; set; }

        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Расширение файла")]
        public string Extension { get; set; }

        [DisplayName("Таблица-владелец")]
        public string TableOwner { get; set; }

        [Transient]
        public override string ToString()
        {
            string file_type_ = "";
            switch (Extension)
            {
                case "png": case "jpg" : case "bmp":
                    file_type_ += "Изображение ";
                    break;
                case "docx" : case "doc":
                    file_type_ += "Word документ ";
                    break;
                case "pdf":
                    file_type_ += "PDF документ ";
                    break;
                default:
                    file_type_ += "Документ ";
                    break;
            }
            return file_type_ + Name;
        }
    }
}
