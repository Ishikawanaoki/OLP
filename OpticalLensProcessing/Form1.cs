using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpticalLensProcessing.Funcs;

namespace OpticalLensProcessing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            test3();
        }
        void test()
        {
            #region init
            List<FuncList> funcList = new List<FuncList>();
            funcList.Add(new FuncH24List());
            funcList.Add(new FuncH25List(-30, 180));
            funcList.Add(new FuncH27List());

            List<double[][]> ans = InitDoubleArray(funcList);
            #endregion
            
            culcTo3D(funcList, ref ans);

            SaveCSL(funcList, ans);

            CallStatus(funcList);
        }
        void test2()
        {
            #region init
            List<FuncList> funcList = new List<FuncList>();
            funcList.Add(new FuncH24List());
            funcList.Add(new FuncH25List(-30, 180));
            funcList.Add(new FuncH27List());
            #endregion

            //List<double[][]> ans = new List<double[][]>();
            List<Tuple<FuncLList, double[][]>> tuple
                = new List<Tuple<FuncLList, double[][]>>();

            foreach (FuncLList item in funcList)
            {
                //tuple.Add(Tuple.Create(item,
                //culcTo3Args(item)
                //));
            }
        }
        void test3()
        {
            ExtendedFunc.FuncH27List fl = new ExtendedFunc.FuncH27List(1, 2);
            double[][][] ans = fl.CallCulc().ToArray();
            int num = fl.Count;
            foreach (int i in Enumerable.Range(0, num))
            {
                string filename = fl[i].ToString().Split('.').Last() + ".csv";
                fl.Write(filename, ans[i]);
                Console.WriteLine("{0} in saved", filename);
            }
        }

        private void CallStatus(List<FuncList> litem)
        {
            foreach (FuncLList item in litem)
                Console.WriteLine(item.GetStatus());
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void SaveCSL(List<FuncList> list, List<double[][]> ans)
        {int count = 0;
            foreach(FuncList litem in list)
            {
                try
                {
                    string fileName = "item" + count + ".csv";
                    Console.WriteLine("{0} saving at {1}",
                        fileName, DateTime.Now);

                    litem.Write(fileName, ans[count]);
                    Console.WriteLine("{0} : {1}", litem.getColumNum(), litem.getRawNum());
                    Console.WriteLine(DateTime.Now);
                    Console.WriteLine(litem.ToString());
                } finally
                {
                    count++;
                }
            }
        }
        private double[][] InitDoubleArray(FuncList func)
        {
            List<double[]> blankArray = new List<double[]>();

            int colNum = func.getColumNum();
            int rawNum = func.getRawNum();

            foreach (int step in Enumerable.Range(0, rawNum))
                blankArray.Add(new double[colNum]);

            return blankArray.ToArray();
        }
        private List<double[][]> InitDoubleArray(List<FuncList>  litem)
        {
            List<double[][]> blankArray = new List<double[][]>();
            int count = 0;

            foreach (Funcs.FuncList item in litem)
            {
                int colNum = item.getColumNum();
                int rawNum = item.getRawNum();

                blankArray.Add(new double[rawNum][]);
                foreach (int step in Enumerable.Range(0, rawNum))
                    blankArray[count][step] = new double[colNum];

                count++;
            }
            
            return blankArray;
        }
        private void culcTo3D(List<FuncList> litem, ref List<double[][]> ans)
        {
            int count = 0;

            foreach (FuncList item in litem)
            {
                int rawNum = item.getRawNum();
                int colNum = item.getColumNum();

                foreach (int step_R in Enumerable.Range(0, rawNum))
                {
                    foreach (int step_0 in Enumerable.Range(0, colNum))
                    {
                        ans[count][step_R][step_0]
                            = item.CallCulc(step_R, step_0);
                    }
                }

                count++;
            }
        }
    }
}
