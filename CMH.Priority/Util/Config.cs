namespace CMH.Priority.Util
{
    public class Config
    {
        private static IConfiguration? _configuration;

        public QueueCacheClass QueueCache { get; set; } = new();

        public PriorityClass Priority { get; set; } = new();        

        public BackoffPolicyClass BackoffPolicy { get; set; } = new();

        public class QueueCacheClass
        {
            private int? _RefreshInterval;
            public int RefreshInterval
            {
                get
                {
                    return _RefreshInterval ?? _configuration.GetValue<int>("QueueCache:RefreshInterval");
                }
                set
                {
                    _RefreshInterval = value;
                }
            }
        }

        public class PriorityClass
        {
            private short? _Tasks;
            public short Tasks
            {
                get
                {
                    return _Tasks ?? _configuration.GetValue<short>("Priority:Tasks");
                }
                set
                {
                    _Tasks = value;
                }
            }

            private short? _MessageBatch;
            public short MessageBatch
            {
                get
                {
                    return _MessageBatch ?? _configuration.GetValue<short>("Priority:MessageBatch");
                }
                set
                {
                    _MessageBatch = value;
                }
            }

            private string? _DefaultProcessChannel;
            public string DefaultProcessChannel
            {
                get
                {
                    return _DefaultProcessChannel ?? _configuration.GetValue<string>("Priority:ProcessChannel_Default");
                }
                set
                {
                    _DefaultProcessChannel = value;
                }
            }
        }

        public class BackoffPolicyClass
        {
            public EmptyIterationClass EmptyIteration { get; set; } = new();

            public ProcessChannelFullClass ProcessChannelFull { get; set; } = new();

            public class EmptyIterationClass
            {
                private int? _InitialSleepTime;
                public int InitialSleepTime
                {
                    get
                    {
                        return _InitialSleepTime ?? _configuration.GetValue<int>("BackoffPolicy:EmptyIteration:InitialSleepTime");
                    }
                    set
                    {
                        _InitialSleepTime = value;
                    }
                }

                private double? _BackoffFactor;
                public double BackoffFactor
                {
                    get
                    {
                        return _BackoffFactor ?? _configuration.GetValue<double>("BackoffPolicy:EmptyIteration:BackoffFactor");
                    }
                    set
                    {
                        _BackoffFactor = value;
                    }
                }

                private int? _MaxSleepTime;
                public int MaxSleepTime
                {
                    get
                    {
                        return _MaxSleepTime ?? _configuration.GetValue<int>("BackoffPolicy:EmptyIteration:MaxSleepTime");
                    }
                    set
                    {
                        _MaxSleepTime = value;
                    }
                }
            }

            public class ProcessChannelFullClass
            {
                private short? _MaxSize;
                public short MaxSize
                {
                    get
                    {
                        return _MaxSize ?? _configuration.GetValue<short>("BackoffPolicy:ProcessChannelFull:MaxSize");
                    }
                    set
                    {
                        _MaxSize = value;
                    }
                }

                private short? _PriorityStepSize;
                public short PriorityStepSize
                {
                    get
                    {
                        return _PriorityStepSize ?? _configuration.GetValue<short>("BackoffPolicy:ProcessChannelFull:PriorityStepSize");
                    }
                    set
                    {
                        _PriorityStepSize = value;
                    }
                }

                private int? _InitialSleepTime;
                public int InitialSleepTime
                {
                    get
                    {
                        return _InitialSleepTime ?? _configuration.GetValue<int>("BackoffPolicy:ProcessChannelFull:InitialSleepTime");
                    }
                    set
                    {
                        _InitialSleepTime = value;
                    }
                }

                private double? _PriorityFactor;
                public double PriorityFactor
                {
                    get
                    {
                        return _PriorityFactor ?? _configuration.GetValue<double>("BackoffPolicy:ProcessChannelFull:PriorityFactor");
                    }
                    set
                    {
                        _PriorityFactor = value;
                    }
                }

                private double? _TryFactor;
                public double TryFactor
                {
                    get
                    {
                        return _TryFactor ?? _configuration.GetValue<double>("BackoffPolicy:ProcessChannelFull:TryFactor");
                    }
                    set
                    {
                        _TryFactor = value;
                    }
                }

                private int? _MaxSleepTime;
                public int MaxSleepTime
                {
                    get
                    {
                        return _MaxSleepTime ?? _configuration.GetValue<int>("BackoffPolicy:ProcessChannelFull:MaxSleepTime");
                    }
                    set
                    {
                        _MaxSleepTime = value;
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
