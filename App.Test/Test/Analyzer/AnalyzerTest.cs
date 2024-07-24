using App.Utils;

namespace App.Test.Test.Analyzer
{
    public class AnalyzerTest
    {
        [Fact]
        public async Task SqlAnalyzerTest()
        {
            // Arrange
            var expected = File.ReadLines(@"./TestFolder/Main.json");
            var options = new Options
            {
                InputPath = "./TestFolder/MainSql.sql",
                OutputPath = "./TestFolder",
                GPTReport = false,
            };

            // Act
            ProcessAnalyze analyzer = new();
            await analyzer.ProcessAnalyzeAsync(options);
            var result = File.ReadLines("./TestFolder/MainSql.json");

            // Assert
            Assert.Equal(expected.FirstOrDefault(), result.FirstOrDefault());
        }
    }
}
