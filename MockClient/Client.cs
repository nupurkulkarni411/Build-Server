/////////////////////////////////////////////////////////////////////
// Clent.cs -   This package provides functionality for creating   //
//              messages to test various functionalities of system //
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
 The client Demo class which is used to kick off required functionalities of system.

 *  Public Interface:
 * ================= 
 * void send(Message msg) : This unables client to communicate with other packages
 *                          by sending message to Common request handler. 
 * public string CreateBuildRequest() : Used to create Build request XML.
 * public Message CreateBuildMessage() : Used to command repository to process build request by sending message
 * public Message CreateViewLogMessage(): Used to create request for viewing logs
 *
 * Build Process:
 * --------------
 * Required Files: Client.cs BuilderMessages.cs IFederation.cs Messages.cs Serialization.cs IRequest.cs
 * Build Command: csc Client.cs BuilderMessages.cs IFederation.cs Messages.cs Serialization.cs IRequest.cs
 *
 * Maintenance History:
    - Ver 1.0 Oct 2017
 * --------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BuildServerMessages;
using Utilities;
using Messages;
using FederationInterface;
using RequestHandlerInterface;

namespace MockClient
{
    public class Client:IFederation
    {
        public Client(IRequest req):base(req)
        {
            Console.Write("\n   Constructor of mock client is called.");
        }

        //This unables client to communicate with other packages
        public void send(Message msg)
        {
            req.send(msg);
        }

        //Build Request for C#
        public string CreateBuildRequest()
        {
            TestElement te1 = new TestElement();
            te1.testName = "test1";
            te1.addDriver("TestDriver2.cs");
            te1.addTestConfiguration("C#");
            te1.addCode("ITest.cs");
            te1.addCode("CodeToTest2.cs");

            TestElement te2 = new TestElement();
            te2.testName = "test2";
            te2.addDriver("TestDriver1.cs");
            te2.addTestConfiguration("C#");
            te2.addCode("ITest.cs");
            te2.addCode("CodeToTest1.cs");

            TestRequest tr = new TestRequest();
            tr.author = "Jim Fawcett";
            tr.timeStamp = DateTime.Now;
            tr.tests.Add(te1);
            tr.tests.Add(te2);
            string trXml = tr.ToXml();
            Console.Write("\n   Test Request: \n{0}\n",trXml);
            return trXml;
        }

        //Build Request for java
        public string CreateJavaBuildRequest()
        {
            TestElement te1 = new TestElement();
            te1.testName = "test1";
            te1.addDriver("HelloWorld.java");
            te1.addTestConfiguration("Java");

            TestRequest tr = new TestRequest();
            tr.author = "Jim Fawcett";
            tr.timeStamp = DateTime.Now;
            tr.tests.Add(te1);
            string trXml = tr.ToXml();
            Console.Write("\n   Test Request: \n{0}\n", trXml);
            return trXml;
        }

        //Command to process test request.
        public Message CreateBuildMessage(string language)
        {
            string tr ="";
            if (language == "c#")
                tr = CreateBuildRequest();
            if (language == "java")
                tr = CreateJavaBuildRequest();
            Message rqstMsg = new Message();
            rqstMsg.author = "Fawcett";
            rqstMsg.to = "Repository";
            rqstMsg.from = "Client";
            rqstMsg.type = "TestRequest";
            rqstMsg.time = DateTime.Now;
            rqstMsg.body = tr;
            rqstMsg.show();
            return rqstMsg;
        }

        //Request for viewing logs 
        public Message CreateViewLogMessage()
        {
            Message req = new Message();
            req.author = "Fawcett";
            req.to = "Repository";
            req.from = "Client";
            req.type = "ViewLog";
            req.time = DateTime.Now;
            req.body = "BuildLogJim Fawcett";
            req.show();
            return req;
        }

        //Request for different user to view logs
        public Message CreateViewLogMessageLogNotFound()
        {
            Message req = new Message();
            req.author = "Fawcett";
            req.to = "Repository";
            req.from = "Client";
            req.type = "ViewLog";
            req.time = DateTime.Now;
            req.body = "TestLogNupur Kulkarni";
            req.show();
            return req;
        }
    }


    class MockClientTest
    {
#if (TEST_MockCLIENT)
        static void Main(string[] args)
        {
            Console.Write("\n   demo of client");
            Console.Write("\n   ==============================");
            HelpRepoMock repo = new HelpRepoMock();
            repo.getFiles("*.*");
            Client dc = new Client(rh);
            Message msg = dc.CreateBuildMessage();
                dc.send(msg);
     
            Console.Write("\n\n");
        }
#endif
    }
}
