/////////////////////////////////////////////////////////////////////
// ReqHandler.cs : Implemented mediator pattern for handling       //
//                  communication between all the packages.        //
// ver 1.0                                                         //
// Language:    C#, Visual Studio 2015                             //
// Platform:    Dell XPS 8900, Windows 10                          //
// Application: Test Harness, Software Modelling and Analysis      //
//                                                                 //
// Sneha Patil, CSE681 - Software Modeling and Analysis, Fall 2016 //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * -------------------
 * This is ConcreteMediator in the pattern.
 * ConcreteMediator - implements the Mediator interface and coordinates 
 * communication between Colleague objects. It is aware of all of the 
 * Colleagues and their purposes with regards to inter-communication. 
 *
 * Public Interface:
 * ================= 
 * void send(Message msg): Guiding communication between components
 * 
 * Build Process:
 * --------------
 * Required Files:  IFederation.cs
   Build Command: csc IFederation.cs
 *
 * Maintenance History:
    - Ver 1.0 Oct 2017 
 * --------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RequestHandlerInterface;
using MockRepository;
using CoreBuilder;
using LoadingTests;
using Messages;
using FederationInterface;
using MockClient;

namespace RequestHandler
{
    public class ReqHandler:IRequest
    {
        //declares communication components
        private Repository repo;
        private Builder br;
        private TestHarness th;
        private Client cl;

        //as per mediator pattern instanciated all the communicating components
        public Repository testRepo
        {
            set { repo = value; }
        }

        public Builder testBuild
        {
            set { br = value; }
        }

        public Client testClient
        {
            set { cl = value; }
        }
        public TestHarness testTH
        {
            set { th = value; }
        }
        //this method is present in all the communicating components and this guilds
        //the communication
        public override void send(Message msg)
        {
            if (msg.to == "Repository")
            {
                repo.ProcessMessage(msg);
            }
            else if(msg.to == "Builder")
            {
                br.ProcessRequest(msg);
            }
            else if (msg.to == "TestHarness")
            {
                th.ProcessRequest(msg);
            }
            else
            {
                Console.Write("Invalid Message");
            }
        }
#if (TEST_REQUESTHANDLER)
        static void Main(string[] args)
        {
            ReqHandler rh = new ReqHandler();
            Client dc = new Client(rh);
            Builder br = new Builder(rh);
            Repository dr = new Repository(rh);
            LoadingTests.TestHarness th = new LoadingTests.TestHarness(rh);

            rh.testBuild = br;
            rh.testRepo = dr;
            rh.testTH = th;
            rh.testClient = dc;
            Message msg = dc.CreateBuildMessage();
            dc.send(msg);
        }
#endif
    }
}
