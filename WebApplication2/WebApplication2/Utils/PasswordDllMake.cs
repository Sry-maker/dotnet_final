using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WebApplication2.Utils
{
    public class PasswordDllMake
    {
        [DllImport("C:\\Users\\Derek\\Desktop\\.net期末项目\\WebApplication2\\WebApplication2\\DllPassword.dll")]
        public static extern IntPtr Encryption(string strtest);

        [DllImport("C:\\Users\\Derek\\Desktop\\.net期末项目\\WebApplication2\\WebApplication2\\DllPassword.dll")]
        public static extern IntPtr Decryption(string strtest);
    }
}
