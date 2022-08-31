#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace CMH.PriorityHandler.Tool
{
    public class Config
    {
        private static IConfiguration? _configuration;

        public QueueCacheClass QueueCache { get; set; } = new();

        public PriorityClass Priority { get; set; } = new();        

        public BackoffPolicyClass BackoffPolicy { get; set; } = new();

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
            public int MessageBatch
            {
                get
                {
                    return _configuration.GetValue<int>("Priority:MessageBatch");
                }
            }

            public string DefaultProcessChannel
            {
                get
                {
                    return _configuration.GetValue<string>("Priority:ProcessChannel_Default");
                }
            }

            public Dictionary<short, string> DataSourceProcessChannelMap
            {
                get
                {
                    return _configuration.GetSection("Priority:DataSourceProcessChannelMap")
                        .GetChildren().ToDictionary(x => short.Parse(x.Key), x => x.Value);
                }
            }
        }

        public class BackoffPolicyClass
        {
            public EmptyIterationClass EmptyIteration { get; set; } = new();

            public ProcessChannelFullClass ProcessChannelFull { get; set; } = new();

            public class EmptyIterationClass
            {
                public int InitialSleepTime
                {
                    get
                    {
                        return _configuration.GetValue<int>("BackoffPolicy:EmptyIteration:InitialSleepTime");
                    }
                }

                public double BackoffFactor
                {
                    get
                    {
                        return _configuration.GetValue<double>("BackoffPolicy:EmptyIteration:BackoffFactor");
                    }
                }

                public int MaxSleepTime
                {
                    get
                    {
                        return _configuration.GetValue<int>("BackoffPolicy:EmptyIteration:MaxSleepTime");
                    }
                }
            }

            public class ProcessChannelFullClass
            {
                public short ProcessChannelSize
                {
                    get
                    {
                        return _configuration.GetValue<short>("BackoffPolicy:ProcessChannelFull:ProcessChannelSize");
                    }
                }

                public short PriorityStepSize
                {
                    get
                    {
                        return _configuration.GetValue<short>("BackoffPolicy:ProcessChannelFull:PriorityStepSize");
                    }
                }

                public int InitialSleepTime
                {
                    get
                    {
                        return _configuration.GetValue<int>("BackoffPolicy:ProcessChannelFull:InitialSleepTime");
                    }
                }

                public double PriorityFactor
                {
                    get
                    {
                        return _configuration.GetValue<double>("BackoffPolicy:ProcessChannelFull:PriorityFactor");
                    }
                }

                public double TryFactor
                {
                    get
                    {
                        return _configuration.GetValue<double>("BackoffPolicy:ProcessChannelFull:TryFactor");
                    }
                }

                public int MaxSleepTime
                {
                    get
                    {
                        return _configuration.GetValue<int>("BackoffPolicy:ProcessChannelFull:MaxSleepTime");
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
