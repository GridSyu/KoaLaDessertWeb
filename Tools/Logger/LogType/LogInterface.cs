using KoaLaDessertWeb.Tools.Logger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoaLaDessertWeb.Tools.Logger.LogType
{
    /// <summary>
    /// Log
    /// </summary>
    internal interface LogInterface
    {
        /// <summary>
        /// log存放的根目錄
        /// </summary>
        public string RootFilePath { get; set; }

        /// <summary>
        /// 寫入log訊息
        /// </summary>
        public void Write(string message, string from = "any");

        /// <summary>
        /// 讀取log訊息
        /// </summary>
        public List<LogOutput>? ReadByUTCTime(DateTimeOffset startDate, DateTimeOffset endDate);

        
    }
}
