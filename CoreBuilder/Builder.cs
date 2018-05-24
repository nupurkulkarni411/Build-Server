/////////////////////////////////////////////////////////////////////
// Builder.cs -   This package provides core functionality         //
//                of building files for Build Server system        //
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
 * This Package handles the processing of commands sent by other packages 
 * and builds files sent to it by repository
 * 
 * Public Interface:
 * ================= 
 * void send(Message msg) : This unables builder to communicate with other packages
 *                          by sending message to Common request handler. 
 * void ProcessMessage(Message msg) : This enables processing of messages sent by other 
 *                                    packages and takes appropriate actions.
 * void sendLog(string log,string authorName) : Used for sending log file to repository
 * bool receiveFiles(List<string> files) : For pulling required files from repository
 * void Build(string testDriver , List<string> testCodes = null): For building files sent to builder and display output to client
 * string CreateTestRequest(): For commanding test harness to proceed with testing
 * Message CreateTestMessage(): Creating message for commanding test harness
 * 
 * Build Process:
 * --------------
 * Required Files: Builder.cs BuilderMessages.cs IFederation.cs FileManager.cs Logger.cs Messages.cs IRequestHandler.cs Serialization.cs
   Build Command: csc Builder.cs BuilderMessages.cs IFederation.cs FileManager.cs Logger.cs Messages.cs IRequestHandler.cs Serialization.cs

 * Maintenance History:
    - Ver 1.0 Oct 2017 
 * --------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileManager;
using System.IO;
using BuildServerMessages;
using Messages;
using Utilities;
using System.Diagnostics;
using Logger;
using FederationInterface;
using RequestHandlerInterface;

namespace CoreBuilder
{
    public class Builder:IFederation
    {
        protected FileManage fm = new FileManage();
        protected BuildLog bl = new BuildLog();
        private bool buildResult = false;
        private string testConfig = "";
        private List<string> testFiles { get; set; } = new List<string>();
        private TestRequest tr = new TestRequest();
        public Builder(IRequest req) : base(req)
        {
            Console.Write("\n   Constructor of builder is called.");
        }

        //This unables builder to communicate with other packages
        public void send(Message msg)
        {
            req.send(msg);
        }

        //This enables processing of messages sent by other packages and takes appropriate actions
        public void ProcessRequest(Message msg)
        {
            TestRequest newTrq = msg.body.FromXml<TestRequest>();
            tr.author = newTrq.author;
            tr.timeStamp = DateTime.Now;
            bl.createLog(newTrq.author);
            //Creating temporary directory named Build Storage
            System.IO.Directory.CreateDirectory(Path.GetFullPath(fm.buildPath));
            foreach (TestElement tl in newTrq.tests)
            {
                buildResult = false;
                testConfig = "";
                testConfig = tl.testConfiguration;
                TestElement t = new TestElement();
                t.testName = tl.testName;
                t.testConfiguration = tl.testConfiguration;
                if(tl.testConfiguration=="C#")
                    t.testDriver = Path.GetFileNameWithoutExtension(tl.testDriver) + ".dll";
                if(tl.testConfiguration == "Java")
                    t.testDriver = Path.GetFileNameWithoutExtension(tl.testDriver) + ".jar";
                testFiles.Clear();
                testFiles.Add(tl.testDriver);
                testFiles.AddRange(tl.testCodes);
                Console.WriteLine("\n  3. Builder parses the message and gets test request \n  then builder parses the test request and pulls required files from repository.");
                receiveFiles(testFiles);
                Console.WriteLine("\n  4. Builder tries to build files send to it. (Requirements 5,6)");
                Build(tl.testDriver,tl.testConfiguration, tl.testCodes);
                if(buildResult)
                    tr.tests.Add(t);
            }
            sendLog(bl.getLog(),newTrq.author,testConfig);
        }

        //Sending log to repository
        public void sendLog(string log,string authorName,string testCnfig)
        {
            string filename = "BuildLog" + authorName + testConfig + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            Console.WriteLine("\n  5. Sending created log to repository.");
            Console.WriteLine("\n\n  Sending build log: {0} to {1}", filename, Path.GetFullPath(fm.storagePath));
            // Create the file.
            using (FileStream fs = File.Create(fm.storagePath + "//" + filename))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(log);
                // Add log information to the file.
                fs.Write(info, 0, info.Length);
            }
        }

        //pulling required files from repo storage 
        public bool receiveFiles(List<string> files)
        {
            Console.WriteLine("\n   Requirement 4: Builder receives test code files and test driver from repository.\n   (Builder already has test request received from repository as a part of message body)");
            foreach (string file in files)
            {
                Console.Write("\n   Receiving \"{0}\" to \"{1}\"", file, Path.GetFullPath(fm.buildPath));
                bool res = fm.sendFile(Path.GetFullPath(fm.storagePath + '/' + file), "builder");
                if (!res)
                {
                    return false;
                }
            }
            return true;
        }

        //Builds files sent by repository
        public void Build(string testDriver, string testConfiguration, List<string> testCodes = null)
        {
            try
            {   //Creating background window of command prompt and firing build command
                Console.WriteLine("\n  Start Logging");
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe"; p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (testCodes != null)
                {
                    string codes = "";
                    foreach (string test in testCodes)
                    {
                        codes = codes + " " + test;
                    }
                    if (testConfiguration == "C#")
                        p.StartInfo.Arguments = "/Ccsc /warnaserror /target:library " + testDriver + " " + codes;
                    if (testConfiguration == "Java")
                        p.StartInfo.Arguments = "/Cjavac " + testDriver;
                }
                else
                {
                    if (testConfiguration == "C#")
                        p.StartInfo.Arguments = "/Ccsc /warnaserror /target:library " + testDriver;
                    if (testConfiguration == "Java")
                        p.StartInfo.Arguments = "/Cjavac " + testDriver;
                }
                Console.Write("\n  Build Command: {0}", p.StartInfo.Arguments);
                p.StartInfo.WorkingDirectory = fm.buildPath;
                p.StartInfo.RedirectStandardError = true; p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                if(testConfiguration == "Java")
                    processClassFile(testDriver);
                p.WaitForExit();
                string time = p.TotalProcessorTime.ToString();string errors = p.StandardError.ReadToEnd();
                string output = p.StandardOutput.ReadToEnd(); Console.Write("\n\n");Console.Write("\n  Output:\n{0}", output + errors);
                if (errors == "" && !output.Contains("error"))
                {
                    Console.Write("\n  Build Successful.");
                    buildResult = true;
                }
                else
                    Console.Write("\n  Build Failure");
                Console.Write("\n  Execution Time: {0}", time);
                bl.startLogging(errors, output, time, testDriver);
            }
            catch (Exception ex)
            {
                Console.Write("\n\n  {0}", ex.Message);
            }
        }
        //Creates test request for test harness
        public string CreateTestRequest()
        {
            string trXml = tr.ToXml();
            return trXml;
        }
        //commands test harness by sending message created by this method
        public Message CreateTestMessage()
        {
            string tr = CreateTestRequest();
            Message rqstMsg = new Message();
            rqstMsg.author = "Fawcett";
            rqstMsg.to = "TestHarness";
            rqstMsg.from = "CoreBuilder";
            rqstMsg.type = "TestRequest";
            rqstMsg.time = DateTime.Now;
            rqstMsg.body = tr;
            rqstMsg.show();
            return rqstMsg;
        }

        private void processClassFile(string testDriver)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.Arguments = "/Cjar -cvf HelloWorld.jar " + Path.GetFileNameWithoutExtension(testDriver) + ".class";
            Console.Write("\n  Build Command: {0}", p.StartInfo.Arguments);
            p.StartInfo.WorkingDirectory = fm.buildPath;
            p.Start();
        }
    }

    class BuilderTest
    {
#if (TEST_BUILDER)
        static void Main(string[] args)
        {
            ReqHandler rh = new ReqHandler();
            Builder br = new Builder(rh);
            Message msg = br.CreateTestMessage();
                br.send(msg);
            ProcessMessage(msg);
        }
#endif
    }
}
