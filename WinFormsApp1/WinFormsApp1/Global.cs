using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public static class Global
    {

        public static Hashtable OrigState = new Hashtable();//連線的客戶端集合

        public static Hashtable NewState = new Hashtable();//連線的客戶端集合

        public static void UpdateWith(this Hashtable first, Hashtable second)
        {
            foreach (DictionaryEntry item in second)
            {
                first[item.Key] = item.Value;
            }
        }
    }
}
