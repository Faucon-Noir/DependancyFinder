namespace DependencyFinder.Tool.Utils;

public class Enum
{
    /// <summary>
    /// Enum define visibility and color of console messages
    /// </summary>
    public enum UsageEnum
    {
        Error,
        Info,
        Processing,
        Complete,
        Success,
        Log
    }

    /// <summary>
    /// Enum for the type of stored procedure
    /// </summary>
    public enum SPType
    {
        /// <summary>
        /// Procédure stockée, eg: usp_CalculSyntheseResil
        /// </summary>
        StoreProcedure,
        /// <summary>
        /// Fonction style dbo.FunctionName
        /// </summary>
        Function,
        /// <summary>
        /// Select, Insert, Update, Delete
        /// </summary>
        BasicInstruction,
        /// <summary>
        /// Default
        /// </summary>
        Unkwon
    }
}
