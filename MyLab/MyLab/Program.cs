using MyLab.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MyLab.Extention;
using System.Diagnostics;

namespace MyLab
{
    class Program
    {
        static MyLabEntities db = null;
        static Stopwatch sw = null;

        static void Main(string[] args)
        {
            sw = new Stopwatch();

            db = new MyLabEntities();
            var lst = db.Product.ToList<Product>();
            //BatchInsertProd(lst);

            UpdateProd(lst);
            BatchUpdateProd(lst);                        
            Console.ReadLine();
        }

        private static string BatchInsertProd(List<Product> lstOfProd)
        {
            List<Product> lst = new List<Product>();
            for (int i = 0; i < 20000; i++)
            {
                Product prod = new Product();
                prod.id = Guid.NewGuid();
                prod.name = MyExts.GetRandomString(6);
                prod.descrp = MyExts.GetRandomString(25);
                prod.lastEditAt = DateTime.Now;
                lst.Add(prod);
            }
            db.Product.AddRange(lst);
            db.SaveChanges();

            return string.Empty;
        }

        private static void StirData(List<Product> lstOfProd)
        {
            sw.Reset();
            for (int i = 0; i < 20000; i++)
            {
                Product prod = lstOfProd[i];                
                prod.name = MyExts.GetRandomString(6);
                prod.descrp = MyExts.GetRandomString(25);
                prod.lastEditAt = DateTime.Now;
            }
        }

        private static void UpdateProd(List<Product> lstOfProd)
        {            
            StirData(lstOfProd);
            sw.Start();
            db.SaveChanges();
            sw.Stop();
            Console.WriteLine("UpdateProd m-sec: " + sw.ElapsedMilliseconds);
        }

        private static string BatchUpdateProd(List<Product> lstOfProd)
        {
            StirData(lstOfProd);
            sw.Start();
            string sqlCmd = "sp_BatchEditProd @fromClient, @opMsg out";
            string outMsg = string.Empty;

            var dt = lstOfProd.ToDataTable();
            List<SqlParameter> lstOfPars = new List<SqlParameter>() {
                                               new SqlParameter() {
                                                   Direction = ParameterDirection.Input,
                                                   ParameterName = "fromClient",
                                                   SqlDbType = SqlDbType.Structured,
                                                   TypeName = "Product",
                                                   Value = dt
                                               },
                                               new SqlParameter() {
                                                   Direction = ParameterDirection.Output,
                                                   ParameterName = "opMsg",
                                                   SqlDbType = SqlDbType.NVarChar,
                                                   Size = 40,
                                                   Value = null
                                               }
                                           };
            db.Database.ExecuteSqlCommand(sqlCmd, lstOfPars.ToArray());
            sw.Stop();
            Console.WriteLine("BatchUpdateProd m-sec: " + sw.ElapsedMilliseconds);
            return outMsg;
        }        
    }
}
