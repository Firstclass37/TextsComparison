using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TextsComparison
{
    class Program
    {

        /// <summary>
        /// Метод входа в программу
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            do
            {
                Console.Clear();
                try
                {
                    string path1;
                    string path2;
                    int startWithLine;
                    int endWithLine;
                    int[] linesForSkip;
                    GetPaths(out path1, out path2);
                    GetAdditionalSettings(out startWithLine,out endWithLine,out linesForSkip);
                    CompareTexts(path1, path2,linesForSkip, startWithLine,endWithLine);
                    ShowMessage("Complete!!!", ConsoleColor.Green);                   
                }
                catch (Exception e)
                {
                    ShowMessage(String.Format("Ooooops:( Error: {0}", e.Message), ConsoleColor.Red);
                }

            } while (AskYnQuestion("Do you want to use it again? "));

        }       
        /// <summary>
       /// Метод сравнивает два текста
       /// </summary>
       /// <param name="path1">Путь к этулонному файлу</param>
       /// <param name="path2">Пусть к целевому файлу</param>
       /// <param name="linesForSkip">Массив строк, исключающихся из сравнения</param>
       /// <param name="startWith">Стартовая строка</param>
       /// <param name="endWith">Конечная строка</param>
        private static void CompareTexts(string path1, string path2,int[] linesForSkip, int startWith = 0, int endWith = 0)
        {
            TextReader stream1 = new StreamReader(path1, Encoding.Default);
            TextReader stream2 = new StreamReader(path2, Encoding.Default);
            try
            {
                string line1;
                string line2;
                int lineNumber = 1;
                while (!String.IsNullOrEmpty(line1 = stream1.ReadLine()) |
                       !String.IsNullOrEmpty(line2 = stream2.ReadLine()))
                {
                    if (startWith > 0 && lineNumber < startWith)
                    {
                        lineNumber++;
                        continue;
                    }
                    if (endWith > 0 && lineNumber > endWith)
                    {
                        return;
                    }
                    if (linesForSkip.Contains(lineNumber))
                    {
                        lineNumber++;
                        continue;
                    }
                    
                    int position;
                    if (FindLineDifference(line1, line2, out position))
                    {
                        ShowDifference(line1, line2, position, lineNumber);
                        return;
                    }
                    lineNumber++;
                }
                
                 ShowMessage("No differences between texts", ConsoleColor.Green);
                
            }
            catch
            {
                throw;
            }
            finally
            {
                stream1.Close();
                stream2.Close();
            }          
        }
        /// <summary>
        /// Сравнивает две строки. Если находит различие, возвращает true и задает позицию расскождения в out position
        /// </summary>
        /// <param name="line1">Строка из эталонного файла</param>
        /// <param name="line2">Строка из целевого файла</param>
        /// <param name="position">Позиция перовго найденного рассхождения</param>
        /// <returns>Если находит различие, возвращает true и задает позицию расскождения в out position</returns>
        private static bool FindLineDifference(string line1, string line2,out int position)
        {
            position = 0;
            if (!line1.Equals(line2))
            {             
                for (int i = 0; i < line1.Length && i < line2.Length; i++)
                {
                    char one = line1[i];
                    char two = line2[i];
                    if (one != two)
                    {
                        position = i;
                        return true;
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Предоставляет диалог для пользователя для ввода путей к файлам
        /// </summary>
        /// <param name="path1">Строка, куда запишется путь к эталонному файлу</param>
        /// <param name="path2">Стирока, куда запишется путь к целевому файлу</param>
        private static void GetPaths(out string path1, out string path2)
        {
            Console.Write("Path for standart file: ");
            path1 = Console.ReadLine();
            Console.Write("Path for target file: ");
            path2 = Console.ReadLine();
            CheckPaths(path1,path2);
        }
        /// <summary>
        /// Метод предоставляет диалог для пользователя для получения дополнительных настроек
        /// </summary>
        /// <param name="startWith">Переменная для записи стартовой строки(0 если не будет задана)</param>
        /// <param name="endWith">Переменная для записи конечной строки(0 если не будет задана)</param>
        /// <param name="linesNum">Массив для записи массива строк, которые необходимо исключить из сравнения(пустой если не будет задана)</param>
        private static void GetAdditionalSettings(out int startWith, out int endWith,out int[] linesNum)
        {     
            Console.Write("Start with... (set start line if you need or press enter with empty value): ");
            Int32.TryParse(Console.ReadLine(),out startWith);
            Console.Write("End with... (set end line if you need or press enter with empty value):");
            Int32.TryParse(Console.ReadLine(), out endWith);
            Console.Write("Skip lines... (if you need you can skip some lines, enter numbers separated by spaces):");
            string[] linesString = Console.ReadLine().Split();
            linesNum = new int[linesString.Length];
            for (int i = 0; i < linesString.Length; i++)
            {
                Int32.TryParse(linesString[i],out linesNum[i]);
            }
        }
        /// <summary>
        /// Метод проверяет, существуют ли файлы на диске. Выбрасывает исключения, если один из файлов не существует
        /// </summary>
        /// <param name="path1">Путь к файлу</param>
        /// <param name="path2">Путь к файлу</param>
        private static void CheckPaths(string path1, string path2)
        {
            if (!File.Exists(path1))
            {
                throw new IOException(String.Format("File with path <{0}> didnt found",path1));
            }
            if (!File.Exists(path2))
            {
                throw new IOException(String.Format("File with path <{0}> didnt found", path2));
            }
        }
        /// <summary>
        /// Метод выводит пользователю информацию о рассхождении в строках
        /// </summary>
        /// <param name="line1">Первая строка</param>
        /// <param name="line2">Вторая строка</param>
        /// <param name="index">Позиция первого рассхождения в строке</param>
        /// <param name="lineNumber">Номер строки</param>
        private static void ShowDifference(string line1, string line2, int index,int lineNumber)
        {
            Console.WriteLine("The first difference was found in line - {0} position - {1}", lineNumber,index);
            Console.WriteLine();
            Console.WriteLine("*****Line from standart text*****");
            if (!string.IsNullOrEmpty(line1))
            {

                Console.Write(line1.Substring(0, index));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(line1[index] != ' ' ? line1[index].ToString() : "[ ]");
                Console.ResetColor();
                Console.WriteLine(line1.Substring(index + 1));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("<The current line is empty>");
                Console.WriteLine();
                Console.ResetColor();
            }
            Console.WriteLine("*****Line from target text*****");
            if (!string.IsNullOrEmpty(line2))
            {
                Console.Write(line2.Substring(0, index));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(line2[index] != ' ' ? line2[index].ToString() : "[ ] ");
                Console.ResetColor();
                Console.WriteLine(line2.Substring(index + 1));
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("<The current line is empty>");
                Console.ResetColor();
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Метод показывает заданное сообщение пользователю заданным цветом
        /// </summary>
        /// <param name="message">Строка сообщения</param>
        /// <param name="color">Цвет сообщения</param>
        private static void ShowMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        /// <summary>
        /// Метод показывает заданный вопрос пользователю, ожидая ответа yes-no
        /// </summary>
        /// <param name="question">Вопрос</param>
        /// <returns>Возвращает true, если ответ положительный(y). False в любом другом случае</returns>
        private static bool AskYnQuestion(string question)
        {
            Console.Write(question + "(y/n)");
            char result = Console.ReadKey().KeyChar;
            return result == 'y' ? true : false;
        }


    }
}
