namespace DependencyFinder.Tool.Modules;

public class EnumModule
{
    /// <summary>
    /// Enum for the color of the console
    /// </summary>
    public enum UsageEnum
    {
        Processing,
        Success,
        Error,
        Complete,
        Info,
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
