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
using STasks.Model.Bases;
namespace STasks.Model
{
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

            Trace.WriteLine(STContainerElement == null);
            Trace.WriteLine(STContainerElement?.Children == null);
            STContainerElement.Children.CollectionChanged += handleDomChildrenCollectionChanged;
            STContainerElement.Containers.CollectionChanged += handleDomContainersCollectionChanged;

            Consolidated = new AdditiveProgress();
        }

        public void handleDomChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //added, removed, changed, 
            Refresh();
            PropagateNotificationUp(new InternalProgressTreeNotificationArgs<object>(this, ProgressNotificationType.DescendantComplexProgAttachedDomElementGotChildrenOrContainersCollectionChanged, null));

            if (e.Action == NotifyCollectionChangedAction.Add)
            {

            }
        }
        public void handleDomContainersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Trace.WriteLine("containers collaction changed");
            //added, removed, 
            Refresh();//todo this is temporary, trigger more efficient mechanisms base on the action that took place
            PropagateNotificationUp(new InternalProgressTreeNotificationArgs<object>(this, ProgressNotificationType.DescendantComplexProgAttachedDomElementGotChildrenOrContainersCollectionChanged, null));
        }

        AdditiveProgress GetChildrenSum()
        {
            return STContainerElement.Children.Aggregate(new AdditiveProgress(), (acc, cur) => acc + (cur.Progress as SimpleProgress));
        }
        AdditiveProgress GetContainersSum()
        {
            return STContainerElement.Containers.Aggregate(new AdditiveProgress(), (acc, cur) => acc + (cur.Progress as ComplexProgress).GetDescendantsAndSelfSum());
        }

        public AdditiveProgress GetDescendantsAndSelfSum()
        {
            return GetChildrenSum() + GetContainersSum();
        }

        public void Refresh()
        {
            Consolidated = GetDescendantsAndSelfSum();

        }

        public STContainer STContainerElement;



        private AdditiveProgress _Consolidated;
        public AdditiveProgress Consolidated
        {
            set
            {
                _Consolidated = value;
                notif(nameof(Consolidated));
                notif(nameof(IsComplete));
                notif(nameof(IsDetermined));
                notif(nameof(ProgressPercent));
                notif(nameof(Ratio));
                notif(nameof(Percent));
            }
            get { return _Consolidated; }
        }



        /* private int childrenBuildingBlocksCount;
         private int descendantsBuildingBlocksCount;
         private int completedBuildingBlocksCount;
         private int totalBuildingBlocksCount;*/


        public override bool IsDetermined { get { return TotalCount > 0; } }


        public override float Ratio { get { return (float)Consolidated.CompletedWeight / Consolidated.TotalWeight; } }
        public override bool IsComplex { get { return true; } }

        public override int CompletedCount { get { return Consolidated.CompletedCount; } }

        public override int TotalCount { get { return Consolidated.TotalCount; } }

        public override bool IsComplete { get { return Consolidated.CompletedWeight == Consolidated.TotalWeight; } }

        internal override ProgressObject ParentProgressObject
        {
            get
            {
                return STContainerElement?.Parent?.Progress;
            }
        }

        /// <summary>
        /// helper method: set's the progress to full completion,doesnt do anything else
        /// </summary>
        internal override void SetCompletion()
        {
            this.Consolidated.CompletedCount = TotalCount;
            this.Consolidated.CompletedWeight = Consolidated.TotalWeight;
            this.Consolidated = this.Consolidated;

        }
        /// <summary>
        /// helper method: set's the progress to zero completion, doesnt do anything else
        /// </summary>
        internal override void ResetCompletion()
        {
            this.Consolidated.CompletedCount = 0;
            this.Consolidated.CompletedWeight = 0;
            this.Consolidated = this.Consolidated;

        }

        internal override void HandleNotificaton<T>(InternalProgressTreeNotificationArgs<T> e)
        {
            base.HandleNotificaton<T>(e);
            if (e.Type == ProgressNotificationType.SimpleProgressGotApllied)
            {
                this.Refresh();
                Trace.WriteLine("refreshing");
            }
            if (e.Type == ProgressNotificationType.AncestorComplexProgGotCompletionRequire)
            {
                if ((bool)(object)e.Data == true)
                    this.SetCompletion();
                else
                {
                    this.ResetCompletion();
                }
            }
            if (e.Type == ProgressNotificationType.DescendantComplexProgGotCompletionRequire)
            {
                this.Refresh();
            }
            if (e.Type == ProgressNotificationType.DescendantComplexProgAttachedDomElementGotChildrenOrContainersCollectionChanged)// ok this definitely  broke the record as the longest name i ever used
            {
                Refresh();
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
            PropagateNotificationDown(new InternalProgressTreeNotificationArgs<bool>(this, ProgressNotificationType.AncestorComplexProgGotCompletionRequire, SetOrReset));
            PropagateNotificationUp(new InternalProgressTreeNotificationArgs<string>(this, ProgressNotificationType.DescendantComplexProgGotCompletionRequire, "todo effecientely"));

        }





    }

}
