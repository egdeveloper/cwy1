using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Reflection;
using System.Globalization;
using System.Xml.Linq;
using System.Xml;

namespace SanusVitae
{
    public enum SQLTypeClass { Text, Binary, Real, Integer, AllowsNULL}
    public class SQLInfo : Attribute
    {
        private readonly string column_name;
        private readonly string sql_type;
        private readonly SQLTypeClass type_class;
        public string ColumnName
        {
            get { return column_name; }
        }
        public string SQLType
        {
            get { return sql_type; }
        }
        public SQLTypeClass TypeClass
        {
            get { return type_class; }
        }
        public SQLInfo(string column_name, string sql_type, SQLTypeClass type_class)
        {
            this.column_name = column_name;
            this.sql_type = sql_type;
            this.type_class = type_class;
        }
    }
    public interface IDBUpdated
    {
        event RoutedEventHandler DBUpdated;
    }
    /// <summary>
    /// Модель работы с данными пациентов
    /// </summary>
    class PatientDB : Singleton<PatientDB>, IDBUpdated
    {
        private enum SQLCmd { CreateTable, Insert, Select, Delete, Update}
        //Таблица Patients
        private string db_path;
        private string db_name;
        private string sql_conn_str;
        /*
        private SQLiteConnection sql_conn;
        private SQLiteCommand sql_command;
        private SQLiteDataReader sql_reader;
         * */
        private PatientDB() { }
        /// <summary>
        /// Инициализация базы данных
        /// </summary>
        /// <param name="db_name"></param>
        public void InitDB(string db_name)
        {
            this.db_name = db_name;
            db_path = Directory.GetCurrentDirectory() + @"\" + db_name;
            sql_conn_str = String.Format("Data Source={0};Version=3;UseUTF16Encoding=True;", db_name);
            if (!File.Exists(db_path))
            {
                SQLiteConnection.CreateFile(db_path); //создадим файл базы данных, если его нет
            }
            using (SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_str, true))
            {
                sql_conn.Open();
                using (SQLiteCommand sql_command = new SQLiteCommand(sql_conn))
                {
                    Type[] tables = new Type[] { typeof(Patient), typeof(CommonBloodTest), typeof(BioChemTest), typeof(DailyExcreationTest),
                        typeof(UreaColorTest), typeof(CommonUreaTest), typeof(TitrationTest), typeof(UreaStoneTest), typeof(Attachment)
                    };
                    foreach (var table in tables)
                    {
                        sql_command.CommandText = CreateSQLQuery(null, SQLCmd.CreateTable, String.Empty, table);
                        sql_command.ExecuteNonQuery();
                    }
                }
            }
        }
        /// <summary>
        /// Соединение с существующей БД
        /// </summary>
        /// <param name="db_path"></param>
        public void ConnectToDB(string db_path)
        {
            this.db_path = db_path;
            sql_conn_str = String.Format("Data Source={0};Version=3;UseUTF16Encoding=True;", db_path);
            if (DBUpdated != null)
                DBUpdated(this, null);
        }
        /// <summary>
        /// Сохранение текущей БД
        /// </summary>
        /// <param name="new_db_path"></param>
        public void SaveDBLocally(string new_db_path)
        {
            File.Copy(db_path, new_db_path);
        }
        /// <summary>
        /// Добавление записей о пациенте в базу данных
        /// </summary>
        /// <param name="patient"></param>
        public void AddPatient(ref Patient patient)
        {
            using (SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_str, true))
            {
                sql_conn.Open();
                using (SQLiteCommand sql_command = new SQLiteCommand(sql_conn))
                {
                    sql_command.CommandText = CreateSQLQuery(patient, patient.ID <= 0 ? SQLCmd.Insert : SQLCmd.Update, String.Empty, typeof(Patient));
                    sql_command.Parameters.Add(new SQLiteParameter("Photo", DbType.Binary) { Value = patient.Photo });
                    sql_command.ExecuteNonQuery();
                    sql_command.CommandText = "SELECT last_insert_rowid()";
                    patient.ID = (long)sql_command.ExecuteScalar();
                    if (DBUpdated != null)
                        DBUpdated(this, null);
                }
            }
        }
        /// <summary>
        /// Получение списка пациентов
        /// </summary>
        /// <returns></returns>
        public List<Patient> GetPatients()
        {
            try
            {
                List<Patient> patients = new List<Patient>();
                using (SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_str, true))
                {
                    sql_conn.Open();
                    using (SQLiteCommand sql_command = new SQLiteCommand("SELECT * FROM Patient ORDER BY LastName, FirstName, Patronym", sql_conn))
                    {
                        using (SQLiteDataReader sql_reader = sql_command.ExecuteReader())
                        {
                            while (sql_reader.Read())
                            {
                                Patient patient = new Patient();
                                foreach (PropertyInfo property in typeof(Patient).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => !x.IsDefined(typeof(Transient))))
                                {
                                    string property_name = property.Name;
                                    if (!DBNull.Value.Equals(sql_reader[property_name]))
                                    {
                                        if (property.PropertyType.IsEnum)
                                            property.SetValue(patient, Convert.ChangeType(Enum.ToObject(property.PropertyType, (int)sql_reader[property_name]), property.PropertyType));
                                        else
                                            property.SetValue(patient, Convert.ChangeType(sql_reader[property_name], property.PropertyType));
                                    }
                                }
                                patients.Add(patient);
                            }
                            return patients;
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new SQLiteException(SQLiteErrorCode.Corrupt, "Файл базы данных был испорчен и не может быть открыт!");
            }
        }
        /// <summary>
        /// Добавить данные анализов для данного пациента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="test"></param>
        public void AddAnalysis(ref MedicalTest test, Type test_type)
        {
            using (SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_str, true))
            {
                sql_conn.Open();
                using (SQLiteCommand sql_command = new SQLiteCommand(sql_conn))
                {
                    sql_command.CommandText = CreateSQLQuery(test, SQLCmd.Insert, String.Empty, test_type);
                    sql_command.ExecuteNonQuery();
                    sql_command.CommandText = "SELECT last_insert_rowid()";
                    test.ID = (long)sql_command.ExecuteScalar();
                }
            }
            foreach (var attachment in test.Attachments)
                attachment.TestID = test.ID;
            AddAttachment(test.Attachments);
            if (DBUpdated != null)
                DBUpdated(null, null);
        }
        /// <summary>
        /// Получить данные анализов для данного пациента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="patient_id"></param>
        /// <returns></returns>
        public List<MedicalTest> GetAnalysis(long patient_id, Type test_type)
        {
            try
            {
                List<MedicalTest> tests = new List<MedicalTest>();
                using (SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_str, true))
                {
                    sql_conn.Open();
                    using (SQLiteCommand sql_command = new SQLiteCommand(sql_conn))
                    {
                        sql_command.CommandText = CreateSQLQuery(null, SQLCmd.Select, "WHERE PatientID = " + patient_id.ToString(), test_type);
                        using (SQLiteDataReader sql_reader = sql_command.ExecuteReader())
                        {
                            while (sql_reader.Read())
                            {
                                MedicalTest test = (MedicalTest)test_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[0], new ParameterModifier[0]).Invoke(new object[0]);
                                foreach (PropertyInfo property in test_type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => !x.IsDefined(typeof(Transient))))
                                {
                                    if (DBNull.Value.Equals(sql_reader[property.Name]))
                                        continue;
                                    if (property.PropertyType == typeof(long))
                                        property.SetValue(test, (long)sql_reader[property.Name]);
                                    else if (property.PropertyType == typeof(DateTime))
                                        property.SetValue(test, DateTime.Parse((string)sql_reader[property.Name]));
                                    else if (property.PropertyType == typeof(double))
                                        property.SetValue(test, (double)sql_reader[property.Name]);
                                }
                                test.Attachments = GetAttachments(test.ID, test_type.FullName);
                                tests.Add(test);
                            }
                        }
                    }
                }
                return tests;
            }
            catch (IndexOutOfRangeException)
            {
                throw new SQLiteException(SQLiteErrorCode.Corrupt, "Файл базы данных был испорчен и не может быть открыт!");
            }
        }
        /// <summary>
        /// Удалить записи о пациента из БД
        /// </summary>
        /// <param name="patient_id"></param>
        public void RemovePatient(long patient_id)
        {
            using (SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_str, true))
            {
                sql_conn.Open();
                using (SQLiteCommand sql_command = new SQLiteCommand(sql_conn))
                {
                    sql_command.CommandText = "DELETE FROM Patient WHERE ID = " + patient_id.ToString();
                    sql_command.ExecuteNonQuery();
                    string[] tests = new string[]{
                        "CommonBloodTest", "BioChemTest", "DailyExcreationTest",
                        "UreaColorTest", "CommonUreaTest", "TitrationTest", "UreaStoneTest", "ProcedureInfo"
                    };
                    foreach (var test in tests)
                    {
                        sql_command.CommandText = "DELETE FROM " + test + " WHERE PatientID = " + patient_id.ToString();
                        sql_command.ExecuteNonQuery();
                    }
                    if (DBUpdated != null)
                        DBUpdated(this, null);
                }
            }
        }
        
        
        public Dictionary<string, TestNorms> GetTestNorms(Type test_type)
        {
            Assembly assembly_ = this.GetType().Assembly;
            using (Stream stream = assembly_.GetManifestResourceStream("SanusVitae.med_data.Norms.xml"))
            {
                var norms_list = new Dictionary<string, TestNorms>();
                var indicators = test_type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var norms_document = XDocument.Load(stream).Root.Elements("test").Where(x => x.Attribute("name").Value.Equals(test_type.Name)).ToArray()[0];
                foreach (var indicator in indicators.Where(x => x.IsDefined(typeof(TestInfo))))
                {
                    var indicator_norms = norms_document.Elements("indicator").Where(x => x.Attribute("name").Value.Equals(indicator.Name)).ToArray()[0];
                    TestNorms test_norms = new TestNorms();
                    test_norms.Min = Double.Parse(indicator_norms.Element("min").Value, CultureInfo.InvariantCulture);
                    test_norms.MinError = Double.Parse(indicator_norms.Element("min").Attribute("error").Value, CultureInfo.InvariantCulture);
                    test_norms.Max = Double.Parse(indicator_norms.Element("max").Value, CultureInfo.InvariantCulture);
                    test_norms.MaxError = Double.Parse(indicator_norms.Element("max").Attribute("error").Value, CultureInfo.InvariantCulture);
                    
                    norms_list.Add(indicator.Name, test_norms);
                }
                return norms_list;
            }
        }
        private void AddAttachment(List<Attachment> attachments)
        {
            using (SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_str, true))
            {
                sql_conn.Open();
                using (SQLiteCommand sql_command = new SQLiteCommand(sql_conn))
                {
                    foreach (var attachment in attachments)
                    {
                        sql_command.CommandText = CreateSQLQuery(attachment, SQLCmd.Insert, String.Empty, typeof(Attachment));
                        sql_command.Parameters.Add(new SQLiteParameter("Item", DbType.Binary) { Value = attachment.Item});
                        sql_command.ExecuteNonQuery();
                    }
                }
            }
        }
        private List<Attachment> GetAttachments(long test_id, string test_owner)
        {
            List<Attachment> attachments_ = new List<Attachment>();
            using (SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_str, true))
            {
                sql_conn.Open();
                using (SQLiteCommand sql_command = new SQLiteCommand(sql_conn))
                {
                    sql_command.CommandText = CreateSQLQuery(null, SQLCmd.Select, String.Format("WHERE TestID = {0} AND TableOwner = '{1}'", test_id, test_owner), typeof(Attachment));
                    using (SQLiteDataReader sql_reader = sql_command.ExecuteReader())
                    {
                        while (sql_reader.Read())
                        {
                            Attachment attachment_ = new Attachment()
                            {
                                TestID = test_id,
                                ID = (long)sql_reader["ID"],
                                Name = (string)sql_reader["Name"],
                                TableOwner = test_owner,
                                Extension = (string)sql_reader["Extension"]
                            };
                            if(!DBNull.Value.Equals(sql_reader["Item"]))
                                attachment_.Item = (byte[]) sql_reader["Item"];
                            attachments_.Add(attachment_);
                        }
                    }
                }
            }
            return attachments_;
        }
        /// <summary>
        /// Метод формирования sql-запроса
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="sql_cmd"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private string CreateSQLQuery(object table, SQLCmd sql_cmd, string condition, Type table_type)
        {
            string query = "";
            PropertyInfo[] properties = table_type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => !x.IsDefined(typeof(Transient))).ToArray();
            if (sql_cmd == SQLCmd.CreateTable) //запрос создания таблицы в БД
            {
                query += "CREATE TABLE IF NOT EXISTS" + " " + table_type.Name + " (";
                for (int i = 0; i < properties.Length; i++)
                {
                    string sql_type = "";
                    Type property_type = properties[i].PropertyType;
                    if (properties[i].GetCustomAttribute<PrimaryKey>() != null)
                        sql_type = "INTEGER PRIMARY KEY";
                    else if (property_type == typeof(double))
                        sql_type = "REAL";
                    else if (property_type == typeof(long))
                        sql_type = "INTEGER";
                    else if (property_type == typeof(int))
                        sql_type = "INT";
                    else if (property_type.IsEnum)
                        sql_type = "INT";
                    else if (property_type == typeof(byte[]))
                        sql_type = "BLOB";
                    else
                    {
                        if (Attribute.IsDefined(property_type, typeof(LengthAttribute)))
                            sql_type = "VARCHAR(" + property_type.GetCustomAttribute<LengthAttribute>().Length.ToString() + ")";
                        else
                            sql_type = "TEXT";
                    }
                    query += properties[i].Name + " " + sql_type;
                    if (i == properties.Length - 1)
                        query += ")";
                    else query += ", ";
                }
            }
            else if (sql_cmd == SQLCmd.Insert || sql_cmd == SQLCmd.Update) //запрос добавления записи данных в БД
            {
                query += "INSERT OR REPLACE INTO " + table_type.Name + " (";
                string values = "VALUES (";
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].GetCustomAttribute<PrimaryKey>(false) != null && sql_cmd == SQLCmd.Insert)
                        continue;
                    query += properties[i].Name;
                    if(properties[i].PropertyType == typeof(long))
                        values += String.Format("{0}", properties[i].GetValue(table));
                    if(properties[i].PropertyType.IsEnum)
                        values += String.Format("{0}", ((int)properties[i].GetValue(table)));
                    if (properties[i].PropertyType == typeof(DateTime))
                        values += String.Format("'{0}'", properties[i].GetValue(table).ToString());
                    else if (properties[i].PropertyType == typeof(string))
                        values += String.Format("'{0}'", (string)properties[i].GetValue(table));
                    else if (properties[i].PropertyType == typeof(double))
                        values += ((double)properties[i].GetValue(table)).ToString(CultureInfo.InvariantCulture);
                    else if (properties[i].PropertyType == typeof(byte[]))
                        values += "@" + properties[i].Name;
                    //else values += String.Format("{0}", properties[i].GetValue(table).ToString());
                    if (i == properties.Length - 1)
                        query += ") " + values + ")";
                    else
                    {
                        query += ", ";
                        values += ", ";
                    }
                }
            }
            else if (sql_cmd == SQLCmd.Select) //запрос выборки данных из таблицы БД
            {
                query += "SELECT * FROM " + table_type.Name + " " + condition;
            }
            else if (sql_cmd == SQLCmd.Delete)  //запрос удаления данных из таблицы БД
            {
                query += "DELETE FROM " + table_type.Name + " " + condition;
            }
            return query;
        }
        public event System.Windows.RoutedEventHandler DBUpdated;
    }
}
