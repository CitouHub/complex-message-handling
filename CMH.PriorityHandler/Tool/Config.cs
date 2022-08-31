#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace CMH.PriorityHandler.Tool
{
    public class Config
    {
        private static IConfiguration? _configuration;

        public PriorityClass Priority { get; set; } = new();

        public ProcessClass Process { get; set; } = new();

        public QueueCacheClass QueueCache { get; set; } = new();

        public class QueueCacheClass
        {
            public int RefreshInterval
            {
                get
                {
                    return _configuration.GetValue<int>("QueueCache:RefreshInterval");
                }
            }
        }

        public class PriorityClass
        {
            public BackoffPolicyClass BackoffPolicy { get; set; } = new();

            public int MessageBatch
            {
                get
                {
                    return _configuration.GetValue<int>("Priority:MessageBatch");
                }
            }

            public class BackoffPolicyClass
            {
                public int InitialSleepTime
                {
                    get
                    {
                        return _configuration.GetValue<int>("Priority:BackoffPolicy:InitialSleepTime");
                    }
                }

                public double BackoffSleepTimeFactor
                {
                    get
                    {
                        return _configuration.GetValue<double>("Priority:BackoffPolicy:BackoffSleepTimeFactor");
                    }
                }

                public int MaxSleepTime
                {
                    get
                    {
                        return _configuration.GetValue<int>("Priority:BackoffPolicy:MaxSleepTime");
                    }
                }
            }
        }

        public class ProcessClass
        {
            public BackoffPolicyClass BackoffPolicy { get; set; } = new();

            public string DefaultProcessChannel
            {
                get
                {
                    return _configuration.GetValue<string>("Process:ProcessChannel_Default");
                }
            }

            public Dictionary<short, string> DataSourceProcessChannelMap
            {
                get
                {
                    return _configuration.GetSection("Process:DataSourceProcessChannelMap")
                        .GetChildren().ToDictionary(x => short.Parse(x.Key), x => x.Value);
                }
            }

            public class BackoffPolicyClass
            {
                public short ProcessChannelSize
                {
                    get
                    {
                        return _configuration.GetValue<short>("Process:BackoffPolicy:ProcessChannelSize");
                    }
                }

                public short PriorityStepSize
                {
                    get
                    {
                        return _configuration.GetValue<short>("Process:BackoffPolicy:PriorityStepSize");
                    }
                }

                public int InitialSleepTime
                {
                    get
                    {
                        return _configuration.GetValue<int>("Process:BackoffPolicy:InitialSleepTime");
                    }
                }

                public double PrioritySleepTimeFactor
                {
                    get
                    {
                        return _configuration.GetValue<double>("Process:BackoffPolicy:PrioritySleepTimeFactor");
                    }
                }

                public double TrySleepTimeFactor
                {
                    get
                    {
                        return _configuration.GetValue<double>("Process:BackoffPolicy:TrySleepTimeFactor");
                    }
                }

                public int MaxSleepTime
                {
                    get
                    {
                        return _configuration.GetValue<int>("Process:BackoffPolicy:MaxSleepTime");
                    }
                }
            }
        }

        public Config(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
