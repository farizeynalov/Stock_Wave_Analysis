using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Candlesticks;

namespace Project1_stock
{
    /// <summary>
    /// Main form class for the Candlesticks application that provides stock chart visualization 
    /// and technical analysis capabilities including peak/valley detection and wave analysis.
    /// </summary>
    public partial class Form_Candlesticks : Form
    {
        // Class-level variable to store all candlestick data read from file
        List<Candlestick> candlesticks = new List<Candlestick>(1024);

        // Class-level variable for filtered candlesticks that will be displayed on the chart
        BindingList<Candlestick> boundCandlesticks = null;

        /// <summary>
        /// Constructor for Form_Candlesticks that initializes the form and its components
        /// </summary>
        public Form_Candlesticks()
        {
            InitializeComponent(); // Initialize all form components defined in the designer
        }

        /// <summary>
        /// Event handler triggered when a file is selected in the file dialog
        /// Performs the complete initialization of the chart with selected data
        /// </summary>
        /// <param name="sender">The object that triggered the event</param>
        /// <param name="e">Event arguments containing file selection info</param>
        private void fileHandler(object sender, CancelEventArgs e)
        {
            readCandlesticksFromFile(); // Load candlestick data from the selected file
            filterCandlesticks(); // Apply date range filter to the loaded data
            normalizechart(); // Adjust Y-axis scale to fit the data range
            displayCandlesticks(); // Display the filtered candlestick data on the chart
            findAndDisplayPeaksAndValleys(); // Identify and mark the peaks and valleys
            findAndDisplayWaves(); // Identify and display wave patterns between peaks and valleys
        }

        // Flag to track whether the user is currently dragging on the chart
        private bool _isDragging = false;

        /// <summary>
        /// Removes all temporary drawing elements used during rubber-banding or wave visualization
        /// </summary>
        private void ClearTemporaryRubberband()
        {
            // Remove any temporary series (lines, bands) by name pattern
            foreach (var s in chart_Candlesticks.Series
                .Where(s => s.Name.StartsWith("Temp_") || s.Name.StartsWith("Initial_Line_"))
                .ToList())
            {
                chart_Candlesticks.Series.Remove(s); // Remove each matching series from the chart
            }

            // Remove any temporary annotations (labels, markers) by name pattern
            foreach (var a in chart_Candlesticks.Annotations
                .Where(a => a.Name.StartsWith("Temp_") || a.Name.StartsWith("WaveBox_"))
                .ToList())
            {
                chart_Candlesticks.Annotations.Remove(a); // Remove each matching annotation from the chart
            }
        }

        // Class-level variables to track drag operations and simulation state
        private int _dragStartIndex, _dragEndIndex; // Indices of drag start and end points
        private int _simulationStep; // Current simulation step counter
        private decimal _simulationPrice; // Price level at current simulation step
        private decimal _minPrice; // Minimum price for simulation range
        private decimal _maxPrice; // Maximum price for simulation range
        private decimal _stepSize; // Step size for price increment during simulation

        /// <summary>
        /// Reads candlestick data from a specified file, supporting both comma and space-delimited formats
        /// </summary>
        /// <param name="filename">Path to the data file to read</param>
        /// <returns>List of Candlestick objects created from the file data</returns>
        private List<Candlestick> readCandlesticksFromFile(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                candlesticks.Clear(); // Clear any existing data in the candlesticks list
                string line = sr.ReadLine(); // Read the first line (typically header)
                bool isSpaceSeparated = (line != null && !line.Contains(",")); // Determine file format (space or comma-delimited)

                if (line != null)
                {
                    while ((line = sr.ReadLine()) != null) // Read each subsequent line in the file
                    {
                        if (string.IsNullOrEmpty(line)) continue; // Skip empty lines
                        try
                        {
                            Candlestick cs = new Candlestick(line, isSpaceSeparated); // Parse line into a candlestick object
                            candlesticks.Add(cs); // Add the new candlestick to the list
                        }
                        catch { } // Silently ignore malformed lines that can't be parsed
                    }

                    if (candlesticks.Count > 0)
                        return candlesticks; // Return the populated list if data was found
                }

                Text = "badfile"; // Set form title to indicate loading failure
                return candlesticks; // Return the empty list
            }
        }

        /// <summary>
        /// Helper method that reads candlestick data from the file selected in the open file dialog
        /// </summary>
        private void readCandlesticksFromFile()
        {
            string filename = openFileDialog_TickerChooser.FileName; // Get the selected filename from dialog
            this.Text = filename; // Set the form title to the filename for reference
            readCandlesticksFromFile(filename); // Call the file reading method with the selected file
        }

        /// <summary>
        /// Filters a list of candlesticks to only include those within a specified date range
        /// </summary>
        /// <param name="unfilteredList">Original list of candlesticks</param>
        /// <param name="startDate">Start date for filtering (inclusive)</param>
        /// <param name="endDate">End date for filtering (inclusive)</param>
        /// <returns>Filtered list of candlesticks within the date range</returns>
        private List<Candlestick> filterCandlesticks(List<Candlestick> unfilteredList, DateTime startDate, DateTime endDate)
        {
            // Use LINQ to filter candlesticks that fall within the specified date range
            return unfilteredList.Where(c => c.Date >= startDate && c.Date <= endDate).ToList();
        }

        /// <summary>
        /// Applies date filtering to the current candlestick list using date values from UI controls
        /// </summary>
        private void filterCandlesticks()
        {
            DateTime startDate = datetimepicker_Start_Date.Value; // Get start date from the date picker control
            DateTime endDate = datetimepicker_End_Date.Value; // Get end date from the date picker control

            // Apply the filter and create a bindable list for the chart
            boundCandlesticks = new BindingList<Candlestick>(
                filterCandlesticks(candlesticks, startDate, endDate));
        }

        /// <summary>
        /// Finds the minimum and maximum price values in the visible candlesticks
        /// </summary>
        /// <param name="min">Current minimum value to compare against</param>
        /// <param name="max">Current maximum value to compare against</param>
        /// <returns>Tuple containing updated minimum and maximum values</returns>
        private Tuple<decimal, decimal> normalizechart(decimal min, decimal max)
        {
            foreach (var cs in boundCandlesticks)
            {
                if (cs.Low < min) min = cs.Low; // Update minimum if current candlestick's low is lower
                if (cs.High > max) max = cs.High; // Update maximum if current candlestick's high is higher
            }

            return Tuple.Create(min, max); // Return the updated min and max as a tuple
        }

        /// <summary>
        /// Adjusts the chart's Y-axis range to fit the data with a small buffer margin
        /// </summary>
        private void normalizechart()
        {
            double buffer = 0.02; // Add 2% buffer space above and below the data range
            decimal min = decimal.MaxValue; // Initialize min to highest possible value
            decimal max = decimal.MinValue; // Initialize max to lowest possible value

            (min, max) = normalizechart(min, max); // Find actual min/max values in the data

            // Set Y-axis limits with buffer margins
            chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisY.Minimum = (double)Math.Floor(min) * (1 - buffer);
            chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisY.Maximum = (double)Math.Ceiling(max) * (1 + buffer);
        }

        /// <summary>
        /// Sets up the data binding properties for the candlestick chart
        /// </summary>
        /// <param name="boundCandlesticks">The filtered candlestick data to bind</param>
        /// <returns>The same candlestick data for method chaining</returns>
        private BindingList<Candlestick> displayCandlesticks(BindingList<Candlestick> boundCandlesticks)
        {
            chart_Candlesticks.Series["Series_OHLC"].XValueMember = "Date"; // Set X-axis to use Date property
            chart_Candlesticks.Series["Series_OHLC"].YValueMembers = "High,Low,Open,Close"; // Set Y values to use OHLC properties
            return boundCandlesticks; // Return the bound data for chaining
        }

        /// <summary>
        /// Configures and displays the candlestick chart with the filtered data
        /// </summary>
        private void displayCandlesticks()
        {
            chart_Candlesticks.DataSource = displayCandlesticks(boundCandlesticks); // Set the data source for the chart
            chart_Candlesticks.Series["Series_OHLC"].IsXValueIndexed = false; // Use actual date values, not indices
            chart_Candlesticks.Series["Series_OHLC"].XValueType = ChartValueType.DateTime; // Specify X-axis as DateTime type
            chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisX.IsMarginVisible = false; // Remove extra margins on X-axis
            chart_Candlesticks.DataBind(); // Apply the data binding to render the chart
        }

        /// <summary>
        /// Event handler for the Load Stock button click that allows selection of multiple files
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">Event arguments</param>
        private void button_Stock_Load(object sender, EventArgs e)
        {
            openFileDialog_TickerChooser.Multiselect = true; // Enable multi-file selection

            if (openFileDialog_TickerChooser.ShowDialog() == DialogResult.OK) // Show file dialog and check if user clicked OK
            {
                foreach (var filename in openFileDialog_TickerChooser.FileNames) // Process each selected file
                {
                    var form = new Form_Candlesticks(); // Create a new form instance for each file
                    form.LoadStock(filename); // Load the selected file into the new form
                    form.Show(); // Display the new form
                }
            }
        }

        /// <summary>
        /// Event handler to update the display when date range changes
        /// </summary>
        /// <param name="sender">The control that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void button_update_date(object sender, EventArgs e)
        {
            clearWaveDrawings(); // Clear any existing wave visualizations
            filterCandlesticks(); // Apply new date filter
            normalizechart(); // Adjust chart scale
            displayCandlesticks(); // Update chart display
            findAndDisplayPeaksAndValleys(); // Recalculate and show peaks/valleys
            findAndDisplayWaves(); // Recalculate and show waves
        }

        /// <summary>
        /// Event handler for Refresh Data button click
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">Event arguments</param>
        private void button_Refresh_Data_Click(object sender, EventArgs e)
        {
            button_update_date(sender, e); // Reuse the date update logic
            clearWaveDrawings(); // Clear any wave-related annotations
        }

        /// <summary>
        /// Identifies peaks and valleys in the price data and marks them on the chart
        /// </summary>
        private void findAndDisplayPeaksAndValleys()
        {
            chart_Candlesticks.Annotations.Clear(); // Clear existing annotations
            int margin = scrollbar_Margin.Value; // Get the margin value from scroll bar control

            // Examine each candlestick (except those at edges where we can't check surrounding margin)
            for (int i = margin; i < boundCandlesticks.Count - margin; i++)
            {
                bool isPeak = true, isValley = true; // Initialize peak/valley flags

                // Compare with each candlestick within the margin range
                for (int j = i - margin; j <= i + margin; j++)
                {
                    if (j == i) continue; // Skip comparison with self

                    // Not a peak if any neighbor has higher high
                    if (boundCandlesticks[i].High <= boundCandlesticks[j].High) isPeak = false;

                    // Not a valley if any neighbor has lower low
                    if (boundCandlesticks[i].Low >= boundCandlesticks[j].Low) isValley = false;
                }

                if (isPeak) // If a peak was found
                {
                    TextAnnotation peakMark = new TextAnnotation(); // Create a text annotation for the peak
                    peakMark.Text = "P"; // Label with "P" for Peak
                    peakMark.ForeColor = Color.Red; // Use red color for peaks
                    peakMark.AnchorDataPoint = chart_Candlesticks.Series["Series_OHLC"].Points[i]; // Attach to data point
                    chart_Candlesticks.Annotations.Add(peakMark); // Add to chart annotations
                }

                if (isValley) // If a valley was found
                {
                    TextAnnotation valleyMark = new TextAnnotation(); // Create a text annotation for the valley
                    valleyMark.Text = "V"; // Label with "V" for Valley
                    valleyMark.ForeColor = Color.Green; // Use green color for valleys
                    valleyMark.AnchorDataPoint = chart_Candlesticks.Series["Series_OHLC"].Points[i]; // Attach to data point
                    chart_Candlesticks.Annotations.Add(valleyMark); // Add to chart annotations
                }
            }
        }

        /// <summary>
        /// Removes all wave-related visual elements from the chart
        /// </summary>
        private void clearWaveDrawings()
        {
            // Find all wave line series
            var waveSeries = chart_Candlesticks.Series
                .Where(s => s.Name.StartsWith("WaveLine_"))
                .ToList();

            // Remove each wave line series
            foreach (var series in waveSeries)
                chart_Candlesticks.Series.Remove(series);

            // Find all wave box annotations
            var waveBoxes = chart_Candlesticks.Annotations
                .OfType<RectangleAnnotation>()
                .Where(a => a.Name.StartsWith("WaveBox_"))
                .ToList();

            // Remove each wave box annotation
            foreach (var box in waveBoxes)
                chart_Candlesticks.Annotations.Remove(box);
        }

        /// <summary>
        /// Identifies and lists potential wave patterns (up and down) based on peaks and valleys
        /// </summary>
        private void findAndDisplayWaves()
        {
            // Clear existing wave lists
            combobox_UpWaves.Items.Clear();
            combobox_DownWaves.Items.Clear();

            int margin = scrollbar_Margin.Value; // Get current margin setting

            // Lists to store identified peaks and valleys
            List<(int index, DateTime date, decimal price)> peaks = new List<(int, DateTime, decimal)>();
            List<(int index, DateTime date, decimal price)> valleys = new List<(int, DateTime, decimal)>();

            // Find all peaks and valleys in the data
            for (int i = margin; i < boundCandlesticks.Count - margin; i++)
            {
                if (IsPeak(i)) // If current point is a peak
                {
                    peaks.Add((i, boundCandlesticks[i].Date, boundCandlesticks[i].High)); // Store peak info
                }
                if (IsValley(i)) // If current point is a valley
                {
                    valleys.Add((i, boundCandlesticks[i].Date, boundCandlesticks[i].Low)); // Store valley info
                }
            }

            // Create up waves (valley to peak sequences)
            for (int i = 0; i < valleys.Count; i++)
            {
                for (int j = 0; j < peaks.Count; j++)
                {
                    if (peaks[j].index > valleys[i].index)  // Peak comes after valley (up wave)
                    {
                        string waveDescription = $"{valleys[i].date.ToShortDateString()} - {peaks[j].date.ToShortDateString()}";
                        combobox_UpWaves.Items.Add(new WaveInfo
                        {
                            Description = waveDescription,
                            StartIndex = valleys[i].index,
                            EndIndex = peaks[j].index,
                            IsUpWave = true
                        });
                    }
                }
            }

            // Create down waves (peak to valley sequences)
            for (int i = 0; i < peaks.Count; i++)
            {
                for (int j = 0; j < valleys.Count; j++)
                {
                    if (valleys[j].index > peaks[i].index)  // Valley comes after peak (down wave)
                    {
                        string waveDescription = $"{peaks[i].date.ToShortDateString()} - {valleys[j].date.ToShortDateString()}";
                        combobox_DownWaves.Items.Add(new WaveInfo
                        {
                            Description = waveDescription,
                            StartIndex = peaks[i].index,
                            EndIndex = valleys[j].index,
                            IsUpWave = false
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Class to store information about a wave pattern
        /// </summary>
        public class WaveInfo
        {
            public string Description { get; set; } // Display text for the wave (date range)
            public int StartIndex { get; set; } // Index of start point in candlesticks list
            public int EndIndex { get; set; } // Index of end point in candlesticks list
            public bool IsUpWave { get; set; } // Flag indicating whether this is an up wave (true) or down wave (false)

            public override string ToString()
            {
                return Description; // Use description as string representation for display
            }
        }

        /// <summary>
        /// Event handler for form load (not used, but created by designer)
        /// </summary>
        private void Form_Candlesticks_Load(object sender, EventArgs e) { }

        /// <summary>
        /// Event handler for label click (not used, but created by designer)
        /// </summary>
        private void label_ChartControls_Click(object sender, EventArgs e) { }

        /// <summary>
        /// Event handler for when user selects an up wave from the combo box
        /// </summary>
        /// <param name="sender">The combobox that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void combobox_UpWaves_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combobox_UpWaves.SelectedItem is WaveInfo waveInfo) // Check if valid selection exists
            {
                ClearTemporaryRubberband(); // Clear any existing visual elements
                _dragStartIndex = waveInfo.StartIndex; // Set start index to selected wave start
                _dragEndIndex = waveInfo.EndIndex; // Set end index to selected wave end

                // Draw a green box representing the up wave
                DrawWaveBox(_dragStartIndex, _dragEndIndex, true);

                // Draw a diagonal line connecting start and end points
                DrawInitialDiagonal(_dragStartIndex, _dragEndIndex);

                // Enable simulation controls
                button_Start_Stop.Enabled = true;
                button_Increase_Step.Enabled = false;
                button_Decrease_Step.Enabled = false;
            }
        }

        /// <summary>
        /// Draws a rectangle highlighting the selected wave on the chart
        /// </summary>
        /// <param name="start">Index of wave start point</param>
        /// <param name="end">Index of wave end point</param>
        /// <param name="isUpWave">Flag indicating if this is an up wave (green) or down wave (red)</param>
        private void DrawWaveBox(int start, int end, bool isUpWave)
        {
            // Ensure indices are in correct order
            int i0 = Math.Min(start, end), i1 = Math.Max(start, end);

            // Get the slice of candlesticks for this wave
            var slice = boundCandlesticks.Skip(i0).Take(i1 - i0 + 1).ToList();

            double low, high;

            if (isUpWave)
            {
                // For up waves, use starting candlestick's low and ending candlestick's high
                low = (double)boundCandlesticks[start].Low;
                high = (double)boundCandlesticks[end].High;
            }
            else
            {
                // For down waves, use the slice's min low and max high
                low = slice.Min(c => (double)c.Low);
                high = slice.Max(c => (double)c.High);
            }

            // Convert indices to dates for X-axis positioning
            DateTime startDate = boundCandlesticks[i0].Date;
            DateTime endDate = boundCandlesticks[i1].Date;

            // Remove any existing wave box
            chart_Candlesticks.Annotations
                .OfType<RectangleAnnotation>()
                .Where(a => a.Name.StartsWith("WaveBox_"))
                .ToList()
                .ForEach(a => chart_Candlesticks.Annotations.Remove(a));

            // Create new rectangle annotation
            var box = new RectangleAnnotation
            {
                Name = $"WaveBox_{start}_{end}", // Unique name for the box
                AxisX = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisX, // X-axis to use
                AxisY = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisY, // Y-axis to use
                X = startDate.ToOADate(), // Left edge (X position)
                Y = low, // Bottom edge (Y position)
                Width = endDate.ToOADate() - startDate.ToOADate(), // Width (X span)
                Height = high - low, // Height (Y span)

                // Set colors based on wave type (green for up, red for down)
                BackColor = isUpWave
                               ? Color.FromArgb(40, Color.LightGreen) // Semi-transparent green fill
                               : Color.FromArgb(40, Color.LightPink), // Semi-transparent red fill
                LineColor = isUpWave
                               ? Color.FromArgb(120, Color.Green) // Semi-transparent green border
                               : Color.FromArgb(120, Color.Red), // Semi-transparent red border

                LineWidth = 2, // Border thickness
                ClipToChartArea = "ChartArea_OHLC", // Restrict to chart area
                IsSizeAlwaysRelative = false // Use absolute sizing, not relative
            };

            chart_Candlesticks.Annotations.Add(box); // Add box to chart

            // Draw diagonal line inside the box
            DrawInitialDiagonal(start, end);
        }

        /// <summary>
        /// Event handler for when user selects a down wave from the combo box
        /// </summary>
        /// <param name="sender">The combobox that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void combobox_DownWaves_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combobox_DownWaves.SelectedItem is WaveInfo waveInfo) // Check if valid selection exists
            {
                ClearTemporaryRubberband(); // Clear any existing visual elements
                _dragStartIndex = waveInfo.StartIndex; // Set start index to selected wave start
                _dragEndIndex = waveInfo.EndIndex; // Set end index to selected wave end

                // Draw a pink box representing the down wave
                DrawWaveBox(_dragStartIndex, _dragEndIndex, false);

                // Draw a diagonal line connecting start and end points
                DrawInitialDiagonal(_dragStartIndex, _dragEndIndex);

                // Enable simulation controls
                button_Start_Stop.Enabled = true;
                button_Increase_Step.Enabled = false;
                button_Decrease_Step.Enabled = false;
            }
        }

        /// <summary>
        /// Draws a rectangle on the chart to highlight a selected wave
        /// </summary>
        /// <param name="selectedText">Text containing date range of the wave</param>
        /// <param name="color">Color to use for the rectangle</param>
        private void drawWaveRectangle(string selectedText, Color color)
        {
            // Split the text into start and end dates
            string[] parts = selectedText.Split(new string[] { " - " }, StringSplitOptions.None);
            if (parts.Length != 2) return; // Exit if format is invalid

            // Parse dates from text
            DateTime start = DateTime.Parse(parts[0]);
            DateTime end = DateTime.Parse(parts[1]);

            // Find corresponding indices in the data
            int startIndex = boundCandlesticks.ToList().FindIndex(c => c.Date == start);
            int endIndex = boundCandlesticks.ToList().FindIndex(c => c.Date == end);

            if (startIndex == -1 || endIndex == -1) return; // Exit if dates not found

            // Find min/max price in the wave range
            double minY = (double)boundCandlesticks.Skip(startIndex).Take(endIndex - startIndex + 1).Min(c => c.Low);
            double maxY = (double)boundCandlesticks.Skip(startIndex).Take(endIndex - startIndex + 1).Max(c => c.High);

            // Create rectangle annotation
            RectangleAnnotation box = new RectangleAnnotation();
            box.AxisX = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisX; // X-axis to use
            box.AxisY = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisY; // Y-axis to use
            box.X = boundCandlesticks[startIndex].Date.ToOADate(); // Left edge (X position)
            box.Y = maxY; // Top edge (Y position) - note this differs from DrawWaveBox
            box.Width = boundCandlesticks[endIndex].Date.ToOADate() - boundCandlesticks[startIndex].Date.ToOADate(); // Width
            box.Height = maxY - minY; // Height
            box.BackColor = color; // Fill color
            box.LineColor = Color.Black; // Border color
            box.LineWidth = 3; // Border thickness
            box.LineDashStyle = ChartDashStyle.Solid; // Solid border style
            box.ClipToChartArea = "ChartArea_OHLC"; // Restrict to chart area
            box.IsSizeAlwaysRelative = false; // Use absolute sizing, not relative

            // Create unique name for the box
            box.Name = $"WaveBox_{start:yyyyMMdd}_{end:yyyyMMdd}";
            chart_Candlesticks.Annotations.Add(box); // Add to chart

            // Also draw a connecting line
            drawWaveLine(start, end, color == Color.LightGreen ? Color.DarkGreen : Color.Red);
        }

        /// <summary>
        /// Loads stock data from a file and initializes the chart
        /// </summary>
        /// <param name="filename">Path to the stock data file</param>
        public void LoadStock(string filename)
        {
            this.Text = filename; // Set window title to filename
            readCandlesticksFromFile(filename); // Load data from file

            // Reset interaction state
            _dragStartIndex = -1;
            _dragEndIndex = -1;
            _isDragging = false;

            // Initialize chart
            filterCandlesticks(); // Apply date filter
            normalizechart(); // Set axis scale
            displayCandlesticks(); // Display candlesticks
            findAndDisplayPeaksAndValleys(); // Mark peaks and valleys
            findAndDisplayWaves(); // Identify waves
        }

        /// <summary>
        /// Draws a line connecting the start and end points of a wave
        /// </summary>
        /// <param name="startDate">Start date of the wave</param>
        /// <param name="endDate">End date of the wave</param>
        /// <param name="color">Color for the line</param>
        private void drawWaveLine(DateTime startDate, DateTime endDate, Color color)
        {
            // Find candlesticks corresponding to start and end dates
            var start = boundCandlesticks.FirstOrDefault(c => c.Date == startDate);
            var end = boundCandlesticks.FirstOrDefault(c => c.Date == endDate);
            if (start == null || end == null) return; // Exit if not found

            // Create unique name for this wave line
            string waveName = $"WaveLine_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";

            // Skip if line already exists
            if (chart_Candlesticks.Series.Any(s => s.Name == waveName))
                return;

            // Create new series for the line
            Series waveLine = new Series(waveName);
            waveLine.ChartType = SeriesChartType.Line; // Line chart type
            waveLine.Color = color; // Line color
            waveLine.BorderWidth = 3; // Line thickness
            waveLine.XValueType = ChartValueType.DateTime; // X-axis is DateTime
            waveLine.IsXValueIndexed = false; // Use actual dates, not indices
            waveLine.ChartArea = "ChartArea_OHLC"; // Chart area to use

            // Add start and end points to series
            waveLine.Points.AddXY(start.Date, (double)start.High); // Start at high of first candlestick
            waveLine.Points.AddXY(end.Date, (double)end.Low); // End at low of last candlestick

            chart_Candlesticks.Series.Add(waveLine); // Add the line to the chart
        }

        /// <summary>
        /// Timer event handler for simulation animation
        /// </summary>
        /// <param name="sender">The timer that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void timer_simulation_Tick(object sender, EventArgs e)
        {
            if (_simulationStep >= hscroll_Steps.Value) // Check if we've reached max steps
            {
                // Stop simulation when max steps reached
                timer_simulation.Stop();
                button_Start_Stop.Text = "Start"; // Reset button text
                button_Increase_Step.Enabled = true; // Enable manual stepping
                button_Decrease_Step.Enabled = true;
                return;
            }

            // Increment simulation step
            _simulationStep++;

            // Redraw visualization with updated step
            ClearTemporaryRubberband();
            DrawTemporaryRectangle(_dragStartIndex, _dragEndIndex);
            DrawTemporaryDiagonal(_dragStartIndex, _dragEndIndex);
            DrawTemporaryFibLevels(_dragStartIndex, _dragEndIndex);
            DrawTemporaryConfirmations(_dragStartIndex, _dragEndIndex);
        }

        /// <summary>
        /// Updates the count of confirmation points displayed in the label
        /// </summary>
        private void UpdateConfirmationCount()
        {
            // Count only the annotations that start with Temp_Conf_
            int n = chart_Candlesticks.Annotations
                        .Count(a => a.Name.StartsWith("Temp_Conf_"));

            // Update the label with the count
            label_Confirmations.Text = n.ToString();
        }

        /// <summary>
        /// Updates the simulation by a specified number of steps
        /// </summary>
        /// <param name="increment">Number of steps to increment (can be negative)</param>
        private void SimulateStep(int increment)
        {
            _simulationStep += increment; // Add the increment to current step

            // Enforce boundaries to keep steps within valid range
            if (_simulationStep < 0) _simulationStep = 0;
            if (_simulationStep > hscroll_Steps.Maximum) _simulationStep = hscroll_Steps.Maximum;

            hscroll_Steps.Value = _simulationStep; // Update scrollbar position to match
            label_steps_value.Text = _simulationStep.ToString(); // Update step display label

            // Calculate price based on current step position
            decimal waveHeight = Math.Abs(boundCandlesticks[_dragEndIndex].Low - boundCandlesticks[_dragStartIndex].High);
            decimal range = (decimal)hscroll_SimulationRange.Value / 100m; // Convert percentage to decimal
            _simulationPrice = boundCandlesticks[_dragStartIndex].High +
                               (waveHeight * range * ((decimal)_simulationStep / hscroll_Steps.Maximum));

            // Redraw all visualization components with updated price
            ClearTemporaryRubberband();
            DrawTemporaryRectangle(_dragStartIndex, _dragEndIndex);
            DrawTemporaryDiagonal(_dragStartIndex, _dragEndIndex);
            DrawTemporaryFibLevels(_dragStartIndex, _dragEndIndex);
            DrawTemporaryConfirmations(_dragStartIndex, _dragEndIndex);
        }

        /// <summary>
        /// Event handler for Increase Step button click
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">Event arguments</param>
        private void button_Increase_Step_Click(object sender, EventArgs e)
        {
            SimulateStep(1); // Step forward one increment
        }

        /// <summary>
        /// Event handler for Decrease Step button click
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">Event arguments</param>
        private void button_Decrease_Step_Click(object sender, EventArgs e)
        {
            SimulateStep(-1); // Step backward one increment
        }

        /// <summary>
        /// Determines if the candlestick at the given index is a peak
        /// </summary>
        /// <param name="i">Index to check</param>
        /// <returns>True if the candlestick is a peak, false otherwise</returns>
        private bool IsPeak(int i)
        {
            // Check if index is within bounds
            if (i < 0 || i >= boundCandlesticks.Count)
                return false;

            int margin = scrollbar_Margin.Value; // Get margin from control
            var high = boundCandlesticks[i].High; // High value of the candlestick to check

            // Check all neighbors within margin
            for (int j = Math.Max(0, i - margin); j <= Math.Min(boundCandlesticks.Count - 1, i + margin); j++)
            {
                if (j != i && boundCandlesticks[j].High >= high) // If any neighbor has higher or equal high
                    return false; // Not a peak
            }
            return true; // Is a peak if all checks passed
        }

        /// <summary>
        /// Determines if the candlestick at the given index is a valley
        /// </summary>
        /// <param name="i">Index to check</param>
        /// <returns>True if the candlestick is a valley, false otherwise</returns>
        private bool IsValley(int i)
        {
            // Check if index is within bounds
            if (i < 0 || i >= boundCandlesticks.Count)
                return false;

            int margin = scrollbar_Margin.Value; // Get margin from control
            var low = boundCandlesticks[i].Low; // Low value of the candlestick to check

            // Check all neighbors within margin
            for (int j = Math.Max(0, i - margin); j <= Math.Min(boundCandlesticks.Count - 1, i + margin); j++)
            {
                if (j != i && boundCandlesticks[j].Low <= low) // If any neighbor has lower or equal low
                    return false; // Not a valley
            }
            return true; // Is a valley if all checks passed
        }

        /// <summary>
        /// Event handler for mouse down on the chart - starts rubber-band selection of waves
        /// </summary>
        /// <param name="sender">The chart that triggered the event</param>
        /// <param name="e">Mouse event arguments</param>
        private void chart_Candlesticks_MouseDown(object sender, MouseEventArgs e)
        {
            var hit = chart_Candlesticks.HitTest(e.X, e.Y); // Test what was clicked
            if (hit.ChartElementType != ChartElementType.DataPoint) // Only proceed if a data point was clicked
                return;

            int idx = hit.PointIndex; // Get index of clicked point

            // Only allow starting from peaks or valleys
            if (IsPeak(idx) || IsValley(idx))
            {
                _isDragging = true; // Set dragging flag
                _dragStartIndex = idx; // Set start index
                _dragEndIndex = idx; // Initially, end is same as start

                // Clear any existing temporary drawings
                ClearTemporaryRubberband();
            }
        }

        /// <summary>
        /// Draws Fibonacci levels between start and end points during simulation
        /// </summary>
        /// <param name="start">Start index</param>
        /// <param name="end">End index</param>
        private void DrawTemporaryFibLevels(int start, int end)
        {
            // Ensure indices are in correct order
            int minIndex = Math.Min(start, end);
            int maxIndex = Math.Max(start, end);

            // Guard against invalid indices
            if (minIndex < 0 || maxIndex >= boundCandlesticks.Count || minIndex > maxIndex)
                return;

            // Get price range
            double startPrice = (double)boundCandlesticks[start].High;
            double endPrice = (double)boundCandlesticks[end].Low;
            double fullRange = Math.Abs(startPrice - endPrice);

            // Determine current simulation height
            double currentRange = fullRange;
            if (_simulationStep > 0)
            {
                double completionPercent = (double)_simulationStep / hscroll_Steps.Value;
                currentRange = fullRange * completionPercent;
            }

            // Calculate bottom and top of the range
            double bottom = Math.Min(startPrice, endPrice);
            double top = bottom + currentRange;

            // Fibonacci levels (as percentages)
            double[] fibLevels = { 0.0, 0.236, 0.382, 0.5, 0.618, 0.764, 1.0 };

            // For each Fibonacci level
            foreach (double level in fibLevels)
            {
                // Calculate actual price at this level
                double fibPrice = bottom + (level * (top - bottom));

                // Only draw if this level is within our current simulation range
                if (fibPrice <= top)
                {
                    // Create horizontal line annotation for the Fibonacci level
                    var line = new LineAnnotation
                    {
                        Name = $"Temp_Fib_{start}_{end}_{level}", // Unique name for this line
                        AxisX = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisX, // X-axis to use
                        AxisY = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisY, // Y-axis to use
                        ClipToChartArea = "ChartArea_OHLC", // Restrict to chart area
                        IsInfinitive = false, // Finite line (not infinite)
                        Y = fibPrice, // Y position (price level)
                        X = boundCandlesticks[minIndex].Date.ToOADate(), // Start X position
                        Width = boundCandlesticks[maxIndex].Date.ToOADate() - boundCandlesticks[minIndex].Date.ToOADate(), // Width
                        LineColor = Color.Purple, // Line color
                        LineDashStyle = ChartDashStyle.Dash, // Dashed line style
                        LineWidth = 1 // Line thickness
                    };
                    chart_Candlesticks.Annotations.Add(line); // Add line to chart

                    // Add label for the Fibonacci level
                    var label = new TextAnnotation
                    {
                        Name = $"Temp_FibLabel_{start}_{end}_{level}", // Unique name for this label
                        AxisX = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisX, // X-axis to use
                        AxisY = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisY, // Y-axis to use
                        ClipToChartArea = "ChartArea_OHLC", // Restrict to chart area
                        Text = $"{level * 100:F1}% - {fibPrice:F2}", // Format as "23.6% - 123.45"
                        ForeColor = Color.Purple, // Text color
                        X = boundCandlesticks[maxIndex].Date.ToOADate() + 1, // Position just after end of line
                        Y = fibPrice, // Same Y as the line
                        AnchorAlignment = ContentAlignment.MiddleLeft // Align to middle left
                    };
                    chart_Candlesticks.Annotations.Add(label); // Add label to chart
                }
            }
        }

        /// <summary>
        /// Draws a diagonal line for wave visualization during simulation
        /// </summary>
        /// <param name="start">Start index</param>
        /// <param name="end">End index</param>
        private void DrawTemporaryDiagonal(int start, int end)
        {
            var startDate = boundCandlesticks[start].Date; // Get start date
            var endDate = boundCandlesticks[end].Date; // Get end date

            decimal startPrice = boundCandlesticks[start].High; // Start price
            decimal endPrice = boundCandlesticks[end].Low; // End price

            // Full height of the wave
            decimal fullHeight = Math.Abs(startPrice - endPrice);

            // Default end position
            double currentEndY = (double)endPrice;

            // If simulating, calculate current endpoint based on simulation step
            if (_simulationStep > 0)
            {
                decimal completionPercent = (decimal)_simulationStep / hscroll_Steps.Value;

                // Get start and full end positions
                double startY = (double)startPrice;
                double fullEndY = (double)endPrice;

                // Linear interpolation between start and end points
                currentEndY = startY + ((double)completionPercent * (fullEndY - startY));
            }

            // Create new series for the diagonal line
            var series = new Series($"Temp_Line_{start}_{end}")
            {
                ChartType = SeriesChartType.Line, // Line chart type
                Color = Color.Red, // Line color
                BorderWidth = 2, // Line thickness
                XValueType = ChartValueType.DateTime, // X-axis is DateTime
                IsXValueIndexed = false, // Use actual dates, not indices
                ChartArea = "ChartArea_OHLC" // Chart area to use
            };

            // First point is always fixed at start position
            series.Points.AddXY(startDate, (double)startPrice);

            // Second point grows with simulation progress
            series.Points.AddXY(endDate, currentEndY);

            chart_Candlesticks.Series.Add(series); // Add the line to the chart
        }

        /// <summary>
        /// Draws a rectangle highlighting the wave area during simulation
        /// </summary>
        /// <param name="start">Start index</param>
        /// <param name="end">End index</param>
        private void DrawTemporaryRectangle(int start, int end)
        {
            // Validate indices are within bounds
            if (start < 0 || end < 0 || start >= boundCandlesticks.Count || end >= boundCandlesticks.Count)
            {
                return;  // Exit if indices are invalid
            }

            // Get start and end dates
            var startDate = boundCandlesticks[start].Date;
            var endDate = boundCandlesticks[end].Date;

            // Get the original wave high and low values
            decimal startPrice = boundCandlesticks[start].High;
            decimal endPrice = boundCandlesticks[end].Low;

            // Full height of the wave
            decimal fullHeight = Math.Abs(startPrice - endPrice);

            // Calculate the current height based on simulation step
            decimal currentHeight = fullHeight;
            double bottomY, topY;

            if (_simulationStep > 0) // If simulation is in progress
            {
                // Calculate the percentage completion (0-100%)
                decimal completionPercent = (decimal)_simulationStep / hscroll_Steps.Value;

                // Calculate current height based on completion percentage
                currentHeight = fullHeight * completionPercent;

                // Bottom is always fixed at the lowest price
                bottomY = (double)Math.Min(startPrice, endPrice);

                // Top grows up from the bottom as simulation progresses
                topY = bottomY + (double)currentHeight;
            }
            else // When not simulating, show the full rectangle
            {
                bottomY = (double)Math.Min(startPrice, endPrice);
                topY = (double)Math.Max(startPrice, endPrice);
            }

            // Create rectangle annotation
            var box = new RectangleAnnotation
            {
                Name = $"Temp_Rect_{start}_{end}", // Unique name
                AxisX = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisX, // X-axis to use
                AxisY = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisY, // Y-axis to use
                X = startDate.ToOADate(), // Left edge (X position)
                Y = bottomY, // Bottom edge (Y position)
                Width = endDate.ToOADate() - startDate.ToOADate(), // Width (X span)
                Height = topY - bottomY, // Height (Y span) - grows with simulation
                BackColor = Color.FromArgb(40, Color.LightBlue), // Semi-transparent blue fill
                LineColor = Color.FromArgb(120, Color.Blue), // Semi-transparent blue border
                LineWidth = 2, // Border thickness
                ClipToChartArea = "ChartArea_OHLC", // Restrict to chart area
                IsSizeAlwaysRelative = false // Use absolute sizing, not relative
            };

            chart_Candlesticks.Annotations.Add(box); // Add rectangle to chart
        }

        /// <summary>
        /// Event handler for mouse movement during drag operation
        /// </summary>
        /// <param name="sender">The chart that triggered the event</param>
        /// <param name="e">Mouse event arguments</param>
        private void chart_Candlesticks_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) // Only proceed if dragging is active
                return;

            var hit = chart_Candlesticks.HitTest(e.X, e.Y); // Test what's under the cursor
            if (hit.ChartElementType != ChartElementType.DataPoint) // Only proceed if over a data point
                return;

            int idx = hit.PointIndex; // Get index of point under cursor
            if (idx == _dragEndIndex) // Skip if no change from last position
                return;

            _dragEndIndex = idx; // Update drag end position

            try
            {
                // Clear previous temporary drawings
                ClearTemporaryRubberband();

                // Draw all visualization components
                DrawTemporaryRectangle(_dragStartIndex, _dragEndIndex);
                DrawTemporaryDiagonal(_dragStartIndex, _dragEndIndex);
                DrawTemporaryFibLevels(_dragStartIndex, _dragEndIndex);
                DrawTemporaryConfirmations(_dragStartIndex, _dragEndIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MouseMove: {ex.Message}"); // Log any errors
            }
        }

        /// <summary>
        /// Draws a Fibonacci retracement line at a specific level
        /// </summary>
        /// <param name="startIdx">Start index of the wave</param>
        /// <param name="endIdx">End index of the wave</param>
        /// <param name="price">Price level for the line</param>
        /// <param name="level">Fibonacci level value (0-1)</param>
        private void DrawFibonacciLine(int startIdx, int endIdx, double price, double level)
        {
            // Create horizontal line annotation
            var line = new LineAnnotation
            {
                Name = $"Temp_Fib_{startIdx}_{endIdx}_{level}", // Unique name
                AxisX = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisX, // X-axis to use
                AxisY = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisY, // Y-axis to use
                ClipToChartArea = "ChartArea_OHLC", // Restrict to chart area
                IsInfinitive = false, // Finite line (not infinite)
                Y = price, // Y position (price level)
                X = boundCandlesticks[startIdx].Date.ToOADate(), // Start X position
                Width = boundCandlesticks[endIdx].Date.ToOADate() - boundCandlesticks[startIdx].Date.ToOADate(), // Width
                LineColor = Color.Blue, // Line color
                LineDashStyle = ChartDashStyle.Dash, // Dashed line style
                LineWidth = 1 // Line thickness
            };
            chart_Candlesticks.Annotations.Add(line); // Add line to chart
        }

        /// <summary>
        /// Identifies and marks candlesticks that "confirm" Fibonacci levels
        /// </summary>
        /// <param name="start">Start index of the wave</param>
        /// <param name="end">End index of the wave</param>
        private void DrawTemporaryConfirmations(int start, int end)
        {
            // Ensure indices are in correct order for scanning
            int minIdx = Math.Min(start, end);
            int maxIdx = Math.Max(start, end);

            // Guard against invalid indices
            if (minIdx < 0 || maxIdx >= boundCandlesticks.Count)
                return;

            // Get price extremes (wave high and low points)
            decimal startPrice = boundCandlesticks[start].High;
            decimal endPrice = _simulationStep > 0 ? _simulationPrice : boundCandlesticks[end].Low;

            bool isUpWave = endPrice > startPrice; // Determine direction of wave

            // Ensure correct price ordering for Fibonacci calculations
            decimal waveHigh = isUpWave ? endPrice : startPrice;
            decimal waveLow = isUpWave ? startPrice : endPrice;
            decimal height = waveHigh - waveLow;

            // Calculate Fibonacci levels
            decimal[] fibLevels = new decimal[]
            {
                waveLow,                     // 0%
                waveLow + height * 0.236m,   // 23.6%
                waveLow + height * 0.382m,   // 38.2%
                waveLow + height * 0.5m,     // 50.0%
                waveLow + height * 0.618m,   // 61.8%
                waveLow + height * 0.764m,   // 76.4%
                waveHigh                     // 100%
            };

            // Tolerance for price match (0.5% of wave height)
            decimal tolerance = height * 0.005m;

            int confirmationCount = 0; // Counter for confirmations found

            // Scan all candlesticks, excluding those within the wave itself
            for (int i = 0; i < boundCandlesticks.Count; i++)
            {
                // Skip candlesticks within the wave
                if (i >= minIdx && i <= maxIdx)
                    continue;

                var candle = boundCandlesticks[i]; // Current candlestick

                foreach (decimal fibLevel in fibLevels) // Check each Fibonacci level
                {
                    // Check if the candlestick touches or crosses this Fibonacci level
                    if ((candle.Low <= fibLevel + tolerance && candle.High >= fibLevel - tolerance) ||
                        (candle.Low <= fibLevel - tolerance && candle.High >= fibLevel + tolerance))
                    {
                        // Create a marker for this confirmation
                        var marker = new EllipseAnnotation
                        {
                            Name = $"Temp_Conf_{i}", // Unique name
                            AxisX = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisX, // X-axis to use
                            AxisY = chart_Candlesticks.ChartAreas["ChartArea_OHLC"].AxisY, // Y-axis to use
                            ClipToChartArea = "ChartArea_OHLC", // Restrict to chart area
                            IsSizeAlwaysRelative = false, // Use absolute sizing
                            X = candle.Date.ToOADate(), // Position at candlestick date
                            Y = (double)fibLevel, // Position at Fibonacci level
                            Height = 0.5, // Height of marker
                            Width = 0.5, // Width of marker
                            LineColor = Color.Orange, // Border color
                            BackColor = Color.FromArgb(120, Color.Orange), // Fill color
                            LineWidth = 2 // Border thickness
                        };
                        chart_Candlesticks.Annotations.Add(marker); // Add marker to chart

                        confirmationCount++; // Increment confirmation count
                        break; // Only count one confirmation per candlestick
                    }
                }
            }

            // Update the confirmation count display
            label_Confirmations.Text = $"Confirmations: {confirmationCount}";
        }

        /// <summary>
        /// Event handler for step value label click (not used, but created by designer)
        /// </summary>
        private void label_steps_value_Click(object sender, EventArgs e)
        {
            // Empty event handler created by the designer
        }

        /// <summary>
        /// Event handler for Start/Stop button click to control simulation
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">Event arguments</param>
        private void button_Start_Stop_Click(object sender, EventArgs e)
        {
            if (timer_simulation.Enabled) // If simulation is running
            {
                // Stop simulation
                timer_simulation.Stop();
                button_Start_Stop.Text = "Start"; // Change button text
                button_Increase_Step.Enabled = true; // Enable manual stepping
                button_Decrease_Step.Enabled = true;
            }
            else // If simulation is stopped
            {
                // Start simulation
                _simulationStep = 0; // Reset step counter
                _stepSize = 1; // Start moving forward

                // Calculate price range for simulation
                decimal waveHeight = Math.Abs(boundCandlesticks[_dragEndIndex].Low - boundCandlesticks[_dragStartIndex].High);
                decimal range = (decimal)hscroll_SimulationRange.Value / 100m; // Convert percentage to decimal

                _minPrice = boundCandlesticks[_dragStartIndex].High; // Starting price
                _maxPrice = _minPrice + range * waveHeight; // Target price

                // Initialize display with starting position
                _simulationPrice = _minPrice;

                // Show wave components at initial state
                ClearTemporaryRubberband();
                DrawTemporaryRectangle(_dragStartIndex, _dragEndIndex);
                DrawTemporaryDiagonal(_dragStartIndex, _dragEndIndex);
                DrawTemporaryFibLevels(_dragStartIndex, _dragEndIndex);
                DrawTemporaryConfirmations(_dragStartIndex, _dragEndIndex);

                // Start timer for animation
                timer_simulation.Start();
                button_Start_Stop.Text = "Stop"; // Change button text
                button_Increase_Step.Enabled = false; // Disable manual stepping during animation
                button_Decrease_Step.Enabled = false;
            }
        }

        /// <summary>
        /// Draws the initial diagonal line connecting start and end points of a selected wave
        /// </summary>
        /// <param name="start">Start index</param>
        /// <param name="end">End index</param>
        private void DrawInitialDiagonal(int start, int end)
        {
            var startDate = boundCandlesticks[start].Date; // Get start date
            var endDate = boundCandlesticks[end].Date; // Get end date

            double startValue, endValue;

            if (combobox_UpWaves.SelectedIndex != -1) // If an up wave is selected
            {
                // For up waves, connect starting low to ending high
                startValue = (double)boundCandlesticks[start].Low;
                endValue = (double)boundCandlesticks[end].High;
            }
            else // If a down wave is selected
            {
                // For down waves, connect starting high to ending low
                startValue = (double)boundCandlesticks[start].High;
                endValue = (double)boundCandlesticks[end].Low;
            }

            // Create new series for the diagonal line
            var series = new Series($"Initial_Line_{start}_{end}")
            {
                ChartType = SeriesChartType.Line, // Line chart type
                Color = Color.FromArgb(120, Color.Green), // Semi-transparent green
                BorderWidth = 2, // Line thickness
                XValueType = ChartValueType.DateTime, // X-axis is DateTime
                IsXValueIndexed = false, // Use actual dates, not indices
                ChartArea = "ChartArea_OHLC" // Chart area to use
            };

            // Add start and end points to the line
            series.Points.AddXY(startDate, startValue);
            series.Points.AddXY(endDate, endValue);

            // Remove any existing initial line
            var existingLine = chart_Candlesticks.Series
                .FirstOrDefault(s => s.Name?.StartsWith("Initial_Line_") ?? false);
            if (existingLine != null)
            {
                chart_Candlesticks.Series.Remove(existingLine);
            }

            chart_Candlesticks.Series.Add(series); // Add new line to chart
        }

        /// <summary>
        /// Event handler for wave combo box selection change
        /// </summary>
        /// <param name="sender">The combo box that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void combobox_Wave_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedWave = (sender as ComboBox).SelectedItem.ToString(); // Get selected wave text
            string[] parts = selectedWave.Split(new string[] { " - " }, StringSplitOptions.None); // Split date range
            if (parts.Length != 2) return; // Exit if format is invalid

            // Parse dates from text
            DateTime start = DateTime.Parse(parts[0]);
            DateTime end = DateTime.Parse(parts[1]);

            // Find corresponding indices in the data
            _dragStartIndex = boundCandlesticks.ToList().FindIndex(c => c.Date == start);
            _dragEndIndex = boundCandlesticks.ToList().FindIndex(c => c.Date == end);

            if (_dragStartIndex == -1 || _dragEndIndex == -1) return; // Exit if dates not found

            // Enable simulation controls
            button_Start_Stop.Enabled = true;
            button_Increase_Step.Enabled = true;
            button_Decrease_Step.Enabled = true;

            // Initialize simulation values
            _simulationStep = 0;
            _simulationPrice = boundCandlesticks[_dragStartIndex].High;

            // Show initial state
            ClearTemporaryRubberband();
            DrawTemporaryRectangle(_dragStartIndex, _dragEndIndex);
            DrawTemporaryDiagonal(_dragStartIndex, _dragEndIndex);
            DrawTemporaryFibLevels(_dragStartIndex, _dragEndIndex);
            DrawTemporaryConfirmations(_dragStartIndex, _dragEndIndex);
        }

        /// <summary>
        /// Event handler for simulation range scroll bar movement
        /// </summary>
        /// <param name="sender">The scroll bar that triggered the event</param>
        /// <param name="e">Scroll event arguments</param>
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            // Update the value label to show current range percentage
            label_SimRangeValue.Text = hscroll_SimulationRange.Value + "%";
        }

        /// <summary>
        /// Event handler for up waves label click (not used, but created by designer)
        /// </summary>
        private void label_combo_upwaves_Click(object sender, EventArgs e)
        {
            // Empty event handler created by the designer
        }

        /// <summary>
        /// Event handler for down waves label click (not used, but created by designer)
        /// </summary>
        private void label_combo_downwaves_Click(object sender, EventArgs e)
        {
            // Empty event handler created by the designer
        }

        /// <summary>
        /// Event handler for simulation range value label click (not used, but created by designer)
        /// </summary>
        private void label_SimRangeValue_Click(object sender, EventArgs e)
        {
            // Empty event handler created by the designer
        }

        /// <summary>
        /// Updates the "# of Steps" label when the steps scrollbar is moved
        /// </summary>
        /// <param name="sender">The scroll bar that triggered the event</param>
        /// <param name="e">Scroll event arguments</param>
        private void hscroll_Steps_Scroll(object sender, ScrollEventArgs e)
        {
            // Update the label with the current value of the scrollbar
            label_steps_value.Text = hscroll_Steps.Value.ToString();
        }

        /// <summary>
        /// Event handler for margin scrollbar movement to adjust peak/valley detection sensitivity
        /// </summary>
        /// <param name="sender">The scroll bar that triggered the event</param>
        /// <param name="e">Scroll event arguments</param>
        private void scrollbar_Margin_Scroll(object sender, ScrollEventArgs e)
        {
            // Re-draw everything with the new margin setting
            ClearTemporaryRubberband(); // Clear existing visual elements
            filterCandlesticks(); // Apply date filter
            normalizechart(); // Adjust chart scale
            displayCandlesticks(); // Update chart display
            findAndDisplayPeaksAndValleys(); // Recalculate peaks/valleys with new margin
            findAndDisplayWaves(); // Recalculate waves
        }

        /// <summary>
        /// Updates the "Simulation Range" label when the range scrollbar is moved
        /// </summary>
        /// <param name="sender">The scroll bar that triggered the event</param>
        /// <param name="e">Scroll event arguments</param>
        private void hscroll_SimulationRange_Scroll(object sender, ScrollEventArgs e)
        {
            // Update the label with current percentage value
            label_SimRangeValue.Text = hscroll_SimulationRange.Value + "%";
        }

        /// <summary>
        /// Event handler for confirmations label click (not used, but created by designer)
        /// </summary>
        private void label_Confirmations_Click(object sender, EventArgs e)
        {
            // Empty event handler created by the designer
        }

        /// <summary>
        /// Event handler for mouse up event to end drag operation
        /// </summary>
        /// <param name="sender">The chart that triggered the event</param>
        /// <param name="e">Mouse event arguments</param>
        private void chart_Candlesticks_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return; // Only proceed if dragging was active

            _isDragging = false; // Clear dragging flag
            ClearTemporaryRubberband(); // Clear temporary visual elements
            // Note: TODO comment indicates future plan to make the wave persistent if valid
        }
    }
}