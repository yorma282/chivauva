using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartTech
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Authorization());
        }
    }
    static class database
    {
        public static string connectionString = @"Data Source=SmartTech.db;Integrated Security=False; MultipleActiveResultSets=True";

    }
    static class table_equipment
    {
        public static string main = "equipment";
        public static string id = "id";
        public static string name = "name";
        public static string type = "type";
        public static string purchase_date = "purchase_date";
        public static string responsible = "responsible";
        public static string classroom = "classroom";
        public static string status = "status";

    }

}
