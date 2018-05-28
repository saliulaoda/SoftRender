using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRender_Windows
{
    class Def
    {
        public static int DeviceWidth = 800;
        public static int DeviceHeight = 600;


        public static vertex_t[] mesh = new vertex_t[8];

        static float[,] meshData_Box = new float[,]
        {
            {  1, -1,  1, 1,    0, 0,    1.0f, 0.2f, 0.2f,    1 },
            { -1, -1,  1, 1,    0, 1,    0.2f, 1.0f, 0.2f,    1 },
            { -1,  1,  1, 1,    1, 1,    0.2f, 0.2f, 1.0f,    1 },
            {  1,  1,  1, 1,    1, 0,    1.0f, 0.2f, 1.0f,    1 },
            {  1, -1, -1, 1,    0, 0,    1.0f, 1.0f, 0.2f,    1 },
            { -1, -1, -1, 1,    0, 1,    0.2f, 1.0f, 1.0f,    1 },
            { -1,  1, -1, 1,    1, 1,    1.0f, 0.3f, 0.3f,    1 },
            {  1,  1, -1, 1,    1, 0,    0.2f, 1.0f, 0.3f,    1 },
        };

        public static void InitMesh_Box()
        {
            for (int i=0; i<8;++i)
            {
                mesh[i] = new vertex_t();
                mesh[i].pos = new vector_t(meshData_Box[i,0], meshData_Box[i,1], meshData_Box[i,2], meshData_Box[i,3]);
                mesh[i].tc = new texcoord_t(meshData_Box[i, 4], meshData_Box[i, 5]);
                mesh[i].color = new color_t((int)(meshData_Box[i, 6]*255), (int)(meshData_Box[i, 7] * 255), (int)(meshData_Box[i, 8] * 255));
                mesh[i].rhw = meshData_Box[i,9];
            }
        }
    }

    struct point_t
    {
        public int x;
        public int y;
        public point_t(int _x, int _y)
        {
            x = _x; y = _y;
        }
    }

    struct vector_t
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public vector_t(float _x, float _y, float _z, float _w)
        {
            x = _x; y = _y; z = _z; w = _w;
        }
    }

    struct texcoord_t
    {
        public float u;
        public float v;
        public texcoord_t(float _u, float _v)
        {
            u = _u; v = _v;
        }
    }

    struct vertex_t
    {
        public vector_t pos;
        public texcoord_t tc;
        public color_t color;
        public float rhw;
    }

    struct matrix_t
    {
        public int row;
        public int column;
        public float[,] m;
        public matrix_t(int _r, int _c)
        {
            row = _r; column = _c;
            m = new float[row, column];
        }

        public void SetZero()
        {
            Array.Clear(m, 0, row*column);
        }

        public void SetIdentity()
        {
            SetZero();
            m[0,0] = m[1,1] = m[2,2] = m[3,3] = 1f;
        }

        // 平移变换
        public void SetTranslate(float x, float y, float z)
        {
            SetIdentity();
            m[3,0] = x;
            m[3,1] = y;
            m[3,2] = z;
        }

        // 缩放变换
        public void SetScale(float x, float y, float z)
        {
            SetIdentity();
            m[0,0] = x;
            m[1,1] = y;
            m[2,2] = z;
        }

        // 旋转矩阵
        public void SetRotate(vector_t _rotateV)
        {
            SetRotate(_rotateV.x, _rotateV.y, _rotateV.z, _rotateV.w);
        }

        public void SetRotate(float x, float y, float z, float theta)
        {
            float qsin = (float)Math.Sin(theta * 0.5f);
            float qcos = (float)Math.Cos(theta * 0.5f);
            vector_t vec = new vector_t( x, y, z, 1f);
            float w = qcos;
            MathTool.vector_normalize(vec);
            x = vec.x * qsin;
            y = vec.y * qsin;
            z = vec.z * qsin;
            m[0,0] = 1 - 2 * y * y - 2 * z * z;
            m[1,0] = 2 * x * y - 2 * w * z;
            m[2,0] = 2 * x * z + 2 * w * y;
            m[0,1] = 2 * x * y + 2 * w * z;
            m[1,1] = 1 - 2 * x * x - 2 * z * z;
            m[2,1] = 2 * y * z - 2 * w * x;
            m[0,2] = 2 * x * z - 2 * w * y;
            m[1,2] = 2 * y * z + 2 * w * x;
            m[2,2] = 1 - 2 * x * x - 2 * y * y;
            m[0,3] = m[1,3] = m[2,3] = 0f;
            m[3,0] = m[3,1] = m[3,2] = 0f;
            m[3,3] = 1f;
        }

        public void LogMatrixInfo(string _tag = "")
        {
            Console.WriteLine("============ LogMatrixInfo ============ " + _tag);
            for (int i = 0; i < row; ++i)
            {
                string _tmpStr = "";
                for (int j = 0; j < column; ++j)
                {
                    _tmpStr += string.Format("{0:0000.00}    ", m[i, j]);
                }
                Console.WriteLine(_tmpStr);
            }
        }
    }

    class color_t
    {
        public int r;
        public int g;
        public int b;
        public color_t(int _r, int _g, int _b)
        {
            r = _r; g = _g; b = _b;
        }
    }

    class Transformer
    {
        public matrix_t world;         // 世界坐标变换
        public matrix_t view;          // 摄影机坐标变换
        public matrix_t projection;    // 投影变换
        public matrix_t transform;     // transform = world * view * projection

        public void Init()
        {
            world = new matrix_t(4, 4);
            world.SetIdentity();

            view = new matrix_t(4, 4);
            view.SetIdentity();

            //Init Projection
            projection = new matrix_t(4, 4);
            float fovy = 3.1415926f * 0.5f;
            float aspect = (float)Def.DeviceWidth / ((float)Def.DeviceHeight);
            float zn = 1f;
            float zf = 500f;
            float fax = 1.0f / (float)Math.Tan(fovy * 0.5f);
            projection.SetZero();
            projection.m[0,0] = (float)(fax / aspect);
            projection.m[1,1] = (float)(fax);
            projection.m[2,2] = zf / (zf - zn);
            projection.m[3,2] = -zn * zf / (zf - zn);
            projection.m[2,3] = 1;
        }

        public void UpdateTransform()
        {
            matrix_t _tmp = MathTool.matrix_mul(world, view);
            transform = MathTool.matrix_mul(_tmp, projection);
        }

        
    }
}
