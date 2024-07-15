using App.Utils;
using static App.Utils.EnumUtils;

namespace App.Test.Test.Utils
{
    public class UtilityTest
    {
        [Theory]
        [InlineData(UsageEnum.Success, "Success message", "Success message")]
        [InlineData(UsageEnum.Error, "Error message", "Error message")]
        [InlineData(UsageEnum.Info, "Info message", "Info message")]
        [InlineData(UsageEnum.Processing, "Processing message", "Processing message")]
        [InlineData(UsageEnum.Complete, "Complete message", "Complete message")]
        [InlineData(UsageEnum.Log, "Log message", "Log message")]
        public void CustomWriteLineTest(UsageEnum usage, string message, string expectedOutput)
        {
            // Arrange
            var originalOutput = Console.Out;
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Set the environment variable
            Environment.SetEnvironmentVariable("Verbose", usage.ToString());

            // Act
            Utility.CustomWriteLine(usage, message);

            // Reset the console output
            Console.SetOut(originalOutput);

            // Assert
            var output = stringWriter.ToString().Trim();
            Assert.Equal(expectedOutput, output);
        }
    }
}