using App.Analyzer;
using App.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Test.Test.Analyzer
{
    public class AnalyzerTest
    {
        [Fact]
        public async Task SqlAnalyzerTest()
        {
            // Arrange
            string sql = "./TestFolder/MainSql.sql";
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
