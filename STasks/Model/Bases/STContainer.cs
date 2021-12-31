using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STasks.Model.Bases;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;

namespace STasks.Model.Bases
{
    public abstract partial class STContainer : STDOMObject
    {
        public STContainer()
        {
            Debug.WriteLine("STContainer ctor this is: " + this.GetType().ToString());
            Containers = new ObservableCollection<STContainer>();
            Children = new ObservableCollection<STBuildingBlock>();
            Progress = new ComplexProgress((STContainer)this);


            Containers.CollectionChanged += HandleContainersCollectionChanged;
            Children.CollectionChanged += HandleChildrenCollectionChanged;

        }

        public IEnumerable<STBuildingBlock> GetAllChildAndDescendantBuildingBlocks()
        {
            IEnumerable<STBuildingBlock> desc = this.Children;
            foreach (var co in Containers)
            {
                desc = desc.Concat(co.GetAllChildAndDescendantBuildingBlocks());
            }
            return desc;
        }

        private void HandleContainersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    STContainer added_elem = item as STContainer;
                    added_elem.Parent = this;
                    Trace.WriteLine("auto hooked parent (" + this.GetType().Name + ") to element of type:" + added_elem.GetType().Name);
                }


            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    STContainer removed_elem = item as STContainer;

                    removed_elem.Parent = null;
                }

            }
        }

        private void HandleChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    STBuildingBlock added_elem = item as STBuildingBlock;
                    added_elem.Parent = this;
                }


            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    STBuildingBlock removed_elem = item as STBuildingBlock;
                    removed_elem.Parent = null;
                }

            }
        }

        public new ComplexProgress Progress
        {

            get { return (ComplexProgress)base.Progress; }
            set { base.Progress = value; }
        }
        private ObservableCollection<STContainer> _ontainers;
        public ObservableCollection<STContainer> Containers

        {
            get { return _ontainers; }
            set
            {
                _ontainers = value;
                foreach (var item in _ontainers)
                {
                    item.Parent = this;
                }
                _ontainers.CollectionChanged += HandleContainersCollectionChanged;
                if (Progress != null)
                    _ontainers.CollectionChanged += Progress.handleDomContainersCollectionChanged;
            }
        }
        private ObservableCollection<STBuildingBlock> _children;
        public ObservableCollection<STBuildingBlock> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                foreach (var item in _children)
                {
                    item.Parent = this;
                }
            }
        }

        public Guid guid { get; internal set; }
    }

}
