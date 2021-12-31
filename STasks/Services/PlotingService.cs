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
using STasks.Common;

namespace STasks.Services
{
    /// <summary>
    /// ths may get renamed: PlotingService, and become a singltone that provides the ploting views with data and events
    /// </summary>
    public class PlotingService
    {

        private static PlotingService _instance = null;
        public static PlotingService Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlotingService();
                }
                return _instance;
            } }


        /// <summary>
        /// the raw data required for ploting view ; there is no backwards flow of data or events back to the source
        /// aims to separates the PLOTing concerns from the rest of the app to avoid class coupling
        /// this is all the 
        /// </summary>
        public interface IPlotableUserProgressData
        {
            ReadOnlyObservableCollection<ICompletionPiece> CompletionPiece { get; }
            int GoalWeight { get; }
            DateTime GoalDeadLine { get; }
            DateTime TimeStart { get; }

        }
        /// <summary>
        /// a weight - date pair that may represent one or more completed task,
        /// </summary>
        public interface ICompletionPiece
        {
            int CompletedBuildingBlockWeight { get; }
            DateTime CompletionDate { get; }
        }


        public event EventHandler DataAvailable;

        private IPlotableUserProgressData CurrentData { get; set; }




        public void PushData(IPlotableUserProgressData data)
        {
            CurrentData = data;
            //process data
            ProgressCurvPoints =  GetProgCurvPoints(data);
            StartTime = data.TimeStart;
            ProgressRateCurvPoints = GetProgRateCurvPoints(data);
            IsDataAvailable = true;
            DataAvailable?.Invoke(this, new EventArgs());
        }

        public bool IsDataAvailable { get; set; } = false;

        public static int DayNumber(DateTime dt, DateTime statDate)
        {
            return (int) (dt - new DateTime(1899, 12, 30)).TotalDays;
        }
        public static double DayNumberPrecise(DateTime dt, DateTime statDate)
        {
            return (dt - new DateTime(1899, 12, 30)).TotalDays ; 
        }

        public List<DataPoint> ProgressCurvPoints { get; internal set; }
        public List<DataPoint> ProgressRateCurvPoints { get; internal set; }

        private static List<DataPoint> GetProgCurvPoints(IPlotableUserProgressData data)
        {
            List<DataPoint> outp = new List<DataPoint>();


            var cons = data.CompletionPiece.OrderBy((cp) => cp.CompletionDate).ToArray();
            DateTime curren_dt_goup = DateTime.Now;//random ssignment to shut up the compliler
            bool first_iteration = true;
            int accWeight = 0;
            int acc_weight=0;
           for(int i=0; i<cons.Length; i++)
            {
                var item = cons[i];
                //determin moved_in_group or steped_in_nex_grp
                bool moved_in_grp = item.CompletionDate == curren_dt_goup ;
                bool steped_in_new_grp = !moved_in_grp;

                if (first_iteration)
                {
                    first_iteration = false;
                    curren_dt_goup = item.CompletionDate;
                    acc_weight = item.CompletedBuildingBlockWeight;
                }
                //first iteration
                //initialize bufer: assign current date group,  assing acc_weiht
                else if (moved_in_grp)
                {
                    acc_weight += item.CompletedBuildingBlockWeight;
                }
                //moved in group 
                //update bufer: increment acc_weight

                else if (steped_in_new_grp)
                {
                    outp.Add(new DataPoint(DayNumberPrecise(curren_dt_goup, data.TimeStart),accWeight+= acc_weight));
                    curren_dt_goup = item.CompletionDate;
                    acc_weight = 0;
                    acc_weight += item.CompletedBuildingBlockWeight;
                }
                //steped in new group
                //flush bufer:    add kay pair of : current date goup, acc_weight
                //re-initialize bufer: assign current date group, reset acc_weight, increment acc_weight

               
            }
            //flush buffer
            outp.Add(new DataPoint(DayNumber(curren_dt_goup, data.TimeStart), accWeight+=acc_weight));

            return outp;
        }

        public DateTime DevCurrentWorkingDateTime { get; set; }
        public DateTime StartTime { get; set; }

        private static List<DataPoint> GetProgRateCurvPoints(IPlotableUserProgressData data)
        {
            List<DataPoint> master = new List<DataPoint>();
            //# PLAN A
            //foreach data point in the progress data points
            //make a list of data points around that point that covers a limited area(defined by the aproximat bell-width)
            //call that list CompletionPieceGroupContributionForProgRate, because that's what it actuallt is

            //combine all the contributions, point-wise,to get the master list and retun it

            //note: make sure to use consistent time-quantization between the contributions
            //in order to reduce the amount of points in the master,
            //and in order to make it possible to do the addition!

            //# PLAN B
            //this does it the other way around, i beleive this is more efficient
            //determin the beginin and endng point of the master
            //starting from the begining point to the end, with a quantization step; do the flowing:
            //iterate throgh the CompletionPieceGroups aka the data points from prog curv
            //foreach one, calculate the value that each one's bell curv yields at the current position
            //accumulat those values, and call that a point, 
            //move one to the next position, new point, using the same process


            //remember: use daynumber functions to convert your date time object to what oxy plot can plot

            //# PLAN B IMPLEMENTATION

            var ProgressPoints = GetProgCurvPoints(data);//todo use the same result found when in progress ploting to reduce overhead
            var master_start = ProgressPoints.Min((dp) => dp.X);//todo need to ad half of the width of the bell-curve in quetoin
            var master_end = ProgressPoints.Max((dp) => dp.X);


            double step = 1f / 24;//1 hour
            for(double position= master_start; position<master_end; position += step)
            {
                
                double master_point_y = data.CompletionPiece.Aggregate(0d, (acc, cur) => 
                    acc + (MathUtils.GausianKernel(position, 0.398, DayNumberPrecise( cur.CompletionDate, data.TimeStart))*cur.CompletedBuildingBlockWeight*5)
                );
                master.Add(new DataPoint(position, master_point_y));
            }


            return master;
        }
    }



    public class CompletionPiece : PlotingService.ICompletionPiece
    {
        public int CompletedBuildingBlockWeight { get; set; }

        public DateTime CompletionDate { get; set; }
    }

    public class PlotableData : Services.PlotingService.IPlotableUserProgressData
    {
        public ReadOnlyObservableCollection<PlotingService.ICompletionPiece> CompletionPiece { get; set; }
        public DateTime GoalDeadLine { get; set; }

        public int GoalWeight { get; set; }

        public DateTime TimeStart { get; set; }

    }
}
