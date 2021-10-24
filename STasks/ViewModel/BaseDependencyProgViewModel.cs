using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STasks.Model;
namespace STasks.ViewModel
{
    public class BaseDependencyProgViewModel : BaseViewModel
    {

        private ProgressDependency Model;

        public BaseDependencyProgViewModel( ProgressDependency model)
        {
            Model = model;
            Model.AccomplishmentChanged += (s, e) =>
            {
                notif(nameof(IsAccomplished));
            };
            Model.ProgressChanged += (s, e) =>
            {
                notif(nameof(DiscretProgressAsFloat));
            };
        }

        private bool _IsAccomplished;
        public bool IsAccomplished
        {
            set { Model.IsAccomplished = value; }
            get { return Model.IsAccomplished; }
        }

        public float DiscretProgressAsFloat
        {
            get { return Model.Progress.Progress * 100; }
        }


    }
}
