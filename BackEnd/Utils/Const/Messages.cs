using System.Text.RegularExpressions;

namespace BackEnd.Utils.Const;

public partial class Messages
{
    /// <summary>
    /// Returns the message associated with the message ID.
    /// </summary>
    /// <param name="messageId">Message Id</param>
    /// <param name="args">args</param>
    public static string GetMessage(string messageId, params string[] args)
    {
        //If there is no message that matches the CSV, the following will be returned.
        var message = "No matching message.";

        // var csvPath = System.AppDomain.CurrentDomain.BaseDirectory + $"Settings/ConstantCSV/MessageId.csv";
        var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "Settings/ConstanstCSV/MessageId.csv");

        if (!File.Exists(csvPath)) return $"{csvPath} not found.";

        var reader = new StreamReader(File.OpenRead(csvPath));
        int row = 0;
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            //Skip the first line
            if (row == 0) { row++; continue; }

            string[] values = line.Split(',');
            if (values[1] == messageId)
            {
                message = values[2];
                //Replace the specified part even if {0} etc. are not set
                for (int i = 0; i < args.Length; i++)
                    message = message.Replace($"{{{i}}}", args[i]);

                //Remove the remaining {0} etc.
                message = Regex.Replace(message, @"\{[0-9]+\}", "");

                //Convert \\n to \n
                message = message.Replace("\\n", "\n");

                break;
            }
        }
        return message;
    }
}