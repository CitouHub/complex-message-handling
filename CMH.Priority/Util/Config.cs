using CMH.Common.Variable;

namespace CMH.Priority.Util
{
    public class Config
    {
        private static IConfiguration? _configuration;

        public QueueCacheClass QueueCache { get; set; } = new();

        public PriorityClass Priority { get; set; } = new();        

        public BackoffPolicyClass BackoffPolicy { get; set; } = new();

        public Config() { }

        public Config(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Reset()
        {
            QueueCache = new();
            Priority = new();
            BackoffPolicy = new();
        }

        public class QueueCacheClass
        {
            public void Reset()
            {
                _RefreshInterval = null;
            }

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
            public void Reset()
            {
                _Tasks = null;
                _MessageBatch = null;
                _DefaultProcessChannel = null;
            }

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

            private ProcessChannel? _DefaultProcessChannel;
            public ProcessChannel DefaultProcessChannel
            {
                get
                {
                    return _DefaultProcessChannel ?? Enum.Parse<ProcessChannel>(_configuration.GetValue<string>("Priority:DefaultProcessChannel"));
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

            public void Reset()
            {
                EmptyIteration.Reset();
                ProcessChannelFull.Reset();
            }

            public class EmptyIterationClass
            {
                public void Reset()
                {
                    _InitialSleepTime = null;
                    _BackoffFactor = null;
                    _MaxSleepTime = null;
                }

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
                public void Reset()
                {
                    _MaxSize = null;
                    _PriorityStepSize = null;
                    _InitialSleepTime = null;
                    _PriorityFactor = null;
                    _TryFactor = null;
                    _MaxSleepTime = null;
                }

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
    }
}
