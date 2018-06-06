using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace SoftRender_Windows
{
    class Device
    {
        public vector_t mRotateVector;
        public vector_t mCameraVector;
        Transformer mTransformer;

        Bitmap frameBuffer;

        float[,] zBuffer = new float[Def.DeviceWidth, Def.DeviceHeight];

        public Device()
        {
            mTransformer = new Transformer();
            mTransformer.Init();
            frameBuffer = new Bitmap(Def.DeviceWidth, Def.DeviceHeight);

            //TestDevice();
            CameraInit(3, 0, 0);

            Def.InitMesh_Box();

            mRotateVector = new vector_t(0.1f, 0.7f, 0.5f, 1f);
            //mRotateVector = new vector_t(0f, 0f, 0f, 1f);
            DrawBox();
        }

        public void TestDevice() 
        {
            DrawLine(new point_t(50, 500), new point_t(100, 50), Color.Red);
        }

        public Bitmap GetFrameBuffer()
        {
            return frameBuffer;
        }

        public void ResetFrameBuffer()
        {
            frameBuffer = new Bitmap(Def.DeviceWidth, Def.DeviceHeight);
        }

        public void DrawPoint(point_t _a, Color _color)
        {
            DrawPoint(_a.x, _a.y, _color);
        }

        public void DrawPoint_S(point_t _a, Color _color, int _radio)
        {
            for (int i=_a.x - _radio; i<_a.x + _radio; i++)
            {
                for (int j=_a.y - _radio; j<_a.y + _radio; j++)
                {
                    DrawPoint(i, j, _color);
                }
            }
            
        }

        public void DrawPoint(int _x, int _y, Color _color)
        {
            int _tmpX = Tools.CMID(_x, 0, Def.DeviceWidth - 1);
            int _tmpY = Tools.CMID(_y, 0, Def.DeviceHeight - 1);
            frameBuffer.SetPixel(_tmpX, _tmpY, _color);
        }

        public void DrawLine(point_t _a, point_t _b, Color _color)
        {
            if (_a.x == _b.x && _a.y == _b.y)
            {
                DrawPoint(_a, _color);
            }
            else if (_a.x == _b.x)
            {
                int inc = (_a.y <= _b.y) ? 1 : -1;
                for (int y = _a.y; y != _b.y; y += inc) DrawPoint(_a.x, y, _color);
                DrawPoint(_b, _color);
            }
            else if (_a.y == _b.y)
            {
                int inc = (_a.x <= _b.x) ? 1 : -1;
                for (int x = _a.x; x != _b.x; x += inc) DrawPoint(x, _b.y, _color);
                DrawPoint(_b, _color);
            }
            else
            {
                int dx = (_a.x < _b.x) ? _b.x - _a.x : _a.x - _b.x;
                int dy = (_a.y < _b.y) ? _b.y - _a.y : _a.y - _b.y;

                if (dx >= dy)
                {
                    if (_b.x < _a.x)
                    {
                        int tmp1 = _a.x; int tmp2 = _a.y;
                        _a.x = _b.x; _a.y = _b.y;
                        _b.x = tmp1; _b.y = tmp2;
                    }
                    int inc = (_b.y >= _a.y) ? 1 : -1;
                    int rem = 0;
                    for (int x = _a.x, y = _a.y; x <= _b.x; x++)
                    {
                        DrawPoint(x, y, _color);
                        rem += dy;
                        if (rem >= dx)
                        {
                            rem -= dx;
                            y += inc;
                            DrawPoint(x, y, _color);
                        }
                    }
                    DrawPoint(_b, _color);
                }
                else
                {
                    if (_b.y < _a.y)
                    {
                        int tmp1 = _a.x; int tmp2 = _a.y;
                        _a.x = _b.x; _a.y = _b.y;
                        _b.x = tmp1; _b.y = tmp2;
                    }
                    int inc = (_b.x >= _a.x) ? 1 : -1;
                    int rem = 0;
                    for (int x = _a.x, y = _a.y; y <= _b.y; y++)
                    {
                        DrawPoint(x, y, _color);
                        rem += dx;
                        if (rem >= dy)
                        {
                            rem -= dy;
                            x += inc;
                            DrawPoint(x, y, _color);
                        }
                    }
                    DrawPoint(_b, _color);
                }
            }
        }

        void CameraInit(float x, float y, float z)
        {
            mCameraVector = new vector_t(x, y, z, 1);
            vector_t at = new vector_t(0, 0, 0, 1);
            vector_t up = new vector_t(0, 0, 1, 1);
            SetLookat(mCameraVector, at, up);
            mTransformer.UpdateTransform();
        }


        //public void UpdateCameraPosition(float _x, float _y, float _z)
        //{
        //    vector_t eye = new vector_t(_x, _y, _z, 1);
        //    vector_t at = new vector_t(0, 0, 0, 1);
        //    vector_t up = new vector_t(0, 0, 1, 1);
        //    SetLookat(eye, at, up);
        //}

        // 设置摄像机
        void SetLookat(vector_t eye, vector_t at, vector_t up)
        {
            vector_t xaxis, yaxis, zaxis;
            zaxis = Tools.vector_sub(at, eye);
            zaxis = Tools.vector_normalize(zaxis);

            xaxis = Tools.vector_crossproduct(up, zaxis);
            xaxis = Tools.vector_normalize(xaxis);

            yaxis = Tools.vector_crossproduct(zaxis, xaxis);

            mTransformer.view.m[0, 0] = xaxis.x;
            mTransformer.view.m[1, 0] = xaxis.y;
            mTransformer.view.m[2, 0] = xaxis.z;
            mTransformer.view.m[3, 0] = -Tools.vector_dotproduct(xaxis, eye);

            mTransformer.view.m[0, 1] = yaxis.x;
            mTransformer.view.m[1, 1] = yaxis.y;
            mTransformer.view.m[2, 1] = yaxis.z;
            mTransformer.view.m[3, 1] = -Tools.vector_dotproduct(yaxis, eye);

            mTransformer.view.m[0, 2] = zaxis.x;
            mTransformer.view.m[1, 2] = zaxis.y;
            mTransformer.view.m[2, 2] = zaxis.z;
            mTransformer.view.m[3, 2] = -Tools.vector_dotproduct(zaxis, eye);

            mTransformer.view.m[0, 3] = mTransformer.view.m[1, 3] = mTransformer.view.m[2, 3] = 0.0f;
            mTransformer.view.m[3, 3] = 1.0f;
        }


        public void DrawBox()
        {
            matrix_t ma = new matrix_t(4, 4);
            ma.SetRotate(mRotateVector);
            mTransformer.world = ma;
            mTransformer.UpdateTransform();

            DrawPlane(3, 7, 6, 2);
            DrawPlane(1, 2, 6, 5);
            DrawPlane(4, 5, 6, 7);
            DrawPlane(0, 4, 5, 1);
            DrawPlane(0, 1, 2, 3);
            DrawPlane(0, 4, 7, 3);
        }

        void DrawPlane(int vertexIndex1, int vertexIndex2, int vertexIndex3, int vertexIndex4)
        {
            vertex_t p1 = Def.mesh[vertexIndex1], p2 = Def.mesh[vertexIndex2], p3 = Def.mesh[vertexIndex3], p4 = Def.mesh[vertexIndex4];
            DrawTriangle(p1, p2, p3);
            DrawTriangle(p1, p3, p4);
        }

        void DrawPlane_1(int vertexIndex1, int vertexIndex2, int vertexIndex3, int vertexIndex4)
        {
            vertex_t v1 = Def.mesh[vertexIndex1], v2 = Def.mesh[vertexIndex2], v3 = Def.mesh[vertexIndex3], v4 = Def.mesh[vertexIndex4];
            vector_t p1, p2, p3, p4, c1, c2, c3, c4;
            c1 = Tools.matrix_apply(v1.pos, mTransformer.transform);
            c2 = Tools.matrix_apply(v2.pos, mTransformer.transform);
            c3 = Tools.matrix_apply(v3.pos, mTransformer.transform);
            c4 = Tools.matrix_apply(v4.pos, mTransformer.transform);

            p1 = Tools.Homogenize(c1);
            p2 = Tools.Homogenize(c2);
            p3 = Tools.Homogenize(c3);
            p4 = Tools.Homogenize(c4);

            point_t _node1 = new point_t((int)p1.x, (int)p1.y);
            point_t _node2 = new point_t((int)p2.x, (int)p2.y);
            point_t _node3 = new point_t((int)p3.x, (int)p3.y);
            point_t _node4 = new point_t((int)p4.x, (int)p4.y);

            DrawLine(_node1, _node2, v1.color);
            DrawLine(_node2, _node3, v2.color);
            DrawLine(_node3, _node4, v3.color);
            DrawLine(_node4, _node1, v4.color);
        }

        void DrawTriangle(vertex_t v1, vertex_t v2, vertex_t v3)
        {
            vector_t p1, p2, p3, c1, c2, c3;

            // 按照 Transform 变化
            c1 = Tools.matrix_apply(v1.pos, mTransformer.transform);
            c2 = Tools.matrix_apply(v2.pos, mTransformer.transform);
            c3 = Tools.matrix_apply(v3.pos, mTransformer.transform);

            p1 = Tools.Homogenize(c1);
            p2 = Tools.Homogenize(c2);
            p3 = Tools.Homogenize(c3);

            point_t _point1 = new point_t((int)p1.x, (int)p1.y);
            point_t _point2 = new point_t((int)p2.x, (int)p2.y);
            point_t _point3 = new point_t((int)p3.x, (int)p3.y);


            //画线框
            //DrawLine(_point1, _point2, Color.Black);
            //DrawLine(_point2, _point3, Color.Black);
            //DrawLine(_point1, _point3, Color.Black);

            //DrawPoint_S(_point1, Color.Red, 3);
            //DrawPoint_S(_point2, Color.Yellow, 3);
            //DrawPoint_S(_point3, Color.Blue, 3);

            if (TriangleCullingJudge_A(c1, c2, c3))
            {
            }
            else
            {
                //光栅化填充
                triangle_2D _triangle = new triangle_2D();
                _triangle.mPoints[0] = _point1; _triangle.mPoints[1] = _point2; _triangle.mPoints[2] = _point3;
                _triangle.mColors[0] = v1.color; _triangle.mColors[1] = v2.color; _triangle.mColors[2] = v3.color;

                Rasterization_Triangle(_triangle);
            }
        }

        #region rasterization operation

        //光栅化步骤的三角形数据，顶点信息
        class triangle_2D
        {
            public point_t[] mPoints;
            public Color[] mColors;
            public triangle_2D()
            {
                mPoints = new point_t[3];
                mColors = new Color[3];
            }
        }

        //平底 A
        //平顶 V

        void Rasterization_Triangle(triangle_2D _triangle)
        {
            point_t p1 = _triangle.mPoints[0], p2 = _triangle.mPoints[1], p3 = _triangle.mPoints[2];
            int x1 = p1.x, y1 = p1.y, x2 = p2.x, y2 = p2.y, x3 = p3.x, y3 = p3.y;
            if (y1 == y2)
            {
                if (y3 <= y1) // A  
                {
                    Rasterization_Triangle_A(x3, y3, x1, y1, x2, y2, _triangle);
                }
                else // V
                {
                    Rasterization_Triangle_V(x1, y1, x2, y2, x3, y3, _triangle);
                }
            }
            else if (y1 == y3)
            {
                if (y2 <= y1) // A 
                {
                    Rasterization_Triangle_A(x2, y2, x1, y1, x3, y3, _triangle);
                }
                else // V
                {
                    Rasterization_Triangle_V(x1, y1, x3, y3, x2, y2, _triangle);
                }
            }
            else if (y2 == y3)
            {
                if (y1 <= y2) // A  
                {
                    Rasterization_Triangle_A(x1, y1, x2, y2, x3, y3, _triangle);
                }
                else // V
                {
                    Rasterization_Triangle_V(x2, y2, x3, y3, x1, y1, _triangle);
                }
            }
            else
            {
                int xtop = 0, ytop = 0, xmiddle = 0, ymiddle = 0, xbottom = 0, ybottom = 0;
                if (y1 < y2 && y2 < y3) // y1 y2 y3  
                {
                    xtop = x1;
                    ytop = y1;
                    xmiddle = x2;
                    ymiddle = y2;
                    xbottom = x3;
                    ybottom = y3;
                }
                else if (y1 < y3 && y3 < y2) // y1 y3 y2  
                {
                    xtop = x1;
                    ytop = y1;
                    xmiddle = x3;
                    ymiddle = y3;
                    xbottom = x2;
                    ybottom = y2;
                }
                else if (y2 < y1 && y1 < y3) // y2 y1 y3  
                {
                    xtop = x2;
                    ytop = y2;
                    xmiddle = x1;
                    ymiddle = y1;
                    xbottom = x3;
                    ybottom = y3;
                }
                else if (y2 < y3 && y3 < y1) // y2 y3 y1  
                {
                    xtop = x2;
                    ytop = y2;
                    xmiddle = x3;
                    ymiddle = y3;
                    xbottom = x1;
                    ybottom = y1;
                }
                else if (y3 < y1 && y1 < y2) // y3 y1 y2  
                {
                    xtop = x3;
                    ytop = y3;
                    xmiddle = x1;
                    ymiddle = y1;
                    xbottom = x2;
                    ybottom = y2;
                }
                else if (y3 < y2 && y2 < y1) // y3 y2 y1  
                {
                    xtop = x3;
                    ytop = y3;
                    xmiddle = x2;
                    ymiddle = y2;
                    xbottom = x1;
                    ybottom = y1;
                }
                int xl; // 长边在ymiddle时的x，来决定长边是在左边还是右边  
                xl = (int)((ymiddle - ytop) * (xbottom - xtop) / (ybottom - ytop) + xtop + 0.5f);

                if (xl <= xmiddle) // 左三角形  
                { 
                    Rasterization_Triangle_A(xtop, ytop, xl, ymiddle, xmiddle, ymiddle, _triangle);  
                    Rasterization_Triangle_V(xl, ymiddle, xmiddle, ymiddle, xbottom, ybottom, _triangle);
                }
                else // 右三角形  
                { 
                    Rasterization_Triangle_A(xtop, ytop, xmiddle, ymiddle, xl, ymiddle, _triangle); 
                    Rasterization_Triangle_V(xmiddle, ymiddle, xl, ymiddle, xbottom, ybottom, _triangle);
                }
            }
        }

        void Rasterization_Triangle_A(int x1, int y1, int x2, int y2, int x3, int y3, triangle_2D _triangle)  
        {
            for (int y = y1; y <= y2; ++y)
            {
                int xs, xe;
                xs = (int)((y - y1) * (x2 - x1) / (y2 - y1) + x1 + 0.5f);
                xe = (int)((y - y1) * (x3 - x1) / (y3 - y1) + x1 + 0.5f);
                Rasterization_DrawLine_Horizontal(new point_t(xs, y), new point_t(xe, y), _triangle);
            }
        }

        void Rasterization_Triangle_V(int x1, int y1, int x2, int y2, int x3, int y3, triangle_2D _triangle) 
        {
            for (int y = y1; y <= y3; ++y)
            {
                int xs, xe;
                xs = (int)((y - y1) * (x3 - x1) / (y3 - y1) + x1 + 0.5f);
                xe = (int)((y - y2) * (x3 - x2) / (y3 - y2) + x2 + 0.5f);
                Rasterization_DrawLine_Horizontal(new point_t(xs, y), new point_t(xe, y), _triangle);
            }
        }

        void Rasterization_DrawLine_Horizontal(point_t _pS, point_t _pE, triangle_2D _triangle)
        {
            int _start = Math.Min(_pS.x, _pE.x);
            int _end = Math.Max(_pS.x, _pE.x);
            for (int x = _start; x <= _end; x++)
            {
                point_t _cur = new point_t(x, _pS.y);
                Color _col = Triangle_Interpolation_Color(_cur, _triangle);
                DrawPoint(_cur.x, _cur.y, _col);
            }
        }

        Color Triangle_Interpolation_Color(point_t _c, triangle_2D _triangle)
        {
            Color _tmp = _triangle.mColors[0];
            float[] acreageList = new float[3];
            acreageList[0] = Tools.Triangle_Acreage_2D(_c, _triangle.mPoints[1], _triangle.mPoints[2]);
            acreageList[1] = Tools.Triangle_Acreage_2D(_c, _triangle.mPoints[0], _triangle.mPoints[2]);
            acreageList[2] = Tools.Triangle_Acreage_2D(_c, _triangle.mPoints[0], _triangle.mPoints[1]);
            float _sum = 0f, _r = 0f, _g = 0f, _b = 0f;
            for (int i=0;i<3;++i)
            {
                _sum += acreageList[i];
                _r += _triangle.mColors[i].R * acreageList[i];
                _g += _triangle.mColors[i].G * acreageList[i];
                _b += _triangle.mColors[i].B * acreageList[i];
            }
            _tmp = Color.FromArgb(Tools.CMID((int)(_r / _sum), 0, 255),
                            Tools.CMID((int)(_g / _sum), 0, 255),
                            Tools.CMID((int)(_b / _sum), 0, 255));
            return _tmp;
        }

        #endregion

        #region culling operation

        //是否可剔除
        bool TriangleCullingJudge_A(vector_t v1, vector_t v2, vector_t v3)
        {
            vector_t _v1_2 = Tools.vector_normalize(Tools.vector_sub(v2, v1));
            vector_t _v1_3 = Tools.vector_normalize(Tools.vector_sub(v3, v1));
            vector_t _normal = Tools.vector_crossproduct(_v1_2, _v1_3);

            vector_t _cameraVector = Tools.matrix_apply(mCameraVector, mTransformer.transform);

            float _val = Tools.vector_dotproduct(_normal, _cameraVector);
            if(_val > 0f)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
