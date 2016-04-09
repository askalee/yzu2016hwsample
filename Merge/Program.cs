using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Merge
{
    class Program
    {
        static void Main(string[] args)
        {
            bool quitFlag = false;
            string fileNameData = "";
            string fileNameTemplate = "";
            string fileNameResult = "";
            string fileNameCommon = "";
            bool hasCommon = false;
            for (int i = 0; (i < args.Length) && !quitFlag; i++)
            {
                switch (args[i])
                {
                    case "-i":
                        fileNameData = args[i + 1];
                        ++i;
                        break;
                    case "-t":
                        fileNameTemplate = args[i + 1];
                        ++i;
                        break;
                    case "-r":
                        fileNameResult = args[i + 1];
                        ++i;
                        break;
                    case "-c":
                        fileNameCommon = args[i + 1];
                        hasCommon = true;
                        ++i;
                        break;
                    default:
                        Console.WriteLine("Wrong argument...\nQuit...");
                        quitFlag = true;
                        break;
                }
            }
            if (!quitFlag && File.Exists(fileNameData) && File.Exists(fileNameTemplate))
            {
                StreamReader srData = new StreamReader(fileNameData, Encoding.Default);
                StreamReader srTemplate = new StreamReader(fileNameTemplate, Encoding.Default);

                // titleTerm for all terms' title
                ArrayList titleTerm = new ArrayList();
                string[] termOfData = srData.ReadLine().Split('\t');
                ProcessDataTerm(titleTerm, termOfData);
                int dataTermCount = titleTerm.Count;
                // Read all data terms
                ArrayList dataTerm = new ArrayList();                
                while (!srData.EndOfStream)
                {
                    termOfData = srData.ReadLine().Split('\t');
                    ProcessDataTerm(dataTerm, termOfData);
                }
                // Process template
                string template = "";
                if(!hasCommon) {
                    template = ProcessTemplate(srTemplate, titleTerm);
                    ResultOutput(template, dataTerm, dataTermCount, fileNameResult);
                }
                else
                {
                    if (File.Exists(fileNameCommon))
                    {
                        StreamReader srCommon = new StreamReader(fileNameCommon, Encoding.Default);
                        ArrayList commonTerm = ProcessCommonTerm(titleTerm, srCommon);
                        int commonTermCount = commonTerm.Count;
                        template = ProcessTemplate(srTemplate, titleTerm);
                        ResultOutput(template, dataTerm, commonTerm, dataTermCount, fileNameResult);
                        srCommon.Close();
                    }
                    else
                    {
                        quitFlag = true;
                    }                    
                }
                srData.Close();
                srTemplate.Close();
            }
            else
            {
                Console.WriteLine("Error occur...\nQuit...");
            }
            Console.ReadLine(); // As system("pause");
        }

        // Useless in this project, left for string process reference.
        //static string ProcessTemplate(StreamReader sr)
        //{
        //    string template = sr.ReadToEnd();
        //    string processedTemplate = "";
        //    for (int i = 0; template.IndexOf('$') > -1; i++)
        //    {
        //        int first = template.IndexOf('$');
        //        processedTemplate += template.Substring(0, first);
        //        processedTemplate += "{" + i.ToString() + "}";
        //        int last = template.IndexOf('}');
        //        int length = (template.Length - last) - 1;
        //        template = template.Substring((last + 1), length);
        //    }
        //    processedTemplate += template;
        //    return processedTemplate;
        //}

        static string ProcessTemplate(StreamReader sr, ArrayList titleTerm)
        {
            string template = sr.ReadToEnd();
            for (int i = 0; i < titleTerm.Count; i++)
            {
                string target = "${" + titleTerm[i] + "}";
                string replacement = "{" + i.ToString() + "}";
                template = template.Replace(target, replacement);
            }
            return template;
        }

        static ArrayList ProcessDataTerm(ArrayList arl, string[] terms)
        {
            for (int i = 0; i < terms.Length; i++)
            {
                if (terms[i].Length > 0)
                {
                    arl.Add(terms[i]);
                }
            }
            return arl;
        }

        // Read in common title and common data, return common data list
        static ArrayList ProcessCommonTerm(ArrayList titleTerm, StreamReader sr)
        {
            ArrayList commonData = new ArrayList();
            while (!sr.EndOfStream)
            {
                string[] parse = sr.ReadLine().Split('=');
                titleTerm.Add(parse[0]);
                commonData.Add(parse[1]);
            }
            return commonData;
        }

        static void ResultOutput(string template, ArrayList dataTerm, int numTerm, string fileName)
        {
            StreamWriter swResult = new StreamWriter(fileName);
            string[] data = new string[numTerm];
            for (int i = 0; i < (dataTerm.Count / numTerm); i++)
            {
                for (int j = (i * numTerm); j < ((i * numTerm)+numTerm); j++)
                {
                    data[j % numTerm] = (string)dataTerm[j];
                }
                swResult.WriteLine(template, data);
                swResult.Flush();
            }
            swResult.Close();
        }

        static void ResultOutput(string template, ArrayList dataTerm, ArrayList commonTerm, int dataTermCount, string fileName)
        {
            StreamWriter swResult = new StreamWriter(fileName);
            int titleTermCount = dataTermCount + commonTerm.Count;
            string[] data = new string[dataTermCount + commonTerm.Count];
            for (int i = 0; i < (dataTerm.Count / dataTermCount); i++)
            {
                int limiter = (i * dataTermCount) + dataTermCount;
                for (int j = (i * dataTermCount); j < limiter; j++)
                {
                    data[j % dataTermCount] = (string)dataTerm[j];
                    if (j == (limiter - 1))
                    {
                        int l = (j % dataTermCount) + 1;
                        for (int k = 0; k < commonTerm.Count; k++)
                        {
                            data[l+k] = (string)commonTerm[k];
                        }
                    }
                }
                swResult.WriteLine(template, data);
                swResult.Flush();
            }
            swResult.Close();
        }
    }
}
