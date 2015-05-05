using System;

namespace SEModAPIInternal.API.Common
{
    public static class PathManager
    {
        public static string BasePath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }
    }
}