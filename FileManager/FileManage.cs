/////////////////////////////////////////////////////////////////////
// FileManager.cs -   This package provides functionality          //
//                    of handling sending and receiving of files   //
//                    between all the storage directories          //
// ver 1.0                                                         //
// Language:    C#, Visual Studio 2017                             //
// Platform:    Lenovo ideapad 500, Windows 10                     //
// Application: Build Server                                       //
//                                                                 //
// Name : Nupur Kulkarni                                           //
// CSE681: Software Modeling and Analysis, Fall 2017               //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * -------------------
 * Provides functionality to send and receive files between directories dedicated for
 * storing data by repository, builder, test harness and client
 * 
 * Public Interface:
 * ================= 
 * void getFiles(string path, string pattern): For getting files from directory matching pattern
 * bool sendFile(string fileSpec, string dest): for sending file to correct destination
 * 
 * Build Process:
 * --------------
 * Required Files:  
   Build Command: csc FileManager.cs

 * Maintenance History:
    - Ver 1.0 Oct 2017 
 * --------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class FileManage
    {
        //compiler generates private backing store
        
        //all paths are specified.
        public string storagePath { get; set; } = "../../../RepoStorage";
        public string buildPath { get; set; } = "../../../BuildStorage";
        public string testPath { get; set; } = "../../../TestStorage";
        public string clientPath { get; set; } = "../../../ClientStorage";
        private List<string> files { get; set; } = new List<string>();

        public FileManage()
        {
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);
            if (!Directory.Exists(buildPath))
                Directory.CreateDirectory(buildPath);
            if (!Directory.Exists(testPath))
                Directory.CreateDirectory(testPath);
            if (!Directory.Exists(clientPath))
                Directory.CreateDirectory(clientPath);
        }

        /*----< helper for getFiles method ------*/

        private void getFiles(string path, string pattern)
        {
            string[] tempFiles = Directory.GetFiles(path, pattern);
            for (int i = 0; i < tempFiles.Count(); ++i)
            {
                tempFiles[i] = Path.GetFullPath(tempFiles[i]);
            }
            files.AddRange(tempFiles);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                getFiles(dir, pattern);
            }
        }

        /*-----< get files >----------*/
        public void getFiles(string pattern)
        {
            files.Clear();
            getFiles(storagePath, pattern);
        }

        /*-----<send file>-------*/
        public bool sendFile(string fileSpec, string dest)
        {
            try
            {
                string fileName = Path.GetFileName(fileSpec);
                string destSpec;
                //deciding proper destination
                if (dest.CompareTo("builder") == 0)
                {
                    destSpec = Path.Combine(buildPath, fileName);
                }
                else if(dest.CompareTo("tester") == 0)
                {
                    destSpec = Path.Combine(testPath, fileName);
                }
                else if(dest.CompareTo("client") == 0)
                {
                    destSpec = Path.Combine(clientPath, fileName);
                }
                else
                {
                    destSpec = "";
                }
                File.Copy(fileSpec, destSpec, true);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write("\n---{0}---\n", ex.Message);
                return false;
            }
        }

    }


    class FileManageTest
    {
#if (TEST_FILEMANAGER)
        static void Main(string[] args)
        {
            FileManager fm = new FileManager();
            fm.getFiles("*.*");
            foreach(string file in repo.files)
            {
                Console.Write("\n  \"{0}\"", file);
                Console.Write("\n Sending \"{0}\" to \"{1}\"", file, repo.receivePath);
                repo.sendFile(file);
            }
            
            Console.Write("\n\n");
        }
#endif
    }
}
