namespace Project1_stock
{
    partial class Form_Candlesticks
    {
        
        /// Designer-managed container for components.
        
        private System.ComponentModel.IContainer components = null;

        
        /// Releases resources used by the form.
        
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        
        /// Initializes the UI components. Do not modify the contents with the code editor.
        
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.button_Load_Ticker = new System.Windows.Forms.Button();
            this.openFileDialog_TickerChooser = new System.Windows.Forms.OpenFileDialog();
            this.datetimepicker_Start_Date = new System.Windows.Forms.DateTimePicker();
            this.datetimepicker_End_Date = new System.Windows.Forms.DateTimePicker();
            this.chart_Candlesticks = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.candlestickBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label_ChartControls = new System.Windows.Forms.Label();
            this.label_Date = new System.Windows.Forms.Label();
            this.button_Refresh_Data = new System.Windows.Forms.Button();
            this.combobox_UpWaves = new System.Windows.Forms.ComboBox();
            this.combobox_DownWaves = new System.Windows.Forms.ComboBox();
            this.label_combo_upwaves = new System.Windows.Forms.Label();
            this.label_combo_downwaves = new System.Windows.Forms.Label();
            this.scrollbar_Margin = new System.Windows.Forms.HScrollBar();
            this.label_margin = new System.Windows.Forms.Label();
            this.timer_simulation = new System.Windows.Forms.Timer(this.components);
            this.button_Start_Stop = new System.Windows.Forms.Button();
            this.button_Increase_Step = new System.Windows.Forms.Button();
            this.button_Decrease_Step = new System.Windows.Forms.Button();
            this.hscroll_Steps = new System.Windows.Forms.HScrollBar();
            this.label_steps_value = new System.Windows.Forms.Label();
            this.hscroll_SimulationRange = new System.Windows.Forms.HScrollBar();
            this.label_numberofsteps = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label_SimRangeValue = new System.Windows.Forms.Label();
            this.label_Confirmations = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Candlesticks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.candlestickBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Load_Ticker
            // 
            this.button_Load_Ticker.Location = new System.Drawing.Point(900, 690);
            this.button_Load_Ticker.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Load_Ticker.Name = "button_Load_Ticker";
            this.button_Load_Ticker.Size = new System.Drawing.Size(156, 60);
            this.button_Load_Ticker.TabIndex = 1;
            this.button_Load_Ticker.Text = "Open Stock";
            this.button_Load_Ticker.UseVisualStyleBackColor = true;
            this.button_Load_Ticker.Click += new System.EventHandler(this.button_Stock_Load);
            // 
            // openFileDialog_TickerChooser
            // 
            this.openFileDialog_TickerChooser.FileName = "ABBV-Day.csv";
            this.openFileDialog_TickerChooser.Filter = "TickerFile|*.csv| DayFile|*-Day.csv| WeeklyFile|*-Week.csv| MonthlyFile|*-Month.c" +
    "sv";
            this.openFileDialog_TickerChooser.Multiselect = true;
            this.openFileDialog_TickerChooser.FileOk += new System.ComponentModel.CancelEventHandler(this.fileHandler);
            // 
            // datetimepicker_Start_Date
            // 
            this.datetimepicker_Start_Date.Location = new System.Drawing.Point(61, 745);
            this.datetimepicker_Start_Date.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.datetimepicker_Start_Date.Name = "datetimepicker_Start_Date";
            this.datetimepicker_Start_Date.Size = new System.Drawing.Size(298, 26);
            this.datetimepicker_Start_Date.TabIndex = 3;
            this.datetimepicker_Start_Date.Value = new System.DateTime(2022, 12, 31, 9, 56, 0, 0);
            // 
            // datetimepicker_End_Date
            // 
            this.datetimepicker_End_Date.Location = new System.Drawing.Point(61, 821);
            this.datetimepicker_End_Date.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.datetimepicker_End_Date.Name = "datetimepicker_End_Date";
            this.datetimepicker_End_Date.Size = new System.Drawing.Size(298, 26);
            this.datetimepicker_End_Date.TabIndex = 4;
            // 
            // chart_Candlesticks
            // 
            chartArea1.AxisX.IsMarginVisible = false;
            chartArea1.Name = "ChartArea_OHLC";
            this.chart_Candlesticks.ChartAreas.Add(chartArea1);
            this.chart_Candlesticks.DataSource = this.candlestickBindingSource;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chart_Candlesticks.Legends.Add(legend1);
            this.chart_Candlesticks.Location = new System.Drawing.Point(372, 5);
            this.chart_Candlesticks.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chart_Candlesticks.Name = "chart_Candlesticks";
            this.chart_Candlesticks.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            series1.ChartArea = "ChartArea_OHLC";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series1.CustomProperties = "PriceDownColor=Red, PriceUpColor=Green";
            series1.IsXValueIndexed = true;
            series1.Legend = "Legend1";
            series1.Name = "Series_OHLC";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series1.YValuesPerPoint = 4;
            this.chart_Candlesticks.Series.Add(series1);
            this.chart_Candlesticks.Size = new System.Drawing.Size(1253, 619);
            this.chart_Candlesticks.TabIndex = 5;
            this.chart_Candlesticks.Text = "chart_candlesticks";
            this.chart_Candlesticks.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart_Candlesticks_MouseDown);
            this.chart_Candlesticks.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_Candlesticks_MouseMove);
            this.chart_Candlesticks.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chart_Candlesticks_MouseUp);
            // 
            // label_ChartControls
            // 
            this.label_ChartControls.AutoSize = true;
            this.label_ChartControls.Location = new System.Drawing.Point(57, 664);
            this.label_ChartControls.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_ChartControls.Name = "label_ChartControls";
            this.label_ChartControls.Size = new System.Drawing.Size(68, 20);
            this.label_ChartControls.TabIndex = 7;
            this.label_ChartControls.Text = "Controls";
            this.label_ChartControls.Click += new System.EventHandler(this.label_ChartControls_Click);
            // 
            // label_Date
            // 
            this.label_Date.AutoSize = true;
            this.label_Date.Location = new System.Drawing.Point(57, 701);
            this.label_Date.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Date.Name = "label_Date";
            this.label_Date.Size = new System.Drawing.Size(52, 20);
            this.label_Date.TabIndex = 8;
            this.label_Date.Text = "Dates";
            // 
            // button_Refresh_Data
            // 
            this.button_Refresh_Data.Location = new System.Drawing.Point(900, 788);
            this.button_Refresh_Data.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Refresh_Data.Name = "button_Refresh_Data";
            this.button_Refresh_Data.Size = new System.Drawing.Size(156, 63);
            this.button_Refresh_Data.TabIndex = 9;
            this.button_Refresh_Data.Text = "Update Data";
            this.button_Refresh_Data.UseVisualStyleBackColor = true;
            this.button_Refresh_Data.Click += new System.EventHandler(this.button_Refresh_Data_Click);
            // 
            // combobox_UpWaves
            // 
            this.combobox_UpWaves.FormattingEnabled = true;
            this.combobox_UpWaves.Location = new System.Drawing.Point(656, 722);
            this.combobox_UpWaves.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.combobox_UpWaves.Name = "combobox_UpWaves";
            this.combobox_UpWaves.Size = new System.Drawing.Size(180, 28);
            this.combobox_UpWaves.TabIndex = 10;
            this.combobox_UpWaves.SelectedIndexChanged += new System.EventHandler(this.combobox_UpWaves_SelectedIndexChanged);
            // 
            // combobox_DownWaves
            // 
            this.combobox_DownWaves.FormattingEnabled = true;
            this.combobox_DownWaves.Location = new System.Drawing.Point(656, 819);
            this.combobox_DownWaves.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.combobox_DownWaves.Name = "combobox_DownWaves";
            this.combobox_DownWaves.Size = new System.Drawing.Size(180, 28);
            this.combobox_DownWaves.TabIndex = 11;
            this.combobox_DownWaves.SelectedIndexChanged += new System.EventHandler(this.combobox_DownWaves_SelectedIndexChanged);
            // 
            // label_combo_upwaves
            // 
            this.label_combo_upwaves.AutoSize = true;
            this.label_combo_upwaves.Location = new System.Drawing.Point(671, 686);
            this.label_combo_upwaves.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_combo_upwaves.Name = "label_combo_upwaves";
            this.label_combo_upwaves.Size = new System.Drawing.Size(78, 20);
            this.label_combo_upwaves.TabIndex = 12;
            this.label_combo_upwaves.Text = "Up waves";
            this.label_combo_upwaves.Click += new System.EventHandler(this.label_combo_upwaves_Click);
            // 
            // label_combo_downwaves
            // 
            this.label_combo_downwaves.AutoSize = true;
            this.label_combo_downwaves.Location = new System.Drawing.Point(671, 788);
            this.label_combo_downwaves.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_combo_downwaves.Name = "label_combo_downwaves";
            this.label_combo_downwaves.Size = new System.Drawing.Size(98, 20);
            this.label_combo_downwaves.TabIndex = 13;
            this.label_combo_downwaves.Text = "Down waves";
            this.label_combo_downwaves.Click += new System.EventHandler(this.label_combo_downwaves_Click);
            // 
            // scrollbar_Margin
            // 
            this.scrollbar_Margin.LargeChange = 1;
            this.scrollbar_Margin.Location = new System.Drawing.Point(83, 543);
            this.scrollbar_Margin.Maximum = 4;
            this.scrollbar_Margin.Minimum = 1;
            this.scrollbar_Margin.Name = "scrollbar_Margin";
            this.scrollbar_Margin.Size = new System.Drawing.Size(234, 21);
            this.scrollbar_Margin.TabIndex = 14;
            this.scrollbar_Margin.Value = 2;
            this.scrollbar_Margin.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollbar_Margin_Scroll);
            // 
            // label_margin
            // 
            this.label_margin.AutoSize = true;
            this.label_margin.Location = new System.Drawing.Point(146, 481);
            this.label_margin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_margin.Name = "label_margin";
            this.label_margin.Size = new System.Drawing.Size(57, 20);
            this.label_margin.TabIndex = 15;
            this.label_margin.Text = "Margin";
            // 
            // timer_simulation
            // 
            this.timer_simulation.Interval = 500;
            this.timer_simulation.Tick += new System.EventHandler(this.timer_simulation_Tick);
            // 
            // button_Start_Stop
            // 
            this.button_Start_Stop.Location = new System.Drawing.Point(384, 737);
            this.button_Start_Stop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Start_Stop.Name = "button_Start_Stop";
            this.button_Start_Stop.Size = new System.Drawing.Size(193, 92);
            this.button_Start_Stop.TabIndex = 16;
            this.button_Start_Stop.Text = "Start/Stop";
            this.button_Start_Stop.UseVisualStyleBackColor = true;
            this.button_Start_Stop.Click += new System.EventHandler(this.button_Start_Stop_Click);
            // 
            // button_Increase_Step
            // 
            this.button_Increase_Step.Location = new System.Drawing.Point(597, 701);
            this.button_Increase_Step.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Increase_Step.Name = "button_Increase_Step";
            this.button_Increase_Step.Size = new System.Drawing.Size(51, 56);
            this.button_Increase_Step.TabIndex = 17;
            this.button_Increase_Step.Text = "+";
            this.button_Increase_Step.UseVisualStyleBackColor = true;
            this.button_Increase_Step.Click += new System.EventHandler(this.button_Increase_Step_Click);
            // 
            // button_Decrease_Step
            // 
            this.button_Decrease_Step.Location = new System.Drawing.Point(597, 805);
            this.button_Decrease_Step.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Decrease_Step.Name = "button_Decrease_Step";
            this.button_Decrease_Step.Size = new System.Drawing.Size(51, 52);
            this.button_Decrease_Step.TabIndex = 18;
            this.button_Decrease_Step.Text = "-";
            this.button_Decrease_Step.UseVisualStyleBackColor = true;
            this.button_Decrease_Step.Click += new System.EventHandler(this.button_Decrease_Step_Click);
            // 
            // hscroll_Steps
            // 
            this.hscroll_Steps.Location = new System.Drawing.Point(1254, 701);
            this.hscroll_Steps.Minimum = 1;
            this.hscroll_Steps.Name = "hscroll_Steps";
            this.hscroll_Steps.Size = new System.Drawing.Size(285, 40);
            this.hscroll_Steps.TabIndex = 19;
            this.hscroll_Steps.Value = 32;
            this.hscroll_Steps.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hscroll_Steps_Scroll);
            // 
            // label_steps_value
            // 
            this.label_steps_value.AutoSize = true;
            this.label_steps_value.Location = new System.Drawing.Point(1277, 669);
            this.label_steps_value.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_steps_value.Name = "label_steps_value";
            this.label_steps_value.Size = new System.Drawing.Size(224, 20);
            this.label_steps_value.TabIndex = 20;
            this.label_steps_value.Text = "hscroll_Steps.Value.ToString()";
            this.label_steps_value.Click += new System.EventHandler(this.label_steps_value_Click);
            // 
            // hscroll_SimulationRange
            // 
            this.hscroll_SimulationRange.Location = new System.Drawing.Point(1265, 851);
            this.hscroll_SimulationRange.Minimum = 1;
            this.hscroll_SimulationRange.Name = "hscroll_SimulationRange";
            this.hscroll_SimulationRange.Size = new System.Drawing.Size(269, 30);
            this.hscroll_SimulationRange.TabIndex = 21;
            this.hscroll_SimulationRange.Value = 20;
            this.hscroll_SimulationRange.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // label_numberofsteps
            // 
            this.label_numberofsteps.AutoSize = true;
            this.label_numberofsteps.Location = new System.Drawing.Point(1113, 701);
            this.label_numberofsteps.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_numberofsteps.Name = "label_numberofsteps";
            this.label_numberofsteps.Size = new System.Drawing.Size(129, 20);
            this.label_numberofsteps.TabIndex = 22;
            this.label_numberofsteps.Text = "Number of Steps";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1113, 851);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 20);
            this.label1.TabIndex = 23;
            this.label1.Text = "Simulation Range";
            // 
            // label_SimRangeValue
            // 
            this.label_SimRangeValue.AutoSize = true;
            this.label_SimRangeValue.Location = new System.Drawing.Point(1266, 809);
            this.label_SimRangeValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_SimRangeValue.Name = "label_SimRangeValue";
            this.label_SimRangeValue.Size = new System.Drawing.Size(273, 20);
            this.label_SimRangeValue.TabIndex = 24;
            this.label_SimRangeValue.Text = "hscroll_SimulationRange.Value + \"%\"";
            this.label_SimRangeValue.Click += new System.EventHandler(this.label_SimRangeValue_Click);
            // 
            // label_Confirmations
            // 
            this.label_Confirmations.AutoSize = true;
            this.label_Confirmations.Location = new System.Drawing.Point(122, 383);
            this.label_Confirmations.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Confirmations.Name = "label_Confirmations";
            this.label_Confirmations.Size = new System.Drawing.Size(124, 20);
            this.label_Confirmations.TabIndex = 25;
            this.label_Confirmations.Text = "Confirmations: 0";
            this.label_Confirmations.Click += new System.EventHandler(this.label_Confirmations_Click);
            // 
            // Form_Candlesticks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1638, 955);
            this.Controls.Add(this.label_Confirmations);
            this.Controls.Add(this.label_SimRangeValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_numberofsteps);
            this.Controls.Add(this.hscroll_SimulationRange);
            this.Controls.Add(this.label_steps_value);
            this.Controls.Add(this.hscroll_Steps);
            this.Controls.Add(this.button_Decrease_Step);
            this.Controls.Add(this.button_Increase_Step);
            this.Controls.Add(this.button_Start_Stop);
            this.Controls.Add(this.label_margin);
            this.Controls.Add(this.scrollbar_Margin);
            this.Controls.Add(this.label_combo_downwaves);
            this.Controls.Add(this.label_combo_upwaves);
            this.Controls.Add(this.combobox_DownWaves);
            this.Controls.Add(this.combobox_UpWaves);
            this.Controls.Add(this.button_Refresh_Data);
            this.Controls.Add(this.label_Date);
            this.Controls.Add(this.label_ChartControls);
            this.Controls.Add(this.chart_Candlesticks);
            this.Controls.Add(this.datetimepicker_End_Date);
            this.Controls.Add(this.datetimepicker_Start_Date);
            this.Controls.Add(this.button_Load_Ticker);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form_Candlesticks";
            this.Text = "Form_Candlesticks";
            this.Load += new System.EventHandler(this.Form_Candlesticks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart_Candlesticks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.candlestickBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Load_Ticker;
        private System.Windows.Forms.OpenFileDialog openFileDialog_TickerChooser;
        private System.Windows.Forms.BindingSource candlestickBindingSource;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Candlesticks;
        private System.Windows.Forms.Label label_ChartControls;
        private System.Windows.Forms.Label label_Date;
        private System.Windows.Forms.Button button_Refresh_Data;
        private System.Windows.Forms.ComboBox combobox_UpWaves;
        private System.Windows.Forms.ComboBox combobox_DownWaves;
        private System.Windows.Forms.Label label_combo_upwaves;
        private System.Windows.Forms.Label label_combo_downwaves;
        private System.Windows.Forms.HScrollBar scrollbar_Margin;
        private System.Windows.Forms.Label label_margin;
        public System.Windows.Forms.DateTimePicker datetimepicker_Start_Date;
        public System.Windows.Forms.DateTimePicker datetimepicker_End_Date;
        private System.Windows.Forms.Timer timer_simulation;
        private System.Windows.Forms.Button button_Start_Stop;
        private System.Windows.Forms.Button button_Increase_Step;
        private System.Windows.Forms.Button button_Decrease_Step;
        private System.Windows.Forms.HScrollBar hscroll_Steps;
        private System.Windows.Forms.Label label_steps_value;
        private System.Windows.Forms.HScrollBar hscroll_SimulationRange;
        private System.Windows.Forms.Label label_numberofsteps;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_SimRangeValue;
        private System.Windows.Forms.Label label_Confirmations;
    }
}
