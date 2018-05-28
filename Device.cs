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
            DrawBox();
        }

        public void TestDevice() 
        {
            color_t _red = new color_t(220, 20, 60);
            DrawLine(new point_t(50, 500), new point_t(100, 50), _red);
        }

        public Bitmap GetFrameBuffer()
        {
            return frameBuffer;
        }

        public void ResetFrameBuffer()
        {
            frameBuffer = new Bitmap(Def.DeviceWidth, Def.DeviceHeight);
        }

        public void DrawPoint(point_t _a, color_t _color)
        {
            DrawPoint(_a.x, _a.y, _color);
        }

        public void DrawPoint(int _x, int _y, color_t _color)
        {
            Color _col = Color.FromArgb(255, _color.r, _color.g, _color.b);

            int _tmpX = MathTool.CMID(_x, 0, Def.DeviceWidth - 1);
            int _tmpY = MathTool.CMID(_y, 0, Def.DeviceHeight - 1);

            frameBuffer.SetPixel(_tmpX, _tmpY, _col);
        }

        public void DrawLine(point_t _a, point_t _b, color_t _color)
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
            vector_t eye = new vector_t(x, y, z, 1);
            vector_t at = new vector_t(0, 0, 0, 1);
            vector_t up = new vector_t(0, 0, 1, 1);
            SetLookat(eye, at, up);
            //transform_update(&device->transform);
            mTransformer.UpdateTransform();
        }


        public void UpdateCameraPosition(float _x, float _y, float _z)
        {
            vector_t eye = new vector_t(_x, _y, _z, 1);
            vector_t at = new vector_t(0, 0, 0, 1);
            vector_t up = new vector_t(0, 0, 1, 1);
            SetLookat(eye, at, up);
        }

        // 设置摄像机
        void SetLookat(vector_t eye, vector_t at, vector_t up)
        {
            vector_t xaxis, yaxis, zaxis;
            zaxis = MathTool.vector_sub(at, eye);
            zaxis = MathTool.vector_normalize(zaxis);

            xaxis = MathTool.vector_crossproduct(up, zaxis);
            xaxis = MathTool.vector_normalize(xaxis);

            yaxis = MathTool.vector_crossproduct(zaxis, xaxis);

            mTransformer.view.m[0, 0] = xaxis.x;
            mTransformer.view.m[1, 0] = xaxis.y;
            mTransformer.view.m[2, 0] = xaxis.z;
            mTransformer.view.m[3, 0] = -MathTool.vector_dotproduct(xaxis, eye);

            mTransformer.view.m[0, 1] = yaxis.x;
            mTransformer.view.m[1, 1] = yaxis.y;
            mTransformer.view.m[2, 1] = yaxis.z;
            mTransformer.view.m[3, 1] = -MathTool.vector_dotproduct(yaxis, eye);

            mTransformer.view.m[0, 2] = zaxis.x;
            mTransformer.view.m[1, 2] = zaxis.y;
            mTransformer.view.m[2, 2] = zaxis.z;
            mTransformer.view.m[3, 2] = -MathTool.vector_dotproduct(zaxis, eye);

            mTransformer.view.m[0, 3] = mTransformer.view.m[1, 3] = mTransformer.view.m[2, 3] = 0.0f;
            mTransformer.view.m[3, 3] = 1.0f;
        }



        public void DrawBox()
        {
            matrix_t ma = new matrix_t(4, 4);
            //ma.SetRotate(-1f, -0.5f, 1f, 1f);
            ma.SetRotate(mRotateVector);
            mTransformer.world = ma;
            mTransformer.UpdateTransform();

            //DrawPlane(0, 1, 2, 3);
            //DrawPlane(0, 3, 7, 4);
            //DrawPlane(3, 2, 6, 7);
            //DrawPlane(1, 2, 6, 5);
            //DrawPlane(0, 1, 5, 4);
            //DrawPlane(4, 5, 6, 7);

            DrawPlane_1(0, 1, 2, 3);
            DrawPlane_1(0, 3, 7, 4);
            DrawPlane_1(3, 2, 6, 7);
            DrawPlane_1(1, 2, 6, 5);
            DrawPlane_1(0, 1, 5, 4);
            DrawPlane_1(4, 5, 6, 7);
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
            c1 = MathTool.matrix_apply(v1.pos, mTransformer.transform);
            c2 = MathTool.matrix_apply(v2.pos, mTransformer.transform);
            c3 = MathTool.matrix_apply(v3.pos, mTransformer.transform);
            c4 = MathTool.matrix_apply(v4.pos, mTransformer.transform);

            p1 = MathTool.Homogenize(c1);
            p2 = MathTool.Homogenize(c2);
            p3 = MathTool.Homogenize(c3);
            p4 = MathTool.Homogenize(c4);

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
            c1 = MathTool.matrix_apply(v1.pos, mTransformer.transform);
            c2 = MathTool.matrix_apply(v2.pos, mTransformer.transform);
            c3 = MathTool.matrix_apply(v3.pos, mTransformer.transform);

            p1 = MathTool.Homogenize(c1);
            p2 = MathTool.Homogenize(c2);
            p3 = MathTool.Homogenize(c3);

            color_t _red = new color_t(220, 20, 60);
            color_t _gold = new color_t(255, 215, 0);
            color_t _black = new color_t(0, 0, 0);
            point_t _node1 = new point_t((int)p1.x, (int)p1.y);
            point_t _node2 = new point_t((int)p2.x, (int)p2.y);
            point_t _node3 = new point_t((int)p3.x, (int)p3.y);
            DrawLine(_node1, _node2, _black);
            //DrawLine(_node2, _node3, _gold);
            //DrawLine(_node1, _node3, _black);
            DrawLine(_node2, _node3, _black);
            DrawLine(_node1, _node3, _black);
        }
    }
}
