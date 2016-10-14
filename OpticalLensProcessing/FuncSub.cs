using System;
using static System.Math;

namespace OpticalLensProcessing
{
    namespace FuncSub {
        interface IFunc<T>
        {
            T getA(T theta_R, T theta_0);
            T getR(T theta_A, T theta_0);
            T get0(T theta_A, T theta_R);
            T get__b(T theta_a);
            T culc(int a, int b, T theta_A, T theta_R);
            T get1stArg(int a, int b, T theta_A, T theta_R);
            T get2ndArg(int a, int b, T theta_A, T theta_R);
        }
        public abstract class BaseFunc : IFunc<float>
        {
            public float getX(int a, int b, float theta_A, float theta_R)
            {
                return a * b /
                    (float)Sqrt(
                        (float)Pow(a * a * (float)Sin(theta_R), 2)
                        + (float)Pow(b * b * (float)Cos(theta_R), 2)
                    );
            }
            public float getA(float theta_R, float theta_0)
            {
                return theta_R - theta_0;
            }
            public float getR(float theta_A, float theta_0)
            {
                return theta_A + theta_0;
            }
            public float get0(float theta_A, float theta_R)
            {
                return theta_R - theta_A;
            }
            public float get__b(float theta_a)
            {
                return (float)Asin((float)Sin(theta_a) * 2 / 3);
            }
            public virtual float culc(int a, int b, float theta_A, float theta_R)
            {
                return get1stArg(a, b, theta_A, theta_R) - get2ndArg(a, b, theta_A, theta_R);
            }
            /// <summary>
            /// return l1 on 27H version
            /// </summary>
            public virtual float get1stArg(int a, int b, float theta_A, float theta_R)
            {
                return getX(a, b, theta_A, theta_R)
                    * (float)Sin(theta_R)
                    * (float)Tan((float)PI / 2 - theta_R + get__b(theta_A));
            }
            /// <summary>
            /// return S1 on 27H version
            /// </summary>
            public virtual float get2ndArg(int a, int b, float theta_A, float theta_R)
            {
                return getX(a, b, theta_A, theta_R) * (float)Cos(theta_R);
            }
        }
        class FuncVer1 : BaseFunc
        {
        }
        class FuncVer2 : FuncVer1
        {
            /// <summary>
            /// return S2 on 27H version
            /// 3rd arg : theta_R => PI - theta_R
            /// </summary>
            public override float get1stArg(int a, int b, float theta_A, float theta_R)
            {
                return base
                    .get1stArg(a, b, theta_A, (float)PI - theta_R);
            }
            /// <summary>
            /// return d2 on 27H version
            /// 引き算を足し算にする。culc()は同一
            /// </summary>
            public override float get2ndArg(int a, int b, float theta_A, float theta_R)
            {
                return base
                    .get2ndArg(a, b, theta_A, theta_R)
                    * (-1);
            }
        }
    }
}
