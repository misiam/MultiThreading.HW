using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.HW.IANA.ZoneServer.Helpers
{
    public class Logger
    {
        private Logger()
        {

        }
        private static Logger _logger = new Logger();

        public static Logger Instance
        {
            get {  return _logger; }
        }

        public void Log(string message, params object[] args)
        {
            Console.WriteLine($"[{DateTime.Now}]" + message, args);
        }
        public T WrapWithTimeLog<T>(Func<T> action, string name)
        {
            DateTime startTime = DateTime.Now;
            Console.WriteLine($"[{startTime}] START {name}");
            T result = action();
            DateTime endTime = DateTime.Now;
            var time = endTime - startTime;
            Console.WriteLine($"[{endTime}] END {name}. Total(sec): {time.TotalSeconds}");
            return result;
        }
    }   

    
}
