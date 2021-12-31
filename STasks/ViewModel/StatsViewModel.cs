using STasks.Model;
using STasks.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using OxyPlot.Series;
using OxyPlot;
using Converters;

namespace STasks.ViewModel
{
    public class StatsViewModel : BaseViewModel
    {
        public StatsViewModel()
        {

            Services.PlotingService.Instance.DataAvailable += (s, e) =>
            {

                this.IsDataAvailable = PlotingService.Instance.IsDataAvailable;


            };





            //////obsolete section

            OxyColor primary = OxyColor.Parse("#00669e");
            OxyColor primaryLight = OxyColor.Parse("#16a4e3");
            OxyColor LightLineColor = OxyColor.Parse("#e1e1e1");
            OxyColor midGray = OxyColor.Parse("#959595");
            OxyPlot.PlotModel pm = new OxyPlot.PlotModel();
            //pm.InvalidatePlot(true);
            //pm.Axes.Add(new OxyPlot.Axes.LinearAxis() { Maximum = 10, Minimum = 0, MajorStep = 2 });
            SinTestModel = pm;
            pm.LegendSymbolLength = 8;
            pm.IsLegendVisible = false;
            int days_from_1899 = (int) (DateTime.Now - new DateTime(1899, 12, 30)).TotalDays;
            //fs.Background = OxyPlot.OxyColor.Parse("#16a4e3");
           
            OxyPlot.Series.AreaSeries progSeries = new AreaSeries();
            OxyPlot.Series.AreaSeries ProgRateSeries = new AreaSeries();
            OxyPlot.Axes.DateTimeAxis dateAx = new OxyPlot.Axes.DateTimeAxis();
            
                //12-20-1899 12:0:0 am
            PlotingService.Instance.DataAvailable += (s, e) => {
                progSeries.Points.Clear();
                progSeries.Points.AddRange( PlotingService.Instance.ProgressCurvPoints);

                ProgRateSeries.Points.Clear();
                ProgRateSeries.Points.AddRange(PlotingService.Instance.ProgressRateCurvPoints);
                pm.InvalidatePlot(true);
                
               // dateAx.StartPosition = (PlotingService.Instance.StartTime- new DateTime(1899, 12, 30)).TotalDays;
                dateAx.LabelFormatter = (ss) => DateTimeUtils.FormatDate(new DateTime(1899, 12, 30) + TimeSpan.FromDays(ss));

            };
            var BeginiOxydt = new DateTime(1899, 12, 30);
            dateAx.AxisChanged += (s, e) =>
            {
                if(e.ChangeType== OxyPlot.Axes.AxisChangeTypes.Pan)
                {
                   PlotingService.Instance.DevCurrentWorkingDateTime= DevCurrentWorkingDateTime = BeginiOxydt.AddDays(dateAx.Offset);
                }
            };
            //# colros
            ProgRateSeries.Color = OxyColors.Green;
            ProgRateSeries.Color2 = (OxyColors.Transparent);
            ProgRateSeries.Fill = OxyColor.FromAColor(40, OxyColors.LightGreen);

            progSeries.Color = OxyColors.HotPink;
            progSeries.Color2 = OxyColors.Transparent;
            progSeries.Fill = OxyColor.FromAColor(25, OxyColors.HotPink);

            //progSeries.Color = 
            /*progSeries.Color = primaryLight;
            progSeries.Color2 = OxyColors.Transparent;
            progSeries.Fill = OxyColor.FromAColor(25, primaryLight);

    */


            ProgRateSeries.ConstantY2 = 0;
            progSeries.Points.Add(new DataPoint(0, 0));
            progSeries.Points.Add(new DataPoint(1, 10));
            progSeries.Points.Add(new DataPoint(2, 12));
            progSeries.Points.Add(new DataPoint(2.5, 40));
            progSeries.Points.Add(new DataPoint(5, 44));
            progSeries.Points.Add(new DataPoint(6, 45));
            progSeries.Points.Add(new DataPoint(7, 49));
            progSeries.Points.Add(new DataPoint(10, 60));
            progSeries.ConstantY2 = 0;
            progSeries.Smooth = true;
            pm.Series.Add(progSeries);
            //fs.Background = OxyColor.FromAColor(125, OxyColor.Parse("#16a4e3"));
            ProgRateSeries.LineStyle = LineStyle.Dash;
            OxyPlot.Axes.LinearAxis ax = new OxyPlot.Axes.LinearAxis();
            pm.Axes.Add(ax);
            pm.PlotAreaBorderThickness = new OxyThickness(0);
            ax.MajorStep = 40;
            ax.Maximum = 100;
            ax.TickStyle = OxyPlot.Axes.TickStyle.None;
            ax.MinorStep = 10;
            ax.IsPanEnabled = false;
            pm.PlotType = PlotType.Polar;
            ax.MinorGridlineThickness = 0.25;
            ax.MajorGridlineStyle = LineStyle.Solid;
            ax.MinorGridlineStyle = LineStyle.None;
            ax.MajorGridlineColor = OxyColor.FromAColor(128, primary);
            // ax.MinorGridlineColor = LightLineColor;

            
            pm.Axes.Add(dateAx);
            dateAx.TextColor = midGray;
            ax.Title = "Tasks";
            ax.TickStyle = OxyPlot.Axes.TickStyle.None;

            ax.LabelFormatter = (s) => null;
            dateAx.IsZoomEnabled = false;
            //ax.MajorGridlineStyle = LineStyle.Dot;
            pm.Series.Add(ProgRateSeries);
            
            
        }

        /// <summary>
        /// used in dev and tasting, this is the datetime used for completion instead of Now,
        /// </summary>
        private DateTime _DevCurrentWorkingDateTime;
        public DateTime DevCurrentWorkingDateTime
        {
            set { _DevCurrentWorkingDateTime = value; notif(nameof(DevCurrentWorkingDateTime)); }
            get { return _DevCurrentWorkingDateTime; }
        }



        private OxyPlot.PlotModel _SinTestModel;
        public OxyPlot.PlotModel SinTestModel
        {
            set { _SinTestModel = value; notif(nameof(SinTestModel)); }
            get { return _SinTestModel; }
        }




        private bool _IsDataAvailable;
        public bool IsDataAvailable
        {
            set { _IsDataAvailable = value; notif(nameof(IsDataAvailable)); }
            get { return _IsDataAvailable; }
        }


        private int _ProgressCurv;
        public int ProgressCurv
        {
            set { _ProgressCurv = value; notif(nameof(ProgressCurv)); }
            get { return _ProgressCurv; }
        }




    }
}
