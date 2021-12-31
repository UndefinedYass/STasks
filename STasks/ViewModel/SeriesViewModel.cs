using STasks.Model;
using STasks.Model.Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace STasks.ViewModel
{
    public class SeriesViewModel : BaseProgressObjectViewModel
    {
        public SeriesViewModel(Series model): base(model)
        {
            SeriesModel = model;
            ExercisesVMS = new ObservableCollection<ExerciseViewModel>(model.Exercises.Select((ex) => new ExerciseViewModel((Exercise) ex)));
            
            Name =model.Usi.ToString();

        }

        internal override void handleContainerAdded(STContainer newContainer)
        {
            ExercisesVMS.Add(new ExerciseViewModel((Exercise)newContainer) { IsRenaming=true});
        }
        internal override void handleContainerRemoved(STContainer removedContainer)
        {
            Exercise rem = removedContainer as Exercise;
            ExerciseViewModel removedOne = ExercisesVMS.FirstOrDefault((cc) => cc.ModelExercise == rem);
            ExercisesVMS.Remove(removedOne);
        }
        internal override void handleContainersCollectionReset()
        {
            ExercisesVMS = new ObservableCollection<ExerciseViewModel>(SeriesModel.Exercises.Select((ex) => new ExerciseViewModel((Exercise)ex)));
            notif(nameof(ExercisesVMS));
        }
        public SeriesViewModel():base(Common.Mock.GetDummySeries())
        {
            
            ExercisesVMS = new ObservableCollection<ExerciseViewModel>(Common.Mock.GetDummySeries().Exercises.Select((ex) => new ExerciseViewModel((Exercise) ex)));
            Name = "DTime dummy";
        }

        private string _Name;
        public string Name
        {
            set { _Name = value; notif(nameof(Name)); }
            get { return _Name; }
        }


        private Common.USI _Usi;
        public Common.USI Usi
        {
            set { _Usi = value; notif(nameof(Usi)); }
            get { return _Usi; }
        }

        public ICommand ClearExercisesCommand
        {
            get { return new Common.MICommand(HandleClearExercisesCommand); }
        }

        private void HandleClearExercisesCommand()
        {
            
            SeriesModel.Exercises.Clear();
        }

        public ICommand AddExerciseCommand
        {
            get { return new Common.MICommand(HandleAddExerciseCommand); }
        }

       
        private string GetSmartNewExerciseTitle()
        {
            string smartTitle;
            Exercise lastex = SeriesModel.Exercises.LastOrDefault() as Exercise;
            if (lastex == null) smartTitle = "Exercise 1";
            else
            {
                var m =Regex.Match(lastex.Title, @"(.*)(\d+)");
                if (m.Success)
                {
                    int lst_ex_nm = int.Parse(m.Groups[2].Value);
                    smartTitle = m.Groups[1].Value + (lst_ex_nm+1).ToString();
                }
                else
                {
                    smartTitle = "Exercise 1";
                }
            }

                return smartTitle;
        }
       

        private void HandleAddExerciseCommand()
        {
            
            var new_ex = new Exercise() { Title = GetSmartNewExerciseTitle ()};
            SeriesModel.Exercises.Add(new_ex);
           // ExercisesVMS.Add(new ExerciseViewModel(new_ex) { IsRenaming = true });

        }

        public ICommand DeleteCommand
        {
            get { return new Common.MICommand(HandleDeleteCommand); }
        }

        private void HandleDeleteCommand()
        {
            (SeriesModel?.Parent as Class)?.Containers?.Remove(SeriesModel);
        }

        public ObservableCollection<ExerciseViewModel> ExercisesVMS { get; set; } = null;
        public Series SeriesModel { get; private set; }
        public bool IsRenaming { get; internal set; }
    }
}
