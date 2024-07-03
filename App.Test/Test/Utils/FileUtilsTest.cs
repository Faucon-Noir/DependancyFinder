using App.Utils;

namespace App.Test.Test.Utils;

public class FileUtilsTest
{
    [Fact]
    public void TestFindFileInFilePath()
    {
        // Arrange
        string name = "fileToFind";
        string filePath = "./TestFolder/";
        string expected = $"{filePath}{name}.sql";
        string invalidFilePath = "./TestFolder/demo/";
        string invalidDirectoryPath = "./demo/";

        // Act
        var resultFound = FileUtils.FindFileInFilePath(filePath, name);
        var resultNotFound = FileUtils.FindFileInFilePath(invalidFilePath, name);
        var resultInvalidDirectory = FileUtils.FindFileInFilePath(invalidDirectoryPath, name);

        // Assert
        Assert.Equal(expected, resultFound);
        Assert.Equal("File not found", resultNotFound);
        Assert.Equal("Directory not found", resultInvalidDirectory);

    }

    [Fact]
    public void TestSplitFilePath() {         // Arrange
        string filePath = "./TestFolder/fileToFind.sql";
        string expectedDirectory = ".\\TestFolder";
        string expectedFileName = "fileToFind";

        // Act
        var result = FileUtils.SplitFilePath(filePath);

        // Assert
        Assert.Equal(expectedDirectory, result.Item1);
        Assert.Equal(expectedFileName, result.Item2);
    }

    [Fact]
    public void TestFormatFileName()
    {
        // Arrange
        string filePath = "[fileToFind]";
        string expected = "fileToFind";

        // Act
        var result = FileUtils.FormatFileName(filePath);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestIsValidDirectory()
    {
        // Arrange
        var validDirectoryPath = "./TestFolder";
        var invalidDirectoryPath = "./TestFolder/demo.json";

        // Act
        var resultForValidDirectory = FileUtils.IsValidDirectory(validDirectoryPath);
        var resultForInvalidDirectory = FileUtils.IsValidDirectory(invalidDirectoryPath);

        // Assert
        Assert.True(resultForValidDirectory);
        // All path should be converted to directory path
        Assert.True(resultForInvalidDirectory);
    }

    [Theory]
    // Arrange
    [InlineData(@"./TestFolder/fileToFind.sql", true)]
    [InlineData(@"./TestFolder/", true)]
    [InlineData(@"./demo/", false)]
    [InlineData(@"./TestFolder/invalidFile.json", false)]
    [InlineData(@"./TestFolder/demo/", false)]
    [InlineData(@"./TestFolder/invalidFile.txt", false)]
    public void TestIsValidPath(string input, bool expected)
    {
        // Act
        var result = FileUtils.IsValidPath(input);
        // Assert
        Assert.Equal(result, expected);
    }
}
