using App.Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Test.Test.Analyzer
{
    public class AnalyzerTest
    {
        public void SqlAnalyzerTest()
        {
            // Arrange
            var sql = "";
            var expected = "";

            // Act
            SqlAnalyzer sqlAnalyzer = new();
            var actual =sqlAnalyzer.AnalyzeSqlAsync(sql, false);
            var result = File.ReadAllText("../TestFolder/Main.json");

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
