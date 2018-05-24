/////////////////////////////////////////////////////////////////////
// IRequest.cs : Implemented mediator pattern for handling         //
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
 * Mediator - defines the interface for communication between Colleague objects 
 *
 * Public Interface:
 * ================= 
 * IRequest: For directing request to mediator   //Implemented by clint, repository, test harness, build server

 * Build Process:
 * --------------
 * Required Files:  IRequest.cs
   Build Command: csc IRequest.cs
 *
 * Maintenance History:
    - Ver 1.0 Oct 2017 
 * --------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Messages;

namespace RequestHandlerInterface
{
    //IRequest deligates send functionality
    abstract public class IRequest
    {
        public abstract void send(Message msg);
    }
}
