﻿using DebugUtils.Objects;
using DebugUtils.Objects.Maps;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugOcean : DebugMap
    {
        public DebugOcean(DebugObject parent = null) : base("Ocean", parent)
        {
            Position = new Rectangle(0, 0, DebugRes.Ocean.Width, DebugRes.Ocean.Height);
            Visible = true;
        }

        public override Bitmap GetFigure() => DebugRes.Ocean;
    }
}
