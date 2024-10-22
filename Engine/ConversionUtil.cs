﻿using Assimp;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class ConversionUtil
    {

        public static Matrix ToSharpDXMatrix(Matrix4x4 matrix)
        {
            Matrix ret = new Matrix(matrix.A1, matrix.A2, matrix.A3, matrix.A4, matrix.B1, matrix.B2, matrix.B3, matrix.B4, matrix.C1, matrix.C2, matrix.C3, matrix.C4, matrix.D1, matrix.D2, matrix.D3, matrix.D4);

            return ret;
        }
    }
}
