using System.Collections.Generic;

using CMH.Common.Variable;
using CMH.Data.Model;

namespace CMH.Process.Util
{
    public static class Cache
    {
        public static Dictionary<short, DataSource> DataSource = new();
        public static Dictionary<ProcessChannel, ProcessChannelPolicy> ProcessChannelPolicy = new();
    }
}
