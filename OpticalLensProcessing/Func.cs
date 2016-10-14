using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace OpticalLensProcessing
{
    namespace Funcs
    {
        interface IFunc
        {
            double culc(int step_R, int step_0);

            int getR(int step_A, int step_0);
            int get0(int step_A, int step_R);

            double get1stArg(double step_R, double step_0);
            double get2ndArg(double step_R);
            double getX(double theta_R);

            double get_b(double theta_A);

            string GetStatus();
        }
        public static class BaseF
        {
            public static double getX(double aLength, double bLength, double theta_R)
            {
                return aLength * bLength /
                    Sqrt(
                        Pow(aLength * aLength * Sin(theta_R), 2)
                        + Pow(bLength * bLength * Cos(theta_R), 2)
                    );
            }

            internal static double get_b(double theta_A)
            {
                return Asin(Sin(theta_A) * 2 / 3);
            }
        }
        public abstract class BaseFunc : IFunc
        {
            #region Field_and_constructor
            private const int initstep = 180;
            public int step            = initstep;
            private double thetaToStep = PI / initstep;

            /// <summary>
            /// step の 初期値 を180
            /// ゲットのみ
            /// </summary>
            public int Step
            {
                private set;
                get;
            }
            /// <summary>
            /// セット呼び可能
            /// </summary>
            public double ThetaToStep
            {
                set { step = (int)value; thetaToStep = value > 1 ? PI / value : 0; }
                get { return thetaToStep; }
            }

            private readonly double aLength;
            private readonly double bLength;
            public BaseFunc()
            {
                aLength = 1.0;
                bLength = 1.0;
            }
            public BaseFunc(double a, double b)
            {
                aLength = a;
                bLength = b;
            }
             // for ExtendedFunc
            public int[] start;
            public int[] count;
            #endregion
            public string GetStatus()
            {
                return "a = " + aLength + ", b = " + bLength + "\nstepby : " + step;
            }
            /// <summary>
            /// 楕円の動径
            /// </summary>
            /// <returns>楕円の動径</returns>
            public virtual double getX(double theta_R)
            {
                return BaseF.getX(aLength, bLength, theta_R);
            }
            public int getR(int step_A, int step_0)
            {
                return (step_A + step_0);
            }
            public int get0(int step_A, int step_R)
            {
                return (step_R - step_A);
            }
            public double get_b(double theta_A)
            {
                return BaseF.get_b(theta_A);
            }
            public virtual double culc(int step_R, int step_0)
            {
                double thetaR = step_R * thetaToStep;
                double theta0 = step_0 * thetaToStep;
                return get1stArg(thetaR, theta0) - get2ndArg(thetaR);
            }
            /// <summary>
            /// return L1 on 27H version
            /// </summary>
            public virtual double get1stArg(double theta_R, double theta_0)
            {
                return getX(theta_R)
                    * Sin(theta_R)
                    * Tan(PI / 2 - theta_R + get_b(theta_R - theta_0));
            }
            /// <summary>
            /// return S1 on 27H version
            /// </summary>
            public virtual double get2ndArg(double step_R)
            {
                return getX(step_R)
                    * Cos(step_R);
            }
        }
        interface IFuncList
        {
            int getColumNum();
            int getRawNum();
            double CallCulc(int step_R, int step_0);
            string GetStatus();
        }
        abstract class FuncList : List<BaseFunc>, IFuncList
        {
            #region 保存時のクラスオブジェクトの区別化
            private int _id;
            private static int _id_generator;
            static FuncList()
            {
                _id_generator = 0;
            }
            public FuncList()
            {
                _id = _id_generator;
                _id_generator++;
            }
            #endregion

            // 縦の行
            // protectedにより、継承先だけでアクセス可能
            protected int Rstart = 0;
            protected int Rend = 180;

            // 横の列
            protected int Cstart = 0;
            protected int Cend = 360;
            
            /// <summary>
            /// 最大の列数を計算
            /// </summary>
            /// <returns></returns>
            public int getColumNum()
            {
                if(Cstart * Cend >= 0)
                {
                    return Abs(Cend - Cstart);
                }
                else
                {
                    return Abs(Cstart) + Abs(Cend);
                }
            }
            /// <summary>
            /// 最大の行数を計算
            /// </summary>
            /// <returns></returns>
            public int getRawNum()
            {
                if (Rstart * Rend >= 0)
                {
                    return Abs(Rend - Rstart);
                }
                else
                {
                    return Abs(Rstart) + Abs(Rend);
                }
            }
            public virtual double CallCulc(int step_R, int step_0)
            {
                return 0.0;
            }
            public override string ToString()
            {
                //string fileName = Path.GetFileName(this.GetType().Assembly.Location);
                string className = GetType().FullName;
                //string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                return _id + className;
            }
            public virtual string GetStatus()
            {
                return "Raw:" + Rstart + "-" + Rend
                    + ",Column" + Cstart + "-" + Cend;
            }
            public void Write(string FileName, double[][] data)
            {
                string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(mydocpath + @"\EBooks\" + FileName))
                {
                    int count = Rstart;
                    // init
                    sw.WriteLine(System.DateTime.Now);
                    sw.Write("step,");
                    foreach (int step_culm in Enumerable.Range(0, data[0].Length))
                    {
                        sw.Write("{0},", step_culm);
                        if (step_culm == data[0].Length - 1)
                            sw.WriteLine("");
                    }
                    // init end

                    // foreach文によりジャグ配列へと、柔軟な参照が可能
                    foreach (double[] array in data)
                    {
                        sw.Write("{0},", count);
                        foreach (double item in array)
                        {
                            string val = string.Format("{0:F5}", item);
                            sw.Write("{0},", val);
                        }
                        sw.WriteLine("");
                        count++;
                    }
                }
            }
            public static void SWrite(string FileName, double[][] data)
            {
                string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(mydocpath + @"\EBooks\" + FileName))
                {
                    int count = 0;
                    // init
                    sw.WriteLine(System.DateTime.Now);
                    sw.Write("step,");
                    foreach (int step_culm in Enumerable.Range(0, data[0].Length))
                    {
                        sw.Write("{0},", step_culm);
                        if (step_culm == data[0].Length - 1)
                            sw.WriteLine("");
                    }
                    // init end

                    // foreach文によりジャグ配列へと、柔軟な参照が可能
                    foreach (double[] array in data)
                    {
                        sw.Write("{0},", count);
                        foreach (double item in array)
                        {
                            string val = string.Format("{0:F5}", item);
                            sw.Write("{0},", val);
                        }
                        sw.WriteLine("");
                        count++;
                    }
                }
            }
        }
        class FuncLList : FuncList
        {
            /// <summary>
            /// 内包する任意の子孫の状態を取得、
            /// </summary>
            /// <returns></returns>
            public override string GetStatus()
            {
                string str = base.GetStatus();
                foreach (BaseFunc item in this)
                {
                    str = "\n" + item.GetStatus();
                }
                return str;
            }
            public IEnumerable<int> GetRangeR()
            {
                return Enumerable.Range(Rstart, getRawNum());
            }
            public IEnumerable<int> GetRange0()
            {
                return Enumerable.Range(Cstart, getColumNum());
            }
            public IEnumerable<int> GetRangeA()
            {
                return Enumerable.Range(0, 90);
            }
        }
        class FuncH27List : FuncLList
        {
            
            public FuncH27List()
            {
                int a = 1;
                int b = 2;
                Add(new FuncH27Ver1(a,b));
                Add(new FuncH27Ver2(a,b));
            }
            public override double CallCulc(int step_R, int step_0)
            {
                if(step_R < this[0].Step/2)
                    return this[0].culc(step_R, step_0);
                else
                    return this[1].culc(step_R, step_0);
            }
            class FuncH27Ver1 : BaseFunc
            {
                public FuncH27Ver1(double a, double b) : base(a, b)
                {
                }
            }
            class FuncH27Ver2 : FuncH27Ver1
            {
                public FuncH27Ver2(double a, double b) : base(a, b)
                {
                }
                /// <summary>
                /// return S2 on 27H version
                /// 3rd arg : theta_R => PI - theta_R
                /// </summary>
                public override double get1stArg(double theta_R, double theta_0)
                {
                    return base.get1stArg(theta_R, theta_0);
                }
                /// <summary>
                /// return D2 on 27H version
                /// 引き算を足し算にする。culc()は同一
                /// </summary>
                public override double get2ndArg(double theta_R)
                {
                    // 0 < step_R < step=180
                    return base.get2ndArg(PI - theta_R) * (-1);
                }
            }
        }
        class FuncH25List : FuncLList
        {
            public FuncH25List(int start, int end)
            {
                Rstart = start;
                Rend = end;
                Add(new FuncH25Ver1());
                Add(new FuncH25Ver2());
                Add(new FuncH25Ver3());
            }
            public override double CallCulc(int step_R, int step_0)
            {
                if(step_R < 0)
                {
                    return this[0].culc(step_R, step_0);
                }
                else if (step_R < this[1].step / 2)
                {
                    return this[1].culc(step_R, step_0);
                }
                else
                {
                    return this[2].culc(step_R, step_0);
                }
            }
            class FuncH25Ver1 : BaseFunc
            {
                public override double getX(double theta_R)
                {
                    return Sin(theta_R) + 0.5f;
                }
                public override double get1stArg(double theta_R, double theta_0)
                {
                    return getX(theta_R) * Tan(PI / 2 - theta_R + get_b(theta_R -  theta_0));
                }
                public override double get2ndArg(double theta_R)
                {
                    return Cos(theta_R);
                }
            }
            class FuncH25Ver2 : FuncH25Ver1
            {
                public override double get1stArg(double theta_R, double theta_0)
                {
                    return getX(theta_R)*Tan( - theta_R - PI / 2 + get_b(theta_R - theta_0));
                }
            }
            class FuncH25Ver3 : FuncH25Ver1
            {
                public override double getX(double theta_R)
                {
                    return Sin(PI - theta_R) + 0.5f;
                }
                public override double get2ndArg(double theta_R)
                {
                    return base.get2ndArg(step - theta_R) * (-1);
                }
            }
        }

        class FuncH24List : FuncLList
        {
            public FuncH24List()
            {
                Add(new FuncH24Ver1());
                Add(new FuncH24Ver2());
            }
            public override double CallCulc(int step_R, int step_0)
            {
                if (step_R < this[0].Step / 2)
                {
                    return this[0].culc(step_R, step_0);
                }
                else
                {
                    return this[1].culc(step_R, step_0);
                }
            }
            class FuncH24Ver1 : BaseFunc
            {
                /// <summary>
                /// return L1
                /// a = b = 1 anytime
                /// </summary>
                public override double get1stArg(double theta_R, double theta_0)
                {
                    return Sin(theta_R) * Tan(PI / 2 - theta_R * step + get_b(theta_R -  theta_0));
                }
                /// <summary>
                /// return S1
                /// a = b = 1 anytime
                /// </summary>
                public override double get2ndArg(double theta_R)
                {
                    return Cos(theta_R);
                }
            }
            class FuncH24Ver2 : FuncH24Ver1
            {
                /// <summary>
                /// return D2
                /// a = b = 1 anytime
                /// </summary>
                public override double get1stArg(double theta_R, double theta_0)
                {
                    return base.get1stArg(theta_R, theta_0);
                }
                /// <summary>
                /// return S2
                /// a = b = 1 anytime
                /// </summary>
                public override double get2ndArg(double theta_R)
                {
                    return base.get2ndArg(PI - theta_R) * (-1);
                }
            }
        }
    }    
}
