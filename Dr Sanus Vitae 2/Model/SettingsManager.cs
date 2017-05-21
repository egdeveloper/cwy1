using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace SanusVitae
{
    
    /// <summary>
    /// Класс-хранилище личных данных пользователя
    /// </summary>
    [Serializable]
    public class PersonalInfo : Person
    {
        public string Password { get; set; }
        public string Login { get; set; }
        public string JobPlace { get; set; }
        public string JobPost { get; set; }
        public string PhoneNumber { get; set; }
        public string SecretQuestion { get; set; }
        public string Answer { get; set; }
        public bool UseSecretQuestion { get; set; }
        public override string this[string columnName]
        {
            get
            {
                string error_message = String.Empty;
                switch(columnName)
                {
                    case "Login":
                        if (Login != null && Login.Any(c => !Char.IsLetterOrDigit(c) & c != '_'))
                            error_message = "Логин не может содержать символов, отличных от букв, цифр и нижнего подчеркивания!";
                        break;
                    default:
                        error_message = base[columnName];
                        break;
                }
                return error_message;
            }
        }
    }
    /// <summary>
    /// Менеджер работы с личными настройками пользователя
    /// </summary>
    class SettingsManager : Singleton<SettingsManager>
    {
        private string settings_path;
        private Stream settings_file_stream_;
        private bool get_settings_from_exe = false;
        private XmlSerializer personal_serializer;
        private static PersonalInfo personal_info;
        private SettingsManager() : base() { }
        public void InitManager(string settings_path)
        {
            personal_serializer = new XmlSerializer(typeof(PersonalInfo));
            this.settings_path = settings_path;
            get_settings_from_exe = false;
        }
        public void InitializeManager(Stream sfile_stream)
        {
            personal_serializer = new XmlSerializer(typeof(PersonalInfo));
            settings_file_stream_ = sfile_stream;
            get_settings_from_exe = true;
        }
        public string SettingsPath
        {
            get
            {
                return settings_path;
            }
            set
            {
                settings_path = value;
            }
        }
        public PersonalInfo PersonalInfo
        {
            get
            {
                if (personal_info != null) 
                    return personal_info;
                if (!get_settings_from_exe && File.Exists(settings_path) && new FileInfo(settings_path).Length > 0)
                {
                    using (StreamReader sr = new StreamReader(settings_path, Encoding.UTF8))
                    {
                        personal_info = (PersonalInfo)personal_serializer.Deserialize(sr);
                        return personal_info;
                    }
                }
                else if(get_settings_from_exe)
                {
                    personal_info = (PersonalInfo)personal_serializer.Deserialize(settings_file_stream_);
                    return personal_info;
                }
                return null;
            }
            set
            {
                personal_info = value;
                if (!get_settings_from_exe)
                {
                    using (StreamWriter sw = new StreamWriter(settings_path, false, Encoding.UTF8))
                    {
                        personal_serializer.Serialize(sw, personal_info);
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(settings_file_stream_, Encoding.UTF8))
                    {
                        personal_serializer.Serialize(sw, personal_info);
                    }
                }
            }
        }
    }
}

