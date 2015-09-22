using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.Extention
{
    public static class MyExts
    {
        /// <summary>
        /// Get random string
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetRandomString(int len)
        {
            var str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var rand = new Random(Guid.NewGuid().GetHashCode());
            var builder = new StringBuilder();
            for (var i = 0; i < len; i++)
            {
                builder.Append(str[rand.Next(0, str.Length)]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Extension method for lst&lt;obj&gt; to DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> lst)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in lst)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}
