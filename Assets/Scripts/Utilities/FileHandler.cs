using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class FileHandler
    {

        public static readonly string BASE_NAME = "Data.json";

        public static string AddJObjectsInFile<Type>(List<Type> list, string fileName = "Data.json", string path = "")
        {
            path = ReplaceEmptyPath(path, fileName);
            var datas = ReadJObjectsFromFile<Type>(fileName: fileName, path: path);
            foreach (var type in list) {
                datas.Add(type);
            }
            return WriteJsonInFile(datas, fileName: fileName, path: path);
        }

        public static string AddJObjectInFile<Type>(Type type, string fileName = "Data.json", string path = "")
        {
            path = ReplaceEmptyPath(path, fileName);
            var datas = ReadJObjectsFromFile<Type>(fileName: fileName, path: path);
            if (datas != null) {
                datas.Add(type);
                return WriteJsonInFile(datas, fileName: fileName, path: path);
            }
            return WriteJsonInFile(type, fileName: fileName, path: path);
        }

        public static string GetDesktopPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public static string WriteJsonInFile<T>(List<T> data, string fileName = "Data.json", string path = "")
        {
            path = ReplaceEmptyPath(path, fileName);
            string json = JsonUtility.ToJson(data);
            WriteInFile(path: path, text: json);
            return path;
        }

        public static string WriteJsonInFile<T>(T data, string fileName = "Data.json", string path = "")
        {
            string json = JsonUtility.ToJson(data);
            WriteInFile(path: path, fileName: fileName, text: json);
            return path;
        }

        //Deprecated
        public static List<T> ReadJsonInFile<T>(string fileName = "Data.json", string path = "")
        {
            path = ReplaceEmptyPath(path, fileName);
            using (StreamReader r = new StreamReader(path)) {
                string json = r.ReadToEnd();
                List<T> items = JsonUtility.FromJson<List<T>>(json);
                return items;
            }
        }

        //Deprecated
        public static Dictionary<Key, Value> ReadJsonInFile<Key, Value>(string fileName = "Data.json", string path = "")
        {
            path = ReplaceEmptyPath(path, fileName);
            return JsonUtility.FromJson<Dictionary<Key, Value>>(ReadInFile(path));
        }

        public static List<Type> ReadJObjectsFromFile<Type>(string fileName = "Data.json", string path = "")
        {
            path = ReplaceEmptyPath(path, fileName);
            var json = File.ReadAllText(path);
            if (!json.Contains("[")) {
                json = "[\n" + json;
                json += "]";
            }
            return JsonUtility.FromJson<List<Type>>(json);
        }

        public static bool FileExist(String path)
        {
            return File.Exists(path);
        }

        public static void CreateFile(String path, String text_info)
        {
            try {
                if (File.Exists(path)) {
                    File.Delete(path);
                }
                using (FileStream fs = File.Create(path)) {
                    Byte[] info = new UTF8Encoding(true).GetBytes(text_info);
                    fs.Write(info, 0, info.Length);
                }
                using (StreamReader sr = File.OpenText(path)) {
                    string s = "";
                    while ((s = sr.ReadLine()) != null) {
                        Console.WriteLine(s);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public static T GetLastJsonElement<T>(string fileName = "Data.json", string path = "")
        {
            var items = ReadJsonInFile<T>(fileName, path);
            if (items.Count != 0) {
                return items.Last();
            }
            return default(T);
        }

        public static void TryCreateFile(string path, bool delete = false)
        {
            try {
                if (File.Exists(path)) {
                    if (!delete) {
                        return;
                    }
                    File.Delete(path);
                }
                CreateFile(path);
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void CreateFile(string path)
        {
            using (StreamReader sr = File.OpenText(path)) {
                string s = "";
                while ((s = sr.ReadLine()) != null) {
                    Console.WriteLine(s);
                }
            }
        }

        public static void WriteInFile(string text = "", string fileName = "Data.json", string path = "")
        {
            path = ReplaceEmptyPath(path, fileName);
            using (StreamWriter writetext = new StreamWriter(path)) {
                writetext.WriteLine(text);
            }
        }

        public static void WriteInFile(List<string> list, string fileName = "Data.json", string path = "")
        {
            path = ReplaceEmptyPath(path, fileName);
            using (StreamWriter writetext = new StreamWriter(path)) {
                foreach (var item in list) {
                    writetext.WriteLine(item);
                }
            }
        }

        public static void ClearFile(string fileName = "Data.json", string path = "")
        {
            WriteInFile(text: "", fileName: fileName, path: path);
        }

        public static void RewriteInFile(string text, string fileName = "Data.json", string path = "")
        {
            path = ReplaceEmptyPath(path, fileName);
            WriteInFile(path: path, text: ReadInFile(path) + text);
        }

        public static String ReadInFile(String path)
        {
            using (StreamReader streamReader = new StreamReader(path)) {
                return streamReader.ReadToEnd();
            }
        }

        public static string GetPathFromSolution(string path)
        {
            var solutionPath = GetSolutionPath();
            return Path.GetFullPath(Path.Combine(solutionPath, path));
        }

        public static string GetSolutionPath()
        {
            return Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        }

        private static string ReplaceEmptyPath(string path, string fileName)
        {
            if (path == "") {
                return Environment.CurrentDirectory + "\\" + fileName;
            }
            FileAttributes fileAttributes = File.GetAttributes(path);
            if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory) {
                return path + "\\" + fileName;
            }
            return path;
        }
    }
}
