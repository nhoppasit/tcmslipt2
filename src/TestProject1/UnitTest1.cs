using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            ushort dioCardNbr = 0;
            DIO_Library.D7432.SetupLog();
            short dioCode;
            string dioMessage;
            DIO_Library.D7432.Initial(dioCardNbr, out dioCode, out dioMessage);
            DIO_Library.D7432.Testing = false; //<-------------------------------------------------------------<-TESTING

            bool PreINP1 = false, PreINP2 = false;
            bool INP1, INP2, INP1R, INP1F;
            long cnt = 0;
            while (true)
            {

                // Do read  
                short resCode;
                string respond;
                DIO_Library.D7432.ReadPin(0, 0, out INP1, out resCode, out respond);
                DIO_Library.D7432.ReadPin(0, 1, out INP2, out resCode, out respond);
                if (PreINP1 != INP1)
                {
                    int sum = 0;
                    const int MAX_SUM = 100;
                    const int TOL_SUM_1 = 90;
                    const int TOL_SUM_0 = 80;
                    const int MS_DELAY = 1;
                    for (int i = 0; i < MAX_SUM; i++)
                    {
                        System.Threading.Thread.Sleep(new TimeSpan(100));
                        DIO_Library.D7432.ReadPin(0, 0, out INP1, out resCode, out respond);
                        if (PreINP1 != INP1)
                        {
                            sum++;
                        }
                        else
                        {
                            sum--;
                        }
                    }
                    if ((TOL_SUM_0 <= sum && PreINP1 == true) || 
                        (TOL_SUM_1 <= sum && PreINP1 == false))
                    {
                        cnt++;
                        PreINP1 = INP1;
                        System.Diagnostics.Trace.Write(cnt.ToString());
                        System.Diagnostics.Trace.Write(INP1 ? ", 1" : ", 0");
                        System.Diagnostics.Trace.WriteLine(INP2 ? "1" : "0");
                    }
                }
                
                //System.Diagnostics.Trace.Write(INP1 ? "1" : "0");
                //System.Diagnostics.Trace.WriteLine(INP2 ? "1" : "0");
                System.Threading.Thread.Sleep(10);
            }

        }
    }
}
