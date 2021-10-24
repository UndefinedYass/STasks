using STasks.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.ViewModel
{
    public class SeriesViewModel : BaseDependencyProgViewModel
    {
        public SeriesViewModel(Series model): base(model)
        {
            SeriesModel = model;
            ExercisesVMS = new ObservableCollection<ExerciseViewModel>(model.Exercises.Select((ex) => new ExerciseViewModel(ex)));
            
            Name =model.Usi.ToString();
            
            SeriesModel.ExerciseRemoved += (s, e) => {
                ExerciseViewModel deleedItem = ExercisesVMS.FirstOrDefault((evm) => evm.ModelExercise==e);
                if (deleedItem != null)
                    ExercisesVMS.Remove(deleedItem);
            };

        }

        public SeriesViewModel():base(Common.Mock.GetDummySeries())
        {
            
            ExercisesVMS = new ObservableCollection<ExerciseViewModel>(Common.Mock.GetDummySeries().Exercises.Select((ex) => new ExerciseViewModel(ex)));
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



        public ObservableCollection<ExerciseViewModel> ExercisesVMS { get; set; } = null;
        public Series SeriesModel { get; private set; }
    }
}
