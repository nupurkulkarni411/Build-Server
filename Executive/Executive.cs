/////////////////////////////////////////////////////////////////////
// TestExcutive.cs -   This package excersises all other packages  //
//                               to demonstaret the requirement    //
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
 This is the package that controls the program flow, and contains the main method. 
 The series of tests that must be performed to demonstrate the build server functionalities will be run by this package. 
 It also acts as an entry point into the program.
 We could think of it as a constructor for the Build Server. It would kick off all the other activities. 
 
 * Build Process:
 * --------------
 * Required Files:  IFederation.cs, Builder.cs, Messages.cs, Client.cs, Repository.cs,
                    TestHarness.cs, ReqHandler.cs
   Build Command   - devenv Project2.sln /rebuild debug

 * Maintenance History:
    - Ver 1.0 Oct 2017 
 * --------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MockClient;
using Messages;
using MockRepository;
using LoadingTests;
using CoreBuilder;

using RequestHandler;
using RequestHandlerInterface;
using FederationInterface;
namespace Executive
{
    class Executive
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("\n\n-------------------------------------------------------------------------------");
                Console.WriteLine("\n   Requirement 1  is fulfilled and can be verified by checking source code");
                Console.WriteLine("\n-------------------------------------------------------------------------------");
                Console.WriteLine("\n   Requirement 2  \n   I have total 14 packages in my project which includes packages for\n   Executive, Mock Client, Mock Repository, Mock TestHarness and Core Builder.");
                ReqHandler rh = new ReqHandler();
                Client dc = new Client(rh);
                Builder br = new Builder(rh);
                Repository dr = new Repository(rh);
                LoadingTests.TestHarness th = new LoadingTests.TestHarness(rh);

                rh.testBuild = br;
                rh.testRepo = dr;
                rh.testTH = th;
                rh.testClient = dc;
                Console.WriteLine("\n-------------------------------------------------------------------------------");
                Console.WriteLine("\n   Requirement 3  \n   Showing Builder Operations by fixed sequence of operations.\n");
                Console.WriteLine("\n   1. Client creates test request and command repository by sending message to process testrequest.");
                //Client creates build message
                Message msg = dc.CreateBuildMessage("c#");
                dc.send(msg);
                Console.WriteLine("\n  6. Builder sends message to TestHarness to proceed with testing.\n");
                //Core Builder sends test message to test harness
                msg = br.CreateTestMessage();
                br.send(msg);
                Console.WriteLine("\n----------------------------------------------------------------------------------------------");
                Console.WriteLine("\nJava Implementation");
                msg = dc.CreateBuildMessage("java");
                dc.send(msg);
                Console.WriteLine("\n Sending jar file to test harness");
                th.sendJar();
                Console.WriteLine("\n----------------------------------------------------------------------------------------------");
                Console.WriteLine("\n  Client sending request for viewing logs specifying name of author and type of log(Build/Test)\n");
                //View log
                msg = dc.CreateViewLogMessage();
                dc.send(msg);
                msg = dc.CreateViewLogMessageLogNotFound();
                dc.send(msg);
            }
            catch (Exception ex)
            {
                Console.Write("\n\n  {0}", ex.Message);
            }
        }
    }
}
