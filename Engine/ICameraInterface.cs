using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public interface ICameraInterface
    {
        Vector3 Position { get; }
        Vector3 Forward { get; }
        Vector3 UpVector { get; }

        Matrix ViewMatrix { get; }
        Matrix ProjectionMatrix { get; }
    }
}
