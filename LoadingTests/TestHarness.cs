/////////////////////////////////////////////////////////////////////
// TestHarness.cs - Runs tests by loading dlls and invoking test() //
// ver 2                                                           //
// Language:    C#, Visual Studio 2017                             //
// Platform:    Lenovo ideapad 500, Windows 10                     //
// Application: Build Server                                       //
//                                                                 //
// Name : Nupur Kulkarni                                           //
// CSE681: Software Modeling and Analysis, Fall 2017               //
// Author Dr. Jim Fawcett                                          //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * -------------------
 * This package will load the test drivers and execute tests.
 *
 *  Public Interface:
 * ================= 
 * void sendLog(string log, string authorName): For sending test log to repository.
 * bool receiveFiles(List<string> files) : For pulling required files from BuildStorage
 * void ProcessMessage(Message msg) : This enables processing of messages sent by other 
 *                                    packages and takes appropriate actions.
 * bool LoadTests(string path): For loading test from test driver
 * void run(): run all tests
 * 
 * Build Process:
 * --------------
 * Required Files:  TestHarness.cs BuildMessages.cs IFederation.cs FileManager.cs Logger.cs IRequest.cs Serialization.cs
 * Compiler command: csc TestHarness.cs BuildMessages.cs IFederation.cs FileManager.cs Logger.cs IRequest.cs Serialization.cs
 *
 * Maintenance History:
 * Ver 2 
 * - added more error handling in run()
 * - made load error handling more specific, so it will continue
 *   to load tests after a load or creation error
 *   
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using FederationInterface;
using RequestHandlerInterface;
using FileManager;
using Messages;
using BuildServerMessages;
using Utilities;
using System.IO;
using Logger;
#pragma warning disable

namespace LoadingTests
{
    public class TestHarness:IFederation
    {
        private struct TestData
        {
            private string Name;
            private ITest testDriver;
        }

        private List<Type> testTypes = new List<Type>();
        private List<TestData> testDriver = new List<TestData>();
        protected FileManage fm = new FileManage();
        protected TestLog tl = new TestLog();
        private string log = "";
        public List<string> dllFiles { get; set; } = new List<string>();
        public TestHarness(IRequest req):base(req)
        {
            Console.Write("\n   Constructor of mock TestHarness is called.");
        }

        //For sending logs to repository
        public void sendLog(string log, string authorName)
        {
            string filename = "TestLog" + authorName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            Console.WriteLine("\n   9. Sending test log to repository");
            Console.WriteLine("\n\n  Sending test log: {0} to {1}", filename, Path.GetFullPath(fm.storagePath));
            using (FileStream fs = File.Create(fm.storagePath + "//" + filename))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(log);
                // Add log information to the file.
                fs.Write(info, 0, info.Length);
            }
        }

        //processing messages sent by other packages to test harness
        public void ProcessRequest(Message msg)
        {
            TestRequest tr = msg.body.FromXml<TestRequest>();
            dllFiles.Clear();
            tl.createLog(tr.author);
            foreach (TestElement te in tr.tests)
            {
                dllFiles.Add(te.testDriver); 
            }
            Console.WriteLine("\n  7. Test Harness parses test request and pulls required files from build storage.(Requirement 7)");
            receiveFiles(dllFiles);
            Directory.Delete(Path.GetFullPath(fm.buildPath), true);
            if (LoadTests(fm.testPath))
                run();
            else
                Console.Write("\n  couldn't load tests");
            sendLog(tl.getLog(), tr.author);
        }

        //to pull required drivers from build storage 
        public bool receiveFiles(List<string> files)
        {
            foreach (string file in files)
            {
                Console.Write("\n   Receiving \"{0}\" to \"{1}\"", Path.GetFileName(file), Path.GetFullPath(fm.testPath));
                bool res = fm.sendFile(Path.GetFullPath(fm.buildPath + '/' + Path.GetFileName(file)), "tester");
                if (!res)
                {
                    return false;
                }
            }
            return true;
        }
        //----< load test dlls to invoke >-------------------------------
        public bool LoadTests(string path)
        {
            string[] files = System.IO.Directory.GetFiles(fm.testPath, "*.dll");
            Console.WriteLine("\n  8. Test harness loads all the dlls for testing and runs the tests.(Requirement 8)");
            foreach (string file in files)
            {
                Console.Write("\n  loading: \"{0}\"", file);
                try
                {
                    Assembly assem = Assembly.LoadFrom(file);
                    Type[] types = assem.GetExportedTypes();

                    foreach (Type t in types)
                    {
                        MethodInfo tM = t.GetMethod("test");

                        if (t.IsClass && t.GetInterface("ITest") != null && tM != null && tM.ReturnType == typeof(bool))//typeof(ITest).IsAssignableFrom(t))  // does this type derive from ITest ?
                        {
                            testTypes.Add(t);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // here, in the real testharness you log the error
                    Console.Write("\n  {0}", ex.Message);
                }

            }   
            Console.Write("\n");
            return testTypes.Count > 0;   // if we have items in list then Load succeeded
        }

        //----< run all the tests on list made in LoadTests >------------

        public void run()
        {
            Console.WriteLine("\n  Starting logger");
            foreach (Type test in testTypes)
            {
                MethodInfo testMethod = test.GetMethod("test");
                Console.Write("\n  testing {0}", test.Name);
                bool result = (bool)testMethod.Invoke(Activator.CreateInstance(test), null);
                if (result)
                {
                    Console.Write("\n  test passed");
                    log += ("Test Passed");
                }
                else
                {
                    Console.Write("\n  test failed");
                    log += ("Test Failed");
                }
                tl.startLogging(test.Name, result,log);
            }
        }
        //for sending jar file to test harness directoly and not loading it. 
        public void sendJar()
        {
            string[] tempFiles = Directory.GetFiles(fm.buildPath, "*.jar");
            foreach(string file in tempFiles)
            {
                Console.Write("\n   Receiving \"{0}\" to \"{1}\"", Path.GetFileName(file), Path.GetFullPath(fm.testPath));
                fm.sendFile(Path.GetFullPath(fm.buildPath + '/' + Path.GetFileName(file)), "tester");
            }
            Directory.Delete(Path.GetFullPath(fm.buildPath), true);
            Console.WriteLine("\n  Loader is not present for jar files to run.");
            Console.WriteLine("\n  Jar file can be found at: \n    {0}", Path.GetFullPath(fm.testPath));
            string command = "java -cp HelloWorld.jar HelloWorld";
            Console.WriteLine("\n  For running the jar file following command can be run: \n   {0}",command);
        }
#if (TEST_TEST_HARNESS)
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstrate loading and executing tests");
            Console.Write("\n =========================================");

            // using string path = "../../../Tests/TestDriver.dll" from command line;

            if (args.Count() == 0)
            {
                Console.Write("\n  Please enter path to libraries on command line\n\n");
                return;
            }
            string path = args[0];

            TestHarness th = new TestHarness();
            if (th.LoadTests(path))
                th.run();
            else
                Console.Write("\n  couldn't load tests");

            Console.Write("\n\n");
        }
#endif
    }
}
