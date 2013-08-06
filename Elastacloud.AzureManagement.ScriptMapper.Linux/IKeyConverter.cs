namespace Elastacloud.AzureManagement.ScriptMapper.Linux
{
    /// <summary>
    /// Converts an SSH key between formats
    /// </summary>
    public interface IKeyConverter
    {
        /// <summary>
        /// Converts an SSH key between .pem and opensshv2 formats
        /// </summary>
        /// <returns>conversion succeeded</returns>
        bool Convert();
        /// <summary>
        /// Gets the new key filepath
        /// </summary>
        string KeyFilePath { get; }
    }
}