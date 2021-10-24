using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace STasks.Model
{
    /// <summary>
    /// implement Observability, throu a basic set of events 
    /// ViewModels should make use of hese events to keep in sync with their models
    /// the goal is to avoid  unesecerry cpu pverhead of re constructing the whole lot when only few properties or collection elements change
    /// the model's properties chang is notified through the INotifyPropertyChanged
    /// while the Children list obeservability is hard coded using 3  events
    /// </summary>
    public abstract class STDOMObject : INotifyPropertyChanged
    {

        public STDOMObject()
        {
            if (this is STContainer) Progress = new ComplexProgress((STContainer)this);
            else if (this is STBuildingBlock) Progress = new SimpleProgress((STBuildingBlock)this);

            

        }

        public ProgressObject Progress { get; internal set; }
        public STContainer Parent { get; set; }
        public STContainer GetRoot()
        {
            if (Parent == null) return this as STContainer;
            else return Parent.GetRoot();
        }

        #region INotif stuff
        internal void notif(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }


    public abstract partial class STContainer : STDOMObject
    {
        public STContainer()
        {

        }
        public ComplexProgress Progress {
            get { return (ComplexProgress)base.Progress; }
            set { base.Progress = value; }
        }
        public ObservableCollection<STContainer> Containers { get; set; }
        public ObservableCollection<STBuildingBlock> Children { get; set; }
    }
    public abstract class STBuildingBlock : STDOMObject
    {
        public STBuildingBlock()
        {

        }
        public SimpleProgress Progress {
            get { return (SimpleProgress)base.Progress; }
            set { base.Progress = value; }
        }

    }




    public enum ProgressNotificationType { SimpleProgressGotApllied, ComplexProgGotCompletionRequire }

    public class InternalProgressTreeNotificationArgs<T>
    {
        public InternalProgressTreeNotificationArgs(ProgressObject initiator, ProgressNotificationType type, T data)
        {
            Type = type; Data = data; Initiator = initiator;
        }
        public ProgressNotificationType Type { get; internal set; }
        public T Data { get; internal set; }
        public ProgressObject Initiator { get; internal set; }
    }

   public struct NewOld<T>
    {
        public NewOld(T new_, T old_)
        {
            NewValue = new_; OldValue = old_;
        }
        public T NewValue { get; internal set; }
        public T OldValue { get; internal  set; }
        
    }
    public struct ApplyProgressArgs
    {
        public bool Value;
        public int Weight;
    }


    public  abstract partial class ProgressObject : IProgressObject // this is The ONLY thing the UI knows about
    {


        internal abstract void SetCompletion();
        internal abstract void ResetCompletion();


        internal virtual void HandleNotificaton<T>(InternalProgressTreeNotificationArgs<T> e)
        {
            if (e.Type == ProgressNotificationType.ComplexProgGotCompletionRequire)
            {
                bool value = (bool)(object)e.Data;
                if (value) SetCompletion();
                else ResetCompletion();
            }
        }
        
        internal abstract ProgressObject ParentProgressObject { get; }

        /// <summary>
        /// handling strts ath the first ascentor (direct parent), and doesnt apply on the initiator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Args"></param>
        internal protected void PropagateNotificationUp<T>(InternalProgressTreeNotificationArgs<T> Args)
        {
            if(Args.Initiator!=this)
            HandleNotificaton(Args);
            if(ParentProgressObject!=null)
            ParentProgressObject.PropagateNotificationUp(Args);
        }
        internal abstract IEnumerable< ProgressObject> ChildProgressObjects { get; }
        /// <summary>
        /// handeling applies on self as well;
        /// </summary>
        internal protected void PropagateNotificationDown<T>(InternalProgressTreeNotificationArgs<T> Args)
        {
            HandleNotificaton(Args);
            if(ChildProgressObjects!=null)
            foreach (var progObj in ChildProgressObjects)
            {
                progObj.PropagateNotificationDown(Args);
            }
           
        }


        

        /// <summary>
        /// causes a successive progres set or reset operation  to be propagated down the tree, with this initiator affected as well 
        /// </summary>
        /// <param name="SetOrReset"></param>
        public abstract void RequireCompletion(bool SetOrReset);

        /// <summary>
        /// normalized precentage of compleed tasks to total tasks, wth weights taken into account
        /// </summary>
        abstract public float Ratio { get; }
        public int Percent { get { return (int)(100f * Ratio); } }
        abstract public bool IsComplex { get; }
        /// <summary>
        /// the underlying STObject is an STContainer that has no building blocks children and has containers list of which all the elements have indetermined progress
        /// NOTE that a simple progress cannot be ndetermined
        /// Usage: at the UI end, the user should be able to know that the progress is indetermined also functionlly, the parent progress should byPass any inderermined progres children as if they were completed,
        /// </summary>
        abstract public bool IsDetermined { get; }
        /// <summary>
        /// the number of  descandant simmpleProgress (buildingBlocks) of while the IsComplete value is true
        /// </summary>
        abstract public int CompletedCount { get; } // not available on IsComplex=false
        /// <summary>
        /// the number of descandant simmpleProgress (buildingBlocks)
        /// </summary>
        abstract public int TotalCount { get; }// not available on IsComplex=false

        /// <summary>
        /// returns true if the simmpleProgress's value is true or the complexProgress's children are all complete
        /// </summary>
        abstract public bool IsComplete { get; }// chosed name IsComplete instead of IsAccomplished to avoid confusion with the earlier DependencyProgress framework

        
    }

    public class SimpleProgress : ProgressObject
    {
        public SimpleProgress(STBuildingBlock ttachedBB, int weight=1, bool value=false)
        {
            Weight = weight; Value = value;
            STBBObject = ttachedBB;
            Weight = weight; Value = value;
        }
        //SIMPLE prog can be set externally by user
        // woplex progress cannot be set by user

        //Simple prog always have determined percent value
        // complex prog's percent value can be indetermined in case the underlying STOBjetcs graph doesn contain building blocks, only containers


        private bool Value { get;  set; }
        public int Weight { get; private set; }

        private STBuildingBlock STBBObject;
        public override bool IsDetermined { get { return true; } }

        public override float Ratio { get { return Value ? 1: 0; } }
        public override bool IsComplex { get { return false; } }

        public override int CompletedCount { get { throw new Exception("SimpleProgress Doesnt support CompletedCount"); } }

        public override int TotalCount { get { throw new Exception("SimpleProgress Doesnt support TotalCount"); } }

        public override bool IsComplete { get { return Value; } }

        internal override void SetCompletion()
        {
            Value = true;
        }
        internal override void ResetCompletion()
        {
            Value = false;
        }

        internal override void HandleNotificaton<T>(InternalProgressTreeNotificationArgs<T> e)
        {
            base.HandleNotificaton<T>(e);
            if (e.Type == ProgressNotificationType.ComplexProgGotCompletionRequire)
            {
                bool value = (bool)(object)e.Data;
                if (value) SetCompletion();
                else ResetCompletion();
            }
        }

        internal override ProgressObject ParentProgressObject { get { return STBBObject?.Parent?.Progress; } }

        internal override IEnumerable<ProgressObject> ChildProgressObjects { get { return null; } }
        

        public void ApplyProgressValue(bool newVal, int newWeight)// user sourced action, should propagate up
        {
            ApplyProgressArgs old_ = new ApplyProgressArgs() { Value = Value, Weight = Weight };

            ApplyProgressArgs new_ = new ApplyProgressArgs() { Value = newVal, Weight = newWeight };
            if (old_.Weight!=new_.Weight || old_.Value!=new_.Value)//if something has changeed happened
            {
                Value = newVal;
                Weight = newWeight;
                PropagateNotificationUp(new InternalProgressTreeNotificationArgs<NewOld<ApplyProgressArgs>>(this, ProgressNotificationType.SimpleProgressGotApllied, new NewOld<ApplyProgressArgs>(new_,old_)));

            }
        }
        public override void RequireCompletion(bool newVal)//for consistency witth complex progress
        {
            if (Value != newVal)
            {
                Value = newVal;
                STBBObject.notif("Progress");
            }
           
        }

        /// <summary>
        /// shorthand for applyProgress when only one prop need to be updated
        /// </summary>
        public void ApplyValue(bool v)
        {
            ApplyProgressValue(v, Weight);
        }

        public void ApplyWeight(int w)
        {
            ApplyProgressValue(Value, w);
        }

        public static AdditiveProgress operator +(SimpleProgress sp1, SimpleProgress sp2)
        {
            Func<bool, int> asInt = (bool b) => { return b ? 1 : 0; };
            return new AdditiveProgress()
            {
                CompletedCount = (asInt(sp1.IsComplete) + asInt(sp2.IsComplete)),
                CompletedWeight = (asInt(sp1.IsComplete) * sp1.Weight + asInt(sp2.IsComplete) * sp2.Weight),
                TotalCount = 2,
                TotalWeight = sp1.Weight + sp2.Weight
            };
        }
    }

    /**

    //basic definitions:
    //progress applyin: the acion to take when the user marks a building block element as completed or uncompleted
    //progress refeshing: happens internally in the complexProgress, it involves re-compiting and updating one or more value of the 5 bufrered internal fields as necessary
    //upward propagatng: the successive calls on parent element's methods from children elements , to get progress up to date after some kindof event that provokes progress change

    //downward propagating: same but it goes in the parent-to-child direcion: it would usually be used in "completion state requiring"
    //completion requiring: the action that the user can perform on a non simple element (e.g getting a whole STContaner such as a Class or a Series to be fully Completed or uncompleted)
    // the mechnism involves downward propagating 


    //what types of progress refeshing mehods will be needed on complexProgress and what sort of events should trgger eche one of them

    //a "full refresh" should not happen alot, it happens if the children collection has been dramatically changed or replaced, or somewhere at the construction logic, what it does: i recursively navigates down the tree and examins every simpleprogress , a full reresh invokes all the sub contaner's full refresh
    //"descendant removed refresh": happens when an STObject desapears from the DOM, it simply involves substractng the integers, the event or methode call that would be responsible for this should ofc provide the number of tasks that got deleted and their weight
    //"descendant added refresh": similar job but in an additive manner
    //"simple descendant got progress apply referesh": will take place when some bulding block in the descendants graph got a "progress applyin" action, this event/method should provide the weitgh of the simple progress in question and it's new value and old value; or even providng the whole simpleprogress instance for syntaxt brivity
    //"complex descendant got CompletionRequring refresh": now this is tricky, the CompletionRequring is a downward propagating call, so we should be carfull not let every call trigger this on it's own, instead only the head source of that propagation aka the STContainer that got the USER INTERACTION should fire this upwards propagating refresh , the refresh method/event handler should recieve inforation on: the number of total blocks of the complexProgress in question, and their weghts, the value tht indicates whether it was a "completion" or a "reset" oeration
    //
    //ComplexProgress exposes a simplified piece of information that can tell ascendants 
    //just about everything: ConsolidatedComplexProgress, it basically captures the tree's
    //content in a simple structure of 4 properties: weightsum, completedWeightSum, tasksCount,
    //and completedTasksCount
    //
    //

        progress arithmetic:
        progress addition
        an STContainer with 3 biulding blocks whos Progress's are :
        p1: completed=true, weight=1
        p2: completed=true, weight=3
        p3: completed=false, weight=3
        p4: completed=false, weight=5

        the container then should woeke this out as folowing:
        normalized percent: (1+3)/(1+3+3+5) 
        totalCount: 4
        completedCount: 2
        we can define addition as the operation that yild a ConsolidatedComplexProgress
        totalCount: 4
        CompletedCont: 2
        TotalWeight: 12
        CompletedWeight: 4
        Percent: CompletedWeight/CompletedWeight
        
        IAdditiveProgress:


    */

    public class ComplexProgress : ProgressObject
    {

        public ComplexProgress(STContainer AttachedContainer)
        {
            STContainerElement = AttachedContainer;
            //registering event handlers to kkep sync wth dom


            STContainerElement.Children.CollectionChanged += handleDomChildrenCollectionChanged;
            STContainerElement.Containers.CollectionChanged += handleDomContainersCollectionChanged;


        }

        private void handleDomChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //added, removed, changed, 
            Refresh();
           if(e.Action== NotifyCollectionChangedAction.Add)
            {

            }
        }
        private void handleDomContainersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //added, removed, 
            Refresh();//todo this is temporary, trigger more efficient mechanisms base on the action that took place
        }

        AdditiveProgress GetChildrenSum()
        {
           return STContainerElement.Children.Aggregate(new AdditiveProgress(), (acc, cur) =>  acc + (cur.Progress as SimpleProgress));
        }
        AdditiveProgress GetContainersSum()
        {
            return STContainerElement. Containers.Aggregate(new AdditiveProgress(), (acc, cur) => acc + (cur.Progress as ComplexProgress).GetDescendantsAndSelfSum());
        }

        public AdditiveProgress GetDescendantsAndSelfSum()
        {
            return GetChildrenSum() + GetContainersSum();
        }

        public void Refresh()
        {
            Consolidated = GetDescendantsAndSelfSum();
        }

        private STContainer STContainerElement;

        public AdditiveProgress Consolidated;
        
        private int childrenBuildingBlocksCount;
        private int descendantsBuildingBlocksCount;
        private int completedBuildingBlocksCount;
        private int totalBuildingBlocksCount;
        private bool isDetermined;

       
        public override bool IsDetermined { get { return isDetermined; } }

      
        public override float Ratio { get { return (float)Consolidated.CompletedWeight/Consolidated.TotalWeight; } }
        public override bool IsComplex { get { return true; } }

        public override int CompletedCount { get { return Consolidated.CompletedCount; } }

        public override int TotalCount { get { return Consolidated.TotalCount; } }

        public override bool IsComplete { get { return Consolidated.CompletedWeight==Consolidated.TotalWeight; } }

        internal override ProgressObject ParentProgressObject
        {
            get
            {
                return STContainerElement?.Parent?. Progress;
            }
        }

        /// <summary>
        /// helper method: set's the progress to full completion,doesnt do anything else
        /// </summary>
        internal override void SetCompletion()
        {
            this.Consolidated.CompletedCount =  TotalCount;
            this.Consolidated.CompletedWeight =  Consolidated.TotalWeight;

        }
        /// <summary>
        /// helper method: set's the progress to zero completion, doesnt do anything else
        /// </summary>
        internal override void ResetCompletion()
        {
            this.Consolidated.CompletedCount = 0;
            this.Consolidated.CompletedWeight = 0;
        }

        internal override void HandleNotificaton<T>(InternalProgressTreeNotificationArgs<T> e) 
        {
            base.HandleNotificaton<T>(e);
            if(e.Type== ProgressNotificationType.SimpleProgressGotApllied)
            {
                this.Refresh();
                Trace.WriteLine("refreshing");
            }



        }
        internal override IEnumerable<ProgressObject> ChildProgressObjects
        {
            get
            {
                IEnumerable<ProgressObject> ProgressObjects;

                IEnumerable<STDOMObject> childrenBB = STContainerElement.Children;
                IEnumerable<STDOMObject> childrenContainers = STContainerElement.Containers;
                IEnumerable<STDOMObject> allChildrenNodes = childrenBB.Concat(childrenContainers);
                ProgressObjects = allChildrenNodes.Select((n) => n.Progress);
                return ProgressObjects;

            }
        }

        public override void RequireCompletion(bool SetOrReset)
        {
             PropagateNotificationDown(new InternalProgressTreeNotificationArgs<bool>(this, ProgressNotificationType.ComplexProgGotCompletionRequire, SetOrReset));

        }

       



    }

    public class AdditiveProgress : IAdditiveProgress
    {
        public int CompletedCount { get; set; }
        public int CompletedWeight { get; set; }
        public int TotalCount { get; set; }
        public int TotalWeight { get; set; }


        public static AdditiveProgress operator +(AdditiveProgress ap, AdditiveProgress ap2)
        {
            Func<bool, int> asInt = (bool b) => { return b ? 1 : 0; };
            return new AdditiveProgress()
            {
                CompletedCount = (ap.CompletedCount + ap2.CompletedCount),
                CompletedWeight = (ap.CompletedWeight + ap2.CompletedWeight),
                TotalCount = ap.TotalCount + ap2.TotalCount,
                TotalWeight = ap.TotalWeight + ap2.TotalWeight
            };
        }

        public static AdditiveProgress operator +(AdditiveProgress ap, SimpleProgress sp)
        {
            Func<bool, int> asInt = (bool b) => { return b ? 1 : 0; };
            return new AdditiveProgress()
            {
                CompletedCount = (ap.CompletedCount + asInt(sp.IsComplete)),
                CompletedWeight = (ap.CompletedWeight  + asInt(sp.IsComplete) * sp.Weight),
                TotalCount = ap.TotalCount+1,
                TotalWeight = ap.TotalWeight + sp.Weight
            };
        }
        public static AdditiveProgress operator +(SimpleProgress sp, AdditiveProgress ap)
        {
            return ap + sp;
        }


    }

    /// <summary>
    /// progress that can be summed up and gives a IAdditiveProgress
    /// </summary>
    public interface IAdditiveProgress
    {
        int CompletedCount { get; } 
        int TotalCount { get; }
        int CompletedWeight { get; } 
        int TotalWeight { get; }
        
    }


}
