using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace AA_Latest_Log_finder
{
    public class Class1
    {
        public static void AA_Log_finder(string searchTexts, string startDate, string endDate)
        {
            string latestLogPath = "C:/ProgramData/AutomationAnywhere/BotRunner/Logs/Bot_Launcher_Latest.txt";
            if (File.Exists(latestLogPath))
            {
                using (TextWriter tw = new StreamWriter(latestLogPath, false))
                {
                    tw.WriteLine("");
                }

            }
            string month = DateTime.Now.ToString("MMM");
            string year = DateTime.Now.ToString("yyyy");
            string date = DateTime.Now.ToString("dd");

            string currentMonthFolder = year + "-" + month;
            string[] filePaths = Directory.GetFiles(@"C:\ProgramData\AutomationAnywhere\BotRunner\Logs\" + currentMonthFolder);
            DateTime[] modifiedDates = new DateTime[filePaths.Length];
            for (int i = 0; i < filePaths.Length; i++)
            {
                modifiedDates[i] = File.GetLastWriteTime(filePaths[i]);
            }
            DateTime latestDate = modifiedDates.Max();
            string LatestLogFile = "";
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (File.GetLastWriteTime(filePaths[i]) == latestDate)
                {
                    LatestLogFile = filePaths[i];
                    break;
                }
            }
            int index = LatestLogFile.IndexOf(".zip");
            string fileName = LatestLogFile.Substring(0, index);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            ZipFile.ExtractToDirectory(LatestLogFile, "C:/ProgramData/AutomationAnywhere/BotRunner/Logs/" + currentMonthFolder);


            string[] lines = File.ReadAllLines(fileName);
            string[] searchTextsArray = searchTexts.Split(',').ToArray();
            foreach (string line in lines)
            {
                //get date 
                string[] splitted = line.Split(' ').ToArray();
                string dateString = splitted[0] + " " + splitted[1] + " " + splitted[2];
                DateTime logDatetime = Convert.ToDateTime(dateString);
                DateTime startDate_ = Convert.ToDateTime(startDate);
                DateTime endDate_ = Convert.ToDateTime(endDate);
                if (startDate_ <= logDatetime && logDatetime <= endDate_)
                {
                    foreach (string text in searchTextsArray)
                    {
                        if (line.ToLower().Contains(text.ToLower()))
                        {
                            CreateOrUpdateFile(latestLogPath, line);
                        }
                    }
                }
            }
            GetLatestNodeManagerLog(searchTexts, startDate, endDate);
           
        }

        private static void GetLatestNodeManagerLog(string searchTexts, string startDate, string endDate)
        {
            string latestLogPath = "C:/ProgramData/AutomationAnywhere/BotRunner/Logs/Node_Manager_Latest.txt";
            if (File.Exists(latestLogPath))
            {
                using (TextWriter tw = new StreamWriter(latestLogPath, false))
                {
                    tw.WriteLine("");
                }

            }
            string sourceFile = @"C:\ProgramData\AutomationAnywhere\BotRunner\Logs\Node_Manager.log";
            string destinationFile = @"C:\ProgramData\AutomationAnywhere\BotRunner\Logs\Node_ManagerCopy.log";
            try
            {
                File.Copy(sourceFile, destinationFile, true);
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);

            }
            string[] lines = File.ReadAllLines(destinationFile);
            string[] searchTextsArray = searchTexts.Split(',').ToArray();
            foreach (string line in lines)
            { 
                try
                {
                    string[] splitted = line.Split(' ').ToArray();
                    string dateString = splitted[0] + " " + splitted[1] + " " + splitted[2];
                    DateTime logDatetime = Convert.ToDateTime(dateString);
                    DateTime startDate_ = Convert.ToDateTime(startDate);
                    DateTime endDate_ = Convert.ToDateTime(endDate);
                    if (startDate_ <= logDatetime && logDatetime <= endDate_)
                    {
                        foreach (string text in searchTextsArray)
                        {
                            if (line.ToLower().Contains(text.ToLower()))
                            {
                                CreateOrUpdateFile(latestLogPath, line);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            File.Delete(destinationFile);
        }


        private static void CreateOrUpdateFile(string path, string line)
        {

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();

                using (TextWriter tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(line);
                }

            }
            else if (File.Exists(path))
            {
                using (TextWriter tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(line);
                }
            }
        }
    }
}
