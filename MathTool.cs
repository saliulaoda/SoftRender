using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SoftRender_Windows
{
    class MathTool
    {
        public static int CMID(int x, int min, int max) { return (x < min) ? min : ((x > max) ? max : x); }

        public static float CMID_F(float x, float min, float max) { return (x < min) ? min : ((x > max) ? max : x); }

        #region vector operation
        // | v |
        public static float vector_length(vector_t v)
        {
            return (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        }

        // v1 + v2
        public static vector_t vector_add(vector_t v1, vector_t v2)
        {
            return new vector_t(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, 1f);
        }

        // v1 - v2
        public static vector_t vector_sub(vector_t v1, vector_t v2)
        {
            return new vector_t(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, 1f);
        }

        // 矢量点乘
        public static float vector_dotproduct(vector_t v1, vector_t v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        // 矢量叉乘
        public static vector_t vector_crossproduct(vector_t v1, vector_t v2)
        {
            return new vector_t(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x, 1f);
        }

        // 计算插值：t 为 [0, 1] 之间的数值
        public static float interp(float x1, float x2, float t) { return x1 + (x2 - x1) * t; }

        // 矢量插值，t取值 [0, 1]
        public static vector_t vector_interp(vector_t v1, vector_t v2, float t)
        {
            return new vector_t(interp(v1.x, v2.x, t), interp(v1.y, v2.y, t), interp(v1.z, v2.z, t), 1);
        }

        // 矢量归一化
        public static vector_t vector_normalize(vector_t v)
        {
            float length = vector_length(v);
            if (length != 0.0f)
            {
                float inv = 1.0f / length;
                return new vector_t(v.x * inv, v.y * inv, v.z * inv, v.w);
            }
            return v;
        }
        #endregion

        #region matrix_t operation

        // a + b
        public static matrix_t matrix_add(matrix_t a, matrix_t b)
        {
            matrix_t _tmp = a;
            if(a.row == b.row && a.column == b.column)
            {
                for (int i = 0; i < _tmp.row; i++)
                {
                    for (int j = 0; j < _tmp.column; j++)
                        _tmp.m[i, j] += b.m[i, j];
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "matrix_t Operation Error : matrix_add");
            }
            return _tmp;
        }

        // a - b
        public static matrix_t matrix_sub(matrix_t a, matrix_t b)
        {
            matrix_t _tmp = a;
            if (a.row == b.row && a.column == b.column)
            {
                for (int i = 0; i < _tmp.row; i++)
                {
                    for (int j = 0; j < _tmp.column; j++)
                        _tmp.m[i, j] -= b.m[i, j];
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "matrix_t Operation Error : matrix_sub");
            }
            return _tmp;
        }

        // a * b
        public static matrix_t matrix_mul(matrix_t a, matrix_t b)
        {
            matrix_t _tmp = new matrix_t(a.row, b.column);
            if(a.column == b.row)
            {
                for (int i = 0; i < a.row; i++)
                {
                    for (int j = 0; j < b.column; j++)
                    {
                        for (int k = 0; k < b.column; k++)
                        {
                            _tmp.m[i, j] += a.m[i, k] * b.m[k, j];
                        }
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "matrix_t Operation Error : matrix_mul");
            }
            return _tmp;
        }

        // a(matrix_t) * f(float)
        public static matrix_t matrix_scale(matrix_t a, float f)
        {
            matrix_t _tmp = a;
            for (int i = 0; i < _tmp.row; i++)
            {
                for (int j = 0; j < _tmp.column; j++)
                    _tmp.m[i, j] *= f;
            }
            return _tmp;
        }

        // v(vector_t) * m(matrix_t) 这里默认矩阵row = 4
        public static vector_t matrix_apply(vector_t v, matrix_t m)
        {
            vector_t _tmp = v;
            if (m.row == 4)
            {
                float[] _val = new float[4];
                for(int i=0;i<4;++i)
                {
                    _val[i] = v.x * m.m[0, i] + v.y * m.m[1, i] + v.z * m.m[2, i] + v.w * m.m[3, i];
                }
                _tmp.x = _val[0]; _tmp.y = _val[1]; _tmp.z = _val[2]; _tmp.w = _val[3];
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "matrix_t Operation Error : matrix_apply");
            }
            return _tmp;
        }



        #endregion

        // 归一化，得到屏幕坐标
        public static vector_t Homogenize(vector_t x)
        {
            vector_t _tmp = new vector_t();
            float rhw = 1.0f / x.w;
            _tmp.x = (x.x* rhw + 1.0f) * Def.DeviceWidth * 0.5f;
            _tmp.y = (1.0f - x.y* rhw) * Def.DeviceHeight * 0.5f;
            _tmp.z = x.z* rhw;
            _tmp.w = 1.0f;
            return _tmp;
        }
    }
}
