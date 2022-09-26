using CMH.Priority.Util;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace CMH.Test.Helper
{
    public static class ConfigHelper
    {
        public static Config GetConfig()
        {
            var configuration = Substitute.For<IConfiguration>();
            configuration.GetValue<short>(Arg.Any<string>()).Returns((short)0);
            configuration.GetValue<int>(Arg.Any<string>()).Returns(0);
            configuration.GetValue<string>(Arg.Any<string>()).Returns("");
            configuration.GetValue<double>(Arg.Any<string>()).Returns(0);

            return new Config(configuration);
        }
    }
}
