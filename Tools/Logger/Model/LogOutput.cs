using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace KoaLaDessertWeb.Tools.Logger.Model
{
    /// <summary>
    /// log輸出
    /// </summary>
    internal class LogOutput
    {
        /// <summary>
        /// log類型
        /// </summary>
        public string LogType { get; set; }

        /// <summary>
        /// 時間戳記
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// log內文
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// log來源
        /// </summary>
        public string From { get; set; }
    }
}
