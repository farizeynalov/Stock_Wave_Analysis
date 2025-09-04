using System; // Core system functionality import.
using System.Collections.Generic; // Import for list and dictionary data structures.
using System.ComponentModel; // Support for component attributes and event handling.
using System.Data; // Functionality for data access and manipulation.
using System.Drawing; // Graphics and visual rendering capabilities.
using System.IO; // File and stream operation utilities.
using System.Linq; // Extension methods for collection querying.
using System.Text; // Tools for string manipulation.
using System.Threading.Tasks; // Framework for asynchronous programming.
using System.Windows.Forms; // GUI components for Windows applications.

namespace Candlesticks // Container for candlestick chart-related code.
{

    /// Class that models a financial candlestick with price data points and timestamp.

    public partial class Candlestick // Candlestick class definition.
    {

        /// Opening price property with getter and setter.

        public decimal Open { get; set; } // Stores the price at period start.


        /// Highest price property with getter and setter.

        public decimal High { get; set; } // Tracks the maximum price reached.


        /// Lowest price property with getter and setter.

        public decimal Low { get; set; } // Records the minimum price reached.


        /// Closing price property with getter and setter.

        public decimal Close { get; set; } // Holds the final price at period end.


        /// Trading volume property with getter and setter.

        public ulong Volume { get; set; } // Tracks the quantity of assets traded.


        /// Timestamp property with getter and setter.

        public DateTime Date { get; set; } // Records when this candlestick occurred.


        /// Initializes a candlestick by parsing a text string of financial data.

        /// <param name="rowOfData">Text string containing price information.</param>
        /// <param name="isSpaceSeparated">Indicates whether fields are delimited by spaces (true) or commas (false).</param>
        public Candlestick(string rowOfData, bool isSpaceSeparated = false) // Constructor that converts text data into a candlestick object.
        {
            // Sets appropriate delimiters based on the data format.
            char[] separators = isSpaceSeparated ? new char[] { ' ', '"' } : new char[] { ',', ' ', '"' }; // Defines character separators.

            // Breaks the input string into its component pieces, filtering out empty segments.
            string[] subs = rowOfData.Split(separators, StringSplitOptions.RemoveEmptyEntries); // Divides input into data fields.

            // Extracts the timestamp from the initial position.
            string dateString = subs[0]; // Gets the date information.
            Date = DateTime.Parse(dateString); // Converts string to DateTime object.

            decimal temp; // Temporary storage for price conversion.

            // Extracts and validates the opening price from position 1.
            bool success = decimal.TryParse(subs[1], out temp); // Converts opening price text to decimal.
            if (success) Open = temp; // Assigns if conversion succeeded.

            // Extracts and validates the highest price from position 2.
            success = decimal.TryParse(subs[2], out temp); // Converts highest price text to decimal.
            if (success) High = temp; // Assigns if conversion succeeded.

            // Extracts and validates the lowest price from position 3.
            success = decimal.TryParse(subs[3], out temp); // Converts lowest price text to decimal.
            if (success) Low = temp; // Assigns if conversion succeeded.

            // Extracts and validates the closing price from position 4.
            success = decimal.TryParse(subs[4], out temp); // Converts closing price text to decimal.
            if (success) Close = temp; // Assigns if conversion succeeded.

            // Processes volume data if available in the input.
            if (subs.Length > 5) // Checks for volume information presence.
            {
                ulong tempVolume; // Temporary variable for volume data.
                success = ulong.TryParse(subs[5], out tempVolume); // Converts volume text to integer.
                if (success) Volume = tempVolume; // Assigns if conversion succeeded.
            }
        }
    }
}