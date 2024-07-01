using DependencyFinder.App.Utils;

namespace DependencyFinder.Test.Test
{
    public class UtilityTest
    {
        [Fact]
        public void TestIsValidDirectory()
        {
            // Arrange
            var validDirectoryPath = "./TestFolder";
            var invalidDirectoryPath = "./TestFolder/demo.json";

            // Act
            var resultForValidDirectory = Utility.IsValidDirectory(validDirectoryPath);
            var resultForInvalidDirectory = Utility.IsValidDirectory(invalidDirectoryPath);

            // Assert
            Assert.True(resultForValidDirectory);
            // On attends que tout les chemins fournis y compris vers un fichier soit transformer en chemin de dossier
            Assert.True(resultForInvalidDirectory);
        }
    }
}