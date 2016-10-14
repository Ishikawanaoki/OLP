using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticalLensProcessing
{
    namespace ExtendedFunc
    {
        class ThreeAnotherFuncList : Funcs.FuncLList
        {
            public ThreeAnotherFuncList()
            {
                Add(new FuncRto0(new int[2] { 0, 0 }, new int[2] { 90, 180 }));
                Add(new FuncAto0(new int[2] { 0, 0 }, new int[2] { 180, 90 }));
                Add(new FuncAtoR(new int[2] { 0, 0 }, new int[2] { 90, 90 }));
            }
            private List<Tuple<IEnumerable<int>, IEnumerable<int>>> GetSpecialRange()
            {
                List<Tuple<IEnumerable<int>, IEnumerable<int>>> list
                    = new List<Tuple<IEnumerable<int>, IEnumerable<int>>>();
                foreach (FuncRto0 item in this)
                {
                    Tuple<IEnumerable<int>, IEnumerable<int>> tmp
                        = Tuple.Create(
                            Enumerable.Range(item.start[0], item.count[0]),
                            Enumerable.Range(item.start[1], item.count[1])
                            );

                    list.Add(tmp);
                }
                return list;
            }
            public List<double[][]> Culc()
            {
                if (GetSpecialRange().Count != Count) return new List<double[][]>();
                int count = 0;
                List<double[][]> ans = new List<double[][]>();
                foreach (Tuple<IEnumerable<int>, IEnumerable<int>> item in GetSpecialRange())
                {
                    if (count > 2) break;
                    List<double[]> tmp = new List<double[]>();
                    foreach (int arg1 in item.Item1)
                    {
                        List<double> tmp2 = new List<double>();
                        foreach (int arg2 in item.Item2)
                        {
                            tmp2.Add(this[count].culc(arg1, arg2));
                        }
                        tmp.Add(tmp2.ToArray());
                    }
                    ans.Add(tmp.ToArray());
                    count++;
                }

                return ans;
            }
            /// <summary>
            /// 任意のクラスオブジェクトの名前に指定される順に
            /// 引数を指定することで
            /// 所定の掲載を実行する
            /// </summary>
            /// <param name="_first"></param>
            /// <param name="_second"></param>
            /// <returns></returns>
            public override double CallCulc(int step_R, int step_0)
            {
                return base.CallCulc(step_R, step_0);
            }
            public List<double[][]> GetList(int _first, int _second)
            {
                return new List<double[][]>();
            }
        }
        class FuncSelectedArg : Funcs.BaseFunc
        {
            public FuncSelectedArg(int[] start, int[] count)
            {
                this.start = start;
                this.count = count;
            }
        }
        class FuncRto0 : FuncSelectedArg
        {
            public FuncRto0(int[] start, int[] count) : base(start, count) { }
            public override double get2ndArg(double theta_R)
            {
                if(0 <= theta_R && theta_R < Math.PI / 2)
                {
                    return base.get2ndArg(theta_R);
                }
                else if(90 <= theta_R && theta_R <= Math.PI)
                {
                    return base.get2ndArg(Math.PI - theta_R) * (-1);
                }
                else
                {
                    return -1.111;
                }
            }
        }
        class FuncAto0 : FuncSelectedArg
        {
            public FuncAto0(int[] start, int[] count) : base(start, count) { }
            // Aと0からRを求める
            public override double culc(int step_A, int step_0)
            {
                return base.culc(getR(step_A, step_0), step_0);
            }
        }
        class FuncAtoR : FuncSelectedArg
        {
            public FuncAtoR(int[] start, int[] count) : base(start, count) { }
            // Rをculcの二つ目に移動
            // AとRから0を求める
            public override double culc(int step_A, int step_R)
            {
                return base.culc(step_R, get0(step_A, step_R));
            }
            public override double get2ndArg(double theta_R)
            {
                if (0 <= theta_R && theta_R < Math.PI / 2)
                {
                    return base.get2ndArg(theta_R);
                }
                else if (90 <= theta_R && theta_R <= Math.PI)
                {
                    return base.get2ndArg(Math.PI - theta_R) * (-1);
                }
                else
                {
                    return -1.111;
                }
            }
        }

        class FuncH27List : Funcs.FuncLList
        {
            public FuncH27List(int aLength, int bLength)
            {
                Add(new FuncRto0(new int[2] { 0, 0 }, new int[2] { 90, 180 }));
                Add(new FuncAto0(new int[2] { 0, 0 }, new int[2] { 180, 90 }));
                Add(new FuncAtoR(new int[2] { 0, 0 }, new int[2] { 90, 90 }));
            }
            public List<double[][]> CallCulc()
            {
                List<double[][]> ans = new List<double[][]>();
                foreach (int item in Enumerable.Range(0, Count))
                {
                    List<double[]> ans2 = new List<double[]>();
                    foreach (int fiArg in Enumerable.Range(this[item].start[0], this[item].count[0]))
                    {
                        List<double> ans3 = new List<double>();
                        foreach (int secArg in Enumerable.Range(this[item].start[1], this[item].count[1]))
                        {
                            ans3.Add(this[item].culc(fiArg, secArg));
                        }
                        ans2.Add(ans3.ToArray());
                    }
                    ans.Add(ans2.ToArray());
                }
                return ans;
            }
        }
//        class FuncRto0ver2 : FuncRto0 { }

//        class FuncAtoRver2 : FuncAtoR { }
    }
}
