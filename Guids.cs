// Guids.cs
// MUST match guids.h
using System;

namespace DeltaEngine.TestAfterBuild
{
    static class GuidList
    {
        public const string guidTestAfterBuildPkgString = "ede6f27f-08b8-426a-948d-9c23965753dd";
        public const string guidTestAfterBuildCmdSetString = "1daac2f9-c14d-40d0-bd2d-dd431baf15bf";

        public static readonly Guid guidTestAfterBuildCmdSet = new Guid(guidTestAfterBuildCmdSetString);
    };
}