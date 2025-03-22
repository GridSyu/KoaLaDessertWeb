using KoaLaDessertWeb.Tools.Logger.Model;

namespace KoaLaDessertWeb.Tools.Logger.LogType
{
    /// <summary>
    /// 異常訊息
    /// </summary>
    internal class ToolsError : LogInterface
    {
        private static string _rootFilePath = "Log"; // log存放的根目錄，預設為Log資料夾
        //private static string _logType = "normal"; // log類型
#pragma warning disable CS8602 // 可能 null 參考的取值 (dereference)。
#pragma warning disable CS8601 // 可能有 Null 參考指派。
        private static string _logType = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType?.Name; // log類型(從此檔案的名稱取得)
#pragma warning restore CS8601 // 可能有 Null 參考指派。
#pragma warning restore CS8602 // 可能 null 參考的取值 (dereference)。

        /// <summary>
        /// 設定log存放的根目錄
        /// </summary>
        public string RootFilePath
        {
            get
            {
                return _rootFilePath;
            }
            set
            {
                _rootFilePath = value;
            }
        }


        /// <summary>
        /// 寫入log訊息
        /// </summary>
        public async void Write(string message, string from = "any")
        {
            try
            {
                int endCount = 10; // 最多重試次數
                for (int i = 0; i < endCount; i++)
                {
                    try
                    {
                        // 組合訊息
                        string msg = $"{message} >> {from}";
                        // 取得目前時間的年月日
                        var currentDate = DateTimeOffset.UtcNow;
                        var currentDateString = currentDate.ToString("yyyyMMdd");
                        // 組合檔案路徑
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), _rootFilePath, $"{currentDateString}_{_logType}.log");
                        // 寫入本地檔案
                        await using (StreamWriter writer = File.AppendText(filePath))
                        {
                            writer.WriteLine($"{currentDate} => {msg}");
                        };
                        break;
                    }
                    catch (IOException)
                    {
                        // 如果檔案還無法讀寫，等待一段時間再次嘗試
                        System.Threading.Thread.Sleep(500);
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }

        }

        /// <summary>
        /// 讀取log訊息
        /// </summary>
        public List<LogOutput>? ReadByUTCTime(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            Console.WriteLine("讀取log訊息...");
            try
            {
                // 定義回傳結果
                var result = new List<LogOutput>();

                // 定義需要抓取資料的日期清單
                var dateList = new List<string>();

                string startDateUTCString = startDate.ToString("yyyy-MM-dd");
                DateTime startDateTimeUTC = DateTime.Parse(startDateUTCString);
                string endDateUTCString = endDate.ToString("yyyy-MM-dd");
                DateTime endDateTimeUTC = DateTime.Parse(endDateUTCString);

                // 結束日期減去起始日期，取得相差天數
                TimeSpan timeSpan = endDateTimeUTC.Subtract(startDateTimeUTC);
                int days = timeSpan.Days;
                for (int i = 0; i < days + 1; i++)
                {
                    dateList.Add(startDateTimeUTC.AddDays(i).ToString("yyyyMMdd"));
                }
                dateList.Reverse(); // 清單反轉


                // 批次取得日期
                foreach (var dateItem in dateList)
                {
                    // 組合檔案路徑
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), _rootFilePath, $"{dateItem}_{_logType}.log");
                    // 判斷是否有此檔案
                    if (File.Exists(filePath))
                    {
                        // 讀取檔案
                        var fileContent = File.ReadAllLines(filePath);
                        // 批次文字分割
                        foreach (var row in fileContent)
                        {
                            var rowList = row.Split("=>");
                            // 判斷資料分割是否正確
                            if (rowList.Length > 1)
                            {
                                // 判斷此筆log是否在起始與結束日期之間
                                var timestamp = DateTimeOffset.Parse(rowList[0]);
                                var startCompare = timestamp.CompareTo(startDate);
                                var endCompare = timestamp.CompareTo(endDate);
                                if (startCompare >= 0 && endCompare <= 0)
                                {
                                    // 拆解資料
                                    var rowData = rowList[1].Split(">>");
                                    // 判斷分割資料的第一筆是否有資料
                                    var content = rowData.Length > 0 ? rowData[0] : "";
                                    // 判斷分割資料的第二筆是否有資料
                                    var from = rowData.Length > 1 ? rowData[1] : "";
                                    // 插入資料
                                    result.Add(new LogOutput()
                                    {
                                        LogType = _logType,
                                        Timestamp = DateTimeOffset.Parse(rowList[0]).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                        Content = content,
                                        From = from
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        // 查無檔案不處理
                    }

                }
                return result;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
