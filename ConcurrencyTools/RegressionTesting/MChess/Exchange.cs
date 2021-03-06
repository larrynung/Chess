﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class Exchange : MChessRegressionTestBase
    {
        class Node
        {
            public Node next;
            public readonly int id;
            public Node(int id)
            {
                this.id = id;
            }
        }

        class LinkedList
        {
            public readonly Node head;
            private Node insertionPoint;
            public LinkedList()
            {
                head = insertionPoint = new Node(0);
            }
            public void Insert(int id)
            {
                Node newNode = new Node(id);
                //notice that the value exchanged for insertionPoint is *always new*
                //therefore, after this next line finishes, oldInsertionPoint != insertionPoint
                Node oldInsertionPoint = Interlocked.Exchange(ref insertionPoint, newNode);
                //we may be preempted here; but the new nodes will just go after us
                //so they'll be implicitely added when we finish
                oldInsertionPoint.next = newNode;
            }
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 3, LastThreadCount = 2, LastExecSteps = 10, LastHBExecSteps = 3)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 5, LastThreadCount = 2, LastExecSteps = 10, LastHBExecSteps = 3)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 5, LastThreadCount = 2, LastExecSteps = 10, LastHBExecSteps = 3)]
        public void Test()
        {
            //prep
            var list = new LinkedList();
            var t = new Thread(() => { list.Insert(-1); });

            //run
            t.Start();
            list.Insert(1);
            list.Insert(2);

            //finish
            t.Join();
            int count = 0;
            for (Node n = list.head; n.next != null; n = n.next) count += 1;
            Assert.IsTrue(count == 3);
        }
    }
}