/////////////////////////////////////////////////////////////////////
// BasicRepository.cs - define basic functionalities of repository //
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
 * This package processes messages sent by client to view logs or process test request.
 *
 *  Public Interface:
 * ================= 
 * void send(Message msg) : This unables repository to communicate with other packages
 *                          by sending message to Common request handler. 
 * void ProcessMessage(Message msg) : This enables processing of messages sent by other 
 *                                    packages and takes appropriate actions.
 * Build Process:
 * --------------
 * Required Files:  Repository.cs, Builder.cs, FileManager.cs, Messages.cs, IFederation.cs
 * Compiler command: csc Repository.cs  Builder.cs  FileManager.cs  Messages.cs IFederation.cs
 *
 * Maintenance History:
    - Ver 1.0 Oct 2017 
 * --------------------*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileManager;
using Messages;
using CoreBuilder;

using RequestHandlerInterface;
using FederationInterface;
using System.IO;

namespace MockRepository
{
    public class Repository:IFederation
    {
        protected FileManage fm = new FileManage();
        public Repository(IRequest req):base(req)
        {
            Console.Write("\n   Constructor of mock repository is called.");
        }

        //This unables repository to communicate with other packages
        public void send(Message msg)
        {
            req.send(msg);
        }

        //Process messages sent by other packages
        public void ProcessMessage(Message msg)
        {
            //Parse message sent to repository
            Message ParsedMessage = msg.fromString(msg.ToString());
            switch (ParsedMessage.type)
            {
                case "TestRequest":
                    Console.Write("\n  2. Repository forwards test request to build server and commands build server \n     by sending message to process testrequest. ");
                    ParsedMessage.to = "Builder";
                    ParsedMessage.from = "Repository";
                    ParsedMessage.show();
                    send(ParsedMessage);
                    break;
                case "ViewLog":
                    //fetches logs from repository storage 
                    string[] tempFiles = Directory.GetFiles(fm.storagePath);
                    bool exist = false;
                    foreach(string file in tempFiles)
                    {
                        if (file.Contains(ParsedMessage.body))
                        {
                            fm.sendFile(file, "client");
                            exist = true;
                        }
                    }
                    if (!exist)
                        Console.WriteLine("\n  No log found.");
                    else
                        Console.WriteLine("\n Logs are fetched from repository and stored at following location: \n   {0}", Path.GetFullPath(fm.clientPath));
                    break;
                default:
                    Console.Write("Message is of invalid type");
                    break;
            }
        }
    }

    //test stub
    class MockRepoTest
    {
#if (TEST_HELPREPOMOCK)
        static void Main(string[] args)
        {
            Console.Write("\n   demo of mock repo");
            Console.Write("\n   ==============================");
            HelpRepoMock repo = new HelpRepoMock();
            repo.getFiles("*.*");
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
