using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Reflection;

namespace MotorSelector.Models
{
    class ModelHelper<T> where T : new()
    {
        private readonly string connectionString = "";
        public ModelHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public T GetRecord(int id)
        {
            T t = new T();
            var targetType = t.GetType();
            var instance = Activator.CreateInstance(targetType);
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                string sqlQuery = String.Format("SELECT * FROM {0} where id=@id", targetType.Name);
                using (SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int i = 0;
                            var propertyList = targetType.GetProperties().ToList();
                            //get field names of database
                            var fields = new List<string>();
                            for (int k = 0; k < reader.FieldCount; k++)
                            {
                                fields.Add(reader.GetName(k));
                            }

                            if (TrackSort(fields, ref propertyList))
                            {
                                foreach (var propertyInfo in propertyList)
                                {
                                    propertyInfo.SetValue(instance, reader[i]);
                                    i++;
                                }
                            }
                            else
                            {
                                throw new Exception("field name not compatible");
                            }
                        }
                    }
                }
            }
            var result = (T)instance;
            return result;
        }

        public List<T> GetAllRecords()
        {
            T t = new T();
            var targetType = t.GetType();
            List<T> list = new List<T>();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                string sqlQuery = String.Format("SELECT * FROM {0}", targetType.Name);
                using (SqlCommand cmd = new SqlCommand(sqlQuery, sqlConn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var instance = Activator.CreateInstance(targetType);
                            int i = 0;
                            var propertyList = targetType.GetProperties().ToList();
                            //get field names of database
                            var fields = new List<string>();
                            for (int k = 0; k < reader.FieldCount; k++)
                            {
                                fields.Add(reader.GetName(k));
                            }

                            if (TrackSort(fields, ref propertyList))
                            {
                                foreach (var propertyInfo in propertyList)
                                {
                                    propertyInfo.SetValue(instance, reader[i]);
                                    i++;
                                }
                            }
                            else
                            {
                                throw new Exception("field name not compatible");
                            }
                            list.Add((T)instance);
                        }
                    }
                }
            }
            return list;
        }

        //the name of propertyinfo of list follow target strings to sort
        private bool TrackSort(IEnumerable<string> targets, ref List<PropertyInfo> list)
        {
            var resultList = new List<PropertyInfo>();
            foreach (var target in targets)
            {
                int count = 0;
                foreach (var item in list)
                {
                    if (String.Equals(item.Name, target, StringComparison.OrdinalIgnoreCase))
                    {
                        resultList.Add(item);
                        break;
                    }
                    count++;
                    if (count == list.Count())
                    {
                        //search completed and there is no the same property name founded
                        return false;
                    }
                }
            }
            list = resultList;
            return true;
        }
    }
}