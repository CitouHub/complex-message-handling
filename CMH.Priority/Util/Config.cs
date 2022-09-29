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
            private int? _RefreshInterval;
            public int RefreshInterval
            {
                get
                {
                    return _configuration != null ? (_RefreshInterval ?? _configuration.GetValue<int>("QueueCache:RefreshInterval")) : 0;
                }
                set
                {
                    _RefreshInterval = value;
                }
            }
        }

        public class PriorityClass
        {
            private List<string>? _Queues;
            public List<string> Queues
            {
                get
                {
                    return _configuration != null ? (_Queues ?? _configuration.GetSection("Priority:Queues").Get<List<string>>()) : new List<string>();
                }
                set
                {
                    _Queues = value;
                }
            }

            private short? _ReadTasks;
            public short ReadTasks
            {
                get
                {
                    return _configuration != null ? (_ReadTasks ?? _configuration.GetValue<short>("Priority:ReadTasks")) : (short)1;
                }
                set
                {
                    _ReadTasks = value;
                }
            }

            private short? _WriteTasks;
            public short WriteTasks
            {
                get
                {
                    return _configuration != null ? (_WriteTasks ?? _configuration.GetValue<short>("Priority:WriteTasks")) : (short)1;
                }
                set
                {
                    _WriteTasks = value;
                }
            }

            private short? _MessageBatch;
            public short MessageBatch
            {
                get
                {
                    return _configuration != null ? (_MessageBatch ?? _configuration.GetValue<short>("Priority:MessageBatch")) : (short)0;
                }
                set
                {
                    _MessageBatch = value;
                }
            }

            private short? _MessageFetchTimeOut;
            public short MessageFetchTimeOut
            {
                get
                {
                    return _configuration != null ? (_MessageFetchTimeOut ?? _configuration.GetValue<short>("Priority:MessageFetchTimeOut")) : (short)0;
                }
                set
                {
                    _MessageFetchTimeOut = value;
                }
            }


            private ProcessChannel? _DefaultProcessChannel;
            public ProcessChannel DefaultProcessChannel
            {
                get
                {
                    return _configuration != null ? (_DefaultProcessChannel ?? Enum.Parse<ProcessChannel>(_configuration.GetValue<string>("Priority:DefaultProcessChannel"))) : ProcessChannel.Default;
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
                        return _configuration != null ? (_InitialSleepTime ?? _configuration.GetValue<int>("BackoffPolicy:EmptyIteration:InitialSleepTime")) : 0;
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
                        return _configuration != null ? (_BackoffFactor ?? _configuration.GetValue<double>("BackoffPolicy:EmptyIteration:BackoffFactor")) : 0;
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
                        return _configuration != null ? (_MaxSleepTime ?? _configuration.GetValue<int>("BackoffPolicy:EmptyIteration:MaxSleepTime")) : 0;
                    }
                    set
                    {
                        _MaxSleepTime = value;
                    }
                }
            }

            public class ProcessChannelFullClass
            {
                private short? _PrioritySlots;
                public short PrioritySlots
                {
                    get
                    {
                        return _configuration != null ? (_PrioritySlots ?? _configuration.GetValue<short>("BackoffPolicy:ProcessChannelFull:PrioritySlots")) : (short)0;
                    }
                    set
                    {
                        _PrioritySlots = value;
                    }
                }

                private int? _InitialSleepTime;
                public int InitialSleepTime
                {
                    get
                    {
                        return _configuration != null ? (_InitialSleepTime ?? _configuration.GetValue<int>("BackoffPolicy:ProcessChannelFull:InitialSleepTime")) : 0;
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
                        return _configuration != null ? (_PriorityFactor ?? _configuration.GetValue<double>("BackoffPolicy:ProcessChannelFull:PriorityFactor")) : 0;
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
                        return _configuration != null ? (_TryFactor ?? _configuration.GetValue<double>("BackoffPolicy:ProcessChannelFull:TryFactor")) : 0;
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
                        return _configuration != null ? (_MaxSleepTime ?? _configuration.GetValue<int>("BackoffPolicy:ProcessChannelFull:MaxSleepTime")) : 0;
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
