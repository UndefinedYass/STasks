using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace STasks.Model
{
    /// <summary>
    /// this abstract class provides the basic functionality relaated the the parent-chldren 
    /// relashioship between accoumplishable elements, such as Series, Exercise, Task ..
    /// acouplshing all chldren of an elemnt causes the parrent element to get accouplshed 
    /// accouplishing a parent element auatomatically propagates the accoumplishement to all of it's descendants
    /// in an eficient circularity-free machanism, that is, for example, 
    /// to prevent the accouplishment action from bouncing back to the parent, which may engage
    /// unecessary event/binding calls and cpu usage 
    /// also when children accomplishmet state change, that change doesnt have to propagate all the way
    /// to the root, but it should stop at the first ascendant whos IsAccomplished value didn't get affected
    /// </summary>
    public class ProgressDependency: IAccomplishable
    {




        public ProgressDependency(ProgressDependency parent,bool initialAccomplishmentState)
        {
            ChildrenList = new List<ProgressDependency>();
            Progress = new DiscretProgress(1, 0);
            if (parent != null)
            {
                Parent = parent;
                Parent.RegisterChild(this);
            }

        }
        ~ProgressDependency()
        {
            if (Parent != null)
            {
                Parent.UnRegisterChild(this);
            }
            
        }
        internal ProgressDependency Parent { get; set; }
        /// <summary>
        /// sppecifies whether this progress dep elements is a TaskS elemt, or any bottom most descendant elemnt
        ///  of which the progress can only be binary , 100|1
        /// </summary>
        internal bool IsBuildingBlock { get; set; }
        internal DiscretProgress Progress { get; set; } 

        private void OnSetExternally(bool newValue)
        {
            //if change, set and start propagatiing in bothe directions , else do nothing
            //also propagate the discret progress in both directions, the down direction is complicated tho
            if (IsAccomplished != newValue)
            {
                SetIsAccomplished(newValue);
                ChildrenList.ForEach((child) => child.ForceChangeByParent(newValue));
                
                PropagateProgressChangeByParent(newValue); //it's more lke "bySelf" 
                if (Parent != null)
                {
                    Parent.RequireChildrenStateCheck();
                    Parent.RequireChildrenProgressStateCheck();
                }
            }
        }
        private int IsBuildingBlockAsInt {
            get { return IsBuildingBlock ? 1 : 0; }
        }



        /// <summary>
        /// with self included
        /// </summary>
        private int GetAllDescandantBuildingBlocksCount()
        {
            if (IsBuildingBlock) return 1;

            else if (ChildrenList != null)
                return ChildrenList.Aggregate(0, (acc, c) => acc + c.GetAllDescandantBuildingBlocksCount());
            else
                return 0;

        }
        /// <summary>
        /// sets progress on self, invokes the propagate function on all children
        /// </summary>
        /// <param name="newValue"></param>
        private void PropagateProgressChangeByParent(bool newValue)
        {
            ForceProgressChangeByParent(newValue);
            ChildrenList.ForEach((c) => c.PropagateProgressChangeByParent(newValue));
        }

        private void ForceProgressChangeByParent(bool newValue)
        {
            int desc = GetAllDescandantBuildingBlocksCount();
            SetProgress(new DiscretProgress(desc, newValue ? desc : 0));
        }

        private void OnSetByParent(bool newValue)
        {
            //if there was change set and propagate to children, else do nothing
            if (IsAccomplished != newValue)
            {
                SetIsAccomplished(newValue);
                if(ChildrenList!=null)
                ChildrenList.ForEach((child) => child.ForceChangeByParent(newValue));
            }
        }
        //user acton: compute number of descandat tasks:
        //proagate up ths event: 

        private void OnProgressSetByParent(bool newValue)
        {
            //if there was change set and propagate to children, else do nothing
            SetProgress(new DiscretProgress(Progress.AllDescendantTasksCout,
                Progress.AllDescendantTasksCout*(newValue?1:0)));
            
            
                if (ChildrenList != null)
                    ChildrenList.ForEach((child) => child.ForceProgressChangeByParent(newValue));
            
        }
        private void OnSetByChildrenChange(bool newValue)
        {
            //if change set and propagate up, else do nothing
            if (IsAccomplished != newValue)
            {
                SetIsAccomplished(newValue);
                Parent?.RequireChildrenStateCheck();
            }
        }

        /// <summary>
        /// Note: only call from child
        /// gets called by children on their parents to propagated change up the tree
        /// </summary>
        private void RequireChildrenStateCheck()
        {
            bool _children_accomp = GetAreAllChildrenAccomplished();
            OnSetByChildrenChange(_children_accomp);
            
        }
        /// <summary>
        /// Note: only call from child
        /// gets called by children on their parents to propagated change up the tree
        /// </summary>
        private void RequireChildrenProgressStateCheck()
        {
            var _children_progress = GetAllChildrenProgress();
            onUpdateChildrenProgress(_children_progress);

        }

        private void onUpdateChildrenProgress(DiscretProgress _children_progress)
        {
            //if change set and propagate up, else do nothing
            if (_children_progress != Progress)
            {
                System.Diagnostics.Trace.WriteLine($"prog: {_children_progress}");
                SetProgress(_children_progress);
                Parent?.RequireChildrenProgressStateCheck();
            }
        }

        /// <summary>
        /// Note: only call from parent
        /// gets called by parent on their children to propagated change down the tree
        /// </summary>
        private void ForceChangeByParent(bool vale)
        {
            OnSetByParent(vale);

        }

      

        private List<ProgressDependency> ChildrenList { get; set; }


        private bool _isAccomplished;
        public bool IsAccomplished
        {
            get
            {
                return _isAccomplished;
               
            }
            set
            {
                OnSetExternally(value);
            }
        }

        public event EventHandler<bool> AccomplishmentChanged;
        public event EventHandler<DiscretProgress> ProgressChanged;

        private void RegisterChild(ProgressDependency newChild)
        {
            ChildrenList.Add(newChild);
        }
        internal void UnRegisterChild(ProgressDependency newChild)
        {
            ChildrenList.Remove(newChild);
            
            RequireChildrenStateCheck();
            RequireChildrenProgressStateCheck();
        }




        private bool GetAreAllChildrenAccomplished()
        {
            return ChildrenList.All((child) => child.IsAccomplished);
        }
        internal DiscretProgress GetAllChildrenProgress()
        {
            if (IsBuildingBlock)
            {
                return IsAccomplished ? new DiscretProgress(1, 1) : new DiscretProgress(1, 0);
            }
            else
            {
                if (ChildrenList != null)
                    return ChildrenList.Aggregate(new DiscretProgress(0, 0), (acc, child) => acc + child.GetAllChildrenProgress());
                else return new DiscretProgress(0, 0);
            }
        }



        private void SetIsAccomplished(bool Value)
        {
            if (_isAccomplished != Value)
            {
                _isAccomplished = Value;
                AccomplishmentChanged?.Invoke(this, _isAccomplished);
            }
        }
        private void SetProgress(DiscretProgress Value)
        {
            if (Progress != Value)
            {
                Progress = Value;
                ProgressChanged?.Invoke(this, Progress);
            }
        }
    }




    public interface IAccomplishable
    {
        event EventHandler<bool> AccomplishmentChanged;
        bool IsAccomplished { get; set; }
    }



    public struct DiscretProgress
    {
        internal static DiscretProgress zero = new DiscretProgress(0,0);

        public DiscretProgress(int descendantsCount,int accompCount)
        {
            AccomplishedTasksCount = accompCount; AllDescendantTasksCout = descendantsCount;
        }
        public int AccomplishedTasksCount { get; set; }
        public int AllDescendantTasksCout { get; set; }
        public static bool operator ==(DiscretProgress dp1, DiscretProgress dp2)
        {
            return dp1.AccomplishedTasksCount == dp2.AccomplishedTasksCount && dp1.AllDescendantTasksCout == dp2.AllDescendantTasksCout;
        }
        public static bool operator !=(DiscretProgress dp1, DiscretProgress dp2)
        {
            return  dp1.AccomplishedTasksCount != dp2.AccomplishedTasksCount || dp1.AllDescendantTasksCout != dp2.AllDescendantTasksCout;

        }

        /// <summary>
        /// Normalized progeres float, can return Nan in a division by zero situation
        /// </summary>
        public float Progress { get { return AllDescendantTasksCout==0?float.NaN: (float)AccomplishedTasksCount / AllDescendantTasksCout; } }
        /// <summary>
        /// PERCENT progeres AS INT, can return Nan in a division by zero situation
        /// </summary>
        public int ProgressPercent { get { return AllDescendantTasksCout == 0 ? 0 :(int) (100f* (float)AccomplishedTasksCount / AllDescendantTasksCout); } }
        public static DiscretProgress operator+(DiscretProgress dp1, DiscretProgress dp2 )
        {
            return new DiscretProgress(dp1.AllDescendantTasksCout + dp2.AllDescendantTasksCout, dp1.AccomplishedTasksCount + dp2.AccomplishedTasksCount);
        }
        public override string ToString()
        {
            return $"acc:{AccomplishedTasksCount}, tot:{AllDescendantTasksCout}, p:{Progress}";
        }
        public override bool Equals(object obj)
        {
            return (obj is DiscretProgress) && ((DiscretProgress)obj) == this;
        }
    }

}
