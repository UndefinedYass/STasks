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

namespace STasks.Model.Bases
{
    public abstract partial class ProgressObject : INotifyPropertyChanged, IProgressObject // this is The ONLY thing the UI knows about
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void notif(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        internal abstract void SetCompletion();
        internal abstract void ResetCompletion();


        internal virtual void HandleNotificaton<T>(InternalProgressTreeNotificationArgs<T> e)
        {
            if (e.Type == ProgressNotificationType.AncestorComplexProgGotCompletionRequire)
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
            Debug.WriteLine($"receved PropagateNotificationUp call, this object type is: " + this.GetType().ToString());
            if (this is ComplexProgress)
            {
                Debug.Write("the attached DOMobject's type is: ");
                Debug.WriteLine((this as ComplexProgress).STContainerElement.GetType().ToString());
            }
            else if (this is SimpleProgress)
            {
                Debug.Write("the attached DOMobject's type is: ");
                Debug.WriteLine((this as SimpleProgress).STBBObject.GetType().ToString());

            }
            if (Args.Initiator != this)
                HandleNotificaton(Args);
            if (ParentProgressObject != null)
            {
                Debug.WriteLine("calling propa on parent");
                ParentProgressObject.PropagateNotificationUp(Args);
            }

            else
            {
                Debug.WriteLine("ParentProgressObject is null, stoping propagation");
            }
        }
        internal abstract IEnumerable<ProgressObject> ChildProgressObjects { get; }
        /// <summary>
        /// handeling applies on self as well;
        /// </summary>
        internal protected void PropagateNotificationDown<T>(InternalProgressTreeNotificationArgs<T> Args)
        {
            HandleNotificaton(Args);
            if (ChildProgressObjects != null)
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
        public int ProgressPercent { get { return Percent; } }
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















    public enum ProgressNotificationType
    {
        SimpleProgressGotApllied, AncestorComplexProgGotCompletionRequire,
        DescendantComplexProgGotCompletionRequire,
        DescendantComplexProgAttachedDomElementGotChildrenOrContainersCollectionChanged
    }

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
        public T OldValue { get; internal set; }

    }
    public struct ApplyProgressArgs
    {
        public bool Value;
        public int Weight;
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
                CompletedWeight = (ap.CompletedWeight + asInt(sp.IsComplete) * sp.Weight),
                TotalCount = ap.TotalCount + 1,
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
