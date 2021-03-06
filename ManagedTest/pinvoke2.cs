/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Threading.ConcurrencyScheduler;

namespace Test23
{
    public class ChessTest
    {
        static Object lock_me;
        
        public static void Main(string[] s)
        {
            if (Startup(s))
            {
                Run();
            }
        }

        public static bool Startup(string[] s)
        {
            lock_me = new Object();
            return true;
        }

        public delegate int Win32ThreadStartDelegate(IntPtr f);
        private static Win32ThreadStartDelegate m_threadStart;

        public static IntPtr CreateNativeThread(Win32ThreadStartDelegate p){
            IntPtr n_nativeFunction = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(p);
            //ch = p;
            IntPtr handle = NativeMethods.CreateThread(
                IntPtr.Zero, IntPtr.Zero, n_nativeFunction,
                IntPtr.Zero, 0, IntPtr.Zero);
            return handle;
        }

        public static bool Run()
        {
            int x = 0;
            m_threadStart = Child;
            IntPtr[] h = new IntPtr[2];
            h[0] = CreateNativeThread(m_threadStart);
            h[1] = CreateNativeThread(m_threadStart);
            lock (lock_me)
            {
                System.Console.WriteLine("Parent");
                x++;
            }
            unsafe
            {
                fixed (IntPtr* handlePtr = h)
                {
                    NativeMethods.WaitForMultipleObjects(2, handlePtr, true, Timeout.Infinite);
                }
            }
            NativeMethods.CloseHandle(h[0]);
            NativeMethods.CloseHandle(h[1]);
            return true;
        }

        public static int Child(IntPtr f)
        {
            int x = 0;
            lock (lock_me)
            {
                System.Console.WriteLine("Child");
                x++;
            }
            return 0;
        }
    }
}