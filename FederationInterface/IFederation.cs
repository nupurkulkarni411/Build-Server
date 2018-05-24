/////////////////////////////////////////////////////////////////////
// IFederation.cs : Implemented mediator pattern for handling      //
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
 * Colleague element - defines the interface for communication with other Colleagues 
 *
 * Public Interface:
 * ================= 
 * IFederation: For directing request to mediator   //Implemented by clint, repository, test harness, build server

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

namespace FederationInterface
{
    public abstract class IFederation
    {
        protected IRequest req;
        //defines an interface for the participants taking part in communication.
        public IFederation(IRequest testReq)
        {
            req = testReq;
        }   
    }
}
