using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STasks.Model;
using System.Collections.Specialized;
using System.Windows.Input;
using STasks.Common;
using STasks.Model.Bases;

namespace STasks.ViewModel
{
    public abstract class BaseProgressObjectViewModel : BaseViewModel
    {



        internal virtual void handleContainerAdded(STContainer newContainer) { }
        internal virtual void handleContainerRemoved(STContainer removedContainer) { }
        internal virtual void handleContainersCollectionReset(){}
        internal virtual void handleChildAdded(STBuildingBlock newBuildingBlock) { }
        internal virtual void handleChilRemoved(STBuildingBlock removedBuildingBlock) { }

        internal virtual void handleChildrenCollectionReset() { }


        private void handleContainersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    handleContainerAdded((STContainer)item);
                   
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                   
                    handleContainerRemoved((STContainer )item);
                    
                }
            }
            if(e.Action== NotifyCollectionChangedAction.Reset)
            {
                handleContainersCollectionReset();
            }

        }
        private void handleChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    handleChildAdded((STBuildingBlock)item);

                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {

                    handleChilRemoved((STBuildingBlock)item);

                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                handleChildrenCollectionReset();
            }
        }

        private ProgressObject Model;

        public BaseProgressObjectViewModel( STDOMObject model)
        {
            Model = model.Progress;
            if(model is STContainer)
            {
                STContainer model_as_contaner = (STContainer)model;
                model_as_contaner.Children.CollectionChanged += handleChildrenCollectionChanged;
                model_as_contaner.Containers.CollectionChanged += handleContainersCollectionChanged;
            }
            else if(model is STBuildingBlock)
            {

            }
            Model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ProgressObject.IsComplete))
                {
                    notif(nameof(IsAccomplished));
                }
                if (e.PropertyName == nameof(ProgressObject.Ratio))
                {
                    notif(nameof(DiscretProgressAsFloat));
                }
                notif(nameof(IsDoneVisible)); 
                 notif(nameof(IsCompletionRequiringAllowed)); 

            };
           
        }

        private bool _IsAccomplished;
        public bool IsAccomplished
        {
            set { Model.RequireCompletion (value); }
            get { return Model.IsComplete; }
        }

        /// <summary>
        /// UX specs 25-oct
        /// </summary>
        public bool IsCompletionRequiringAllowed
        {
            get { return Model.IsDetermined; }
        }
        /// <summary>
        /// ux specs 25-oct
        /// </summary>
        public bool IsDoneVisible
        {
            get { return Model.IsComplete&&Model.IsDetermined; }
        }


        public ICommand RequireCompletionCommand
        {
            get { return new MICommand<bool>(handleRequireCompletionCommand, (val) => IsCompletionRequiringAllowed); }
        }

        private void handleRequireCompletionCommand(bool value)
        {
            Model.RequireCompletion(value);
        }

        public float DiscretProgressAsFloat//todo names keepd for compablity with the UI, consider updating them
        {
            get { return Model.Ratio * 100; }
        }


    }
}
