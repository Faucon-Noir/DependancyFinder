using DependencyFinder.Tool.Modules;

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
            var resultForValidDirectory = UtilityModule.IsValidDirectory(validDirectoryPath);
            var resultForInvalidDirectory = UtilityModule.IsValidDirectory(invalidDirectoryPath);

            // Assert
            Assert.True(resultForValidDirectory);
            // On attends que tout les chemins donn� y compris vers un fichier soit transmofrm� en chemin de dossier
            Assert.True(resultForInvalidDirectory);
        }
    }
}